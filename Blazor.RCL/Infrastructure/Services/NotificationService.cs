using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazor.RCL.Application.Interfaces;
using Blazor.RCL.Application.Models.Notifications;
using Blazor.RCL.Domain.Entities.Notifications;
using Blazor.RCL.NLog.LogService.Interface;
using Microsoft.Extensions.Caching.Memory;

namespace Blazor.RCL.Infrastructure.Services
{
    /// <summary>
    /// Core notification service implementation for creating and managing notifications
    /// </summary>
    public class NotificationService : INotificationService
    {
        private readonly INotificationMessageRepository _notificationRepository;
        private readonly INotificationDeliveryRepository _deliveryRepository;
        private readonly IUserNotificationSettingsService _userSettingsService;
        private readonly IApplicationNotificationProfileService _appProfileService;
        private readonly IEmailNotificationService _emailService;
        private readonly INotificationPublisher _publisher;
        private readonly IUserSettingsRepository _userSettingsRepository;
        private readonly ILogHelper _logger;
        private readonly MemoryCache _pollingCache;

        public NotificationService(
            INotificationMessageRepository notificationRepository,
            INotificationDeliveryRepository deliveryRepository,
            IUserNotificationSettingsService userSettingsService,
            IApplicationNotificationProfileService appProfileService,
            IEmailNotificationService emailService,
            INotificationPublisher publisher,
            IUserSettingsRepository userSettingsRepository,
            ILogHelper logger)
        {
            _notificationRepository = notificationRepository;
            _deliveryRepository = deliveryRepository;
            _userSettingsService = userSettingsService;
            _appProfileService = appProfileService;
            _emailService = emailService;
            _publisher = publisher;
            _userSettingsRepository = userSettingsRepository;
            _logger = logger;
            
            // Initialize polling cache only for duplicate prevention in fallback mode
            _pollingCache = new MemoryCache(new MemoryCacheOptions
            {
                SizeLimit = 10000,
                ExpirationScanFrequency = TimeSpan.FromMinutes(1)
            });
        }

        public async Task<NotificationMessage> CreateNotificationAsync(CreateNotificationRequest request)
        {
            try
            {
                // Validate application exists
                var appProfile = await _appProfileService.GetApplicationProfileAsync(request.SourceApplication);
                if (appProfile == null || !appProfile.IsActive)
                {
                    throw new InvalidOperationException($"Application '{request.SourceApplication}' not found or inactive");
                }

                // Create the notification
                var notification = new NotificationMessage
                {
                    Id = Guid.NewGuid(),
                    MessageId = $"{request.SourceApplication}-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid():N}",
                    SourceApplication = request.SourceApplication,
                    Title = request.Title,
                    Content = request.Content,
                    Severity = (int)request.Severity,
                    AlertType = (int)request.AlertType,
                    RequiresConfirmation = request.RequiresConfirmation,
                    Metadata = request.Metadata,
                    CreatedBy = request.CreatedBy,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                await _notificationRepository.CreateAsync(notification);
                
                _logger.LogInfo("Notification created", "NotificationCreated", new { NotificationId = notification.Id, SourceApplication = notification.SourceApplication });

                // If specific users are targeted, send to them
                if (request.TargetUsernames?.Any() == true)
                {
                    await SendNotificationAsync(notification.Id, request.TargetUsernames, request.DeliveryChannels);
                }
                
                // If specific roles are targeted, send to all users with those roles
                if (request.TargetRoles?.Any() == true)
                {
                    foreach (var role in request.TargetRoles)
                    {
                        await SendNotificationToRoleAsync(notification.Id, role, request.DeliveryChannels);
                    }
                }

                return notification;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error creating notification", ex);
                throw;
            }
        }

        public async Task<IEnumerable<NotificationMessage>> CreateNotificationBatchAsync(IEnumerable<CreateNotificationRequest> requests)
        {
            var notifications = new List<NotificationMessage>();

            foreach (var request in requests)
            {
                var notification = await CreateNotificationAsync(request);
                notifications.Add(notification);
            }

            return notifications;
        }

        public async Task<IEnumerable<NotificationDelivery>> SendNotificationAsync(
            Guid notificationId, 
            IEnumerable<string> usernames, 
            DeliveryChannel? channels = null)
        {
            try
            {
                var notification = await _notificationRepository.GetByIdAsync(notificationId);
                if (notification == null)
                {
                    throw new InvalidOperationException($"Notification {notificationId} not found");
                }

                var deliveries = new List<NotificationDelivery>();

                foreach (var username in usernames.Distinct())
                {
                    // Check if user should receive this notification
                    var (shouldReceive, effectiveChannels) = await _userSettingsService.ShouldUserReceiveNotificationAsync(username, notification);
                    
                    if (!shouldReceive)
                    {
                        _logger.LogDebug("User should not receive notification", new { Username = username, NotificationId = notificationId });
                        continue;
                    }

                    // Use specified channels or user's preferred channels
                    var deliveryChannels = channels ?? effectiveChannels;

                    // Create delivery records
                    if (deliveryChannels.HasFlag(DeliveryChannel.SignalR))
                    {
                        var delivery = await CreateDeliveryAsync(notification, username, DeliveryChannel.SignalR);
                        deliveries.Add(delivery);
                    }

                    if (deliveryChannels.HasFlag(DeliveryChannel.Email))
                    {
                        var delivery = await CreateDeliveryAsync(notification, username, DeliveryChannel.Email);
                        deliveries.Add(delivery);
                        
                        // Queue email
                        await _emailService.QueueEmailNotificationAsync(notification, username);
                    }
                }

                // Publish notification for real-time delivery (when Redis available)
                // In polling mode, this is a no-op and clients will poll the database
                var signalRUsers = deliveries
                    .Where(d => d.DeliveryChannel == (int)DeliveryChannel.SignalR)
                    .Select(d => d.Username)
                    .Distinct();
                
                if (signalRUsers.Any())
                {
                    await _publisher.PublishAsync(notification, signalRUsers);
                    
                    // Update delivery status based on publisher mode
                    foreach (var delivery in deliveries.Where(d => d.DeliveryChannel == (int)DeliveryChannel.SignalR))
                    {
                        await _deliveryRepository.UpdateDeliveryStatusAsync(
                            delivery.Id, 
                            _publisher.IsRealTimeAvailable ? DeliveryStatus.Delivered : DeliveryStatus.Pending);
                    }
                }

                return deliveries;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error sending notification", ex);
                throw;
            }
        }

        public async Task<IEnumerable<NotificationDelivery>> SendNotificationToApplicationUsersAsync(
            Guid notificationId, 
            string applicationName,
            DeliveryChannel? channels = null)
        {
            try
            {
                var notification = await _notificationRepository.GetByIdAsync(notificationId);
                if (notification == null)
                {
                    throw new InvalidOperationException($"Notification {notificationId} not found");
                }
                
                // Get subscribed users and send individually
                var subscribedUsers = await _appProfileService.GetSubscribedUsersAsync(applicationName);
                var deliveries = await SendNotificationAsync(notificationId, subscribedUsers, channels);
                
                // Also publish to application channel for any connected users
                await _publisher.PublishToApplicationAsync(notification, applicationName);
                
                return deliveries;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error sending notification to application users", ex);
                throw;
            }
        }

        public async Task<IEnumerable<NotificationDelivery>> SendNotificationToRoleAsync(
            Guid notificationId,
            string role,
            DeliveryChannel? channels = null)
        {
            try
            {
                var notification = await _notificationRepository.GetByIdAsync(notificationId);
                if (notification == null)
                {
                    throw new InvalidOperationException($"Notification {notificationId} not found");
                }
                
                // Get all users with the specified role
                var usersWithRole = await _userSettingsRepository.GetUsernamesWithRoleAsync(role);
                
                if (!usersWithRole.Any())
                {
                    _logger.LogMessage($"No users found with role: {role}");
                    return Enumerable.Empty<NotificationDelivery>();
                }
                
                _logger.LogInfo($"Sending notification to {usersWithRole.Count()} users with role: {role}", 
                    "NotificationToRole", 
                    new { NotificationId = notificationId, Role = role, UserCount = usersWithRole.Count() });
                
                // Send to all users with the role
                var deliveries = await SendNotificationAsync(notificationId, usersWithRole, channels);
                
                // Also publish to role channel for any connected users
                await _publisher.PublishToRoleAsync(notification, role);
                
                return deliveries;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending notification to role: {role}", ex);
                throw;
            }
        }

        public async Task<bool> ConfirmNotificationAsync(Guid notificationId, string username)
        {
            try
            {
                var deliveries = await _deliveryRepository.GetByNotificationAndUserAsync(notificationId, username);
                var confirmed = false;

                foreach (var delivery in deliveries.Where(d => d.DeliveryStatus != (int)DeliveryStatus.Confirmed))
                {
                    await _deliveryRepository.UpdateDeliveryStatusAsync(delivery.Id, DeliveryStatus.Confirmed);
                    delivery.ConfirmedAt = DateTime.UtcNow;
                    confirmed = true;
                }

                if (confirmed)
                {
                    _logger.LogInfo("Notification confirmed", "NotificationConfirmed", new { NotificationId = notificationId, Username = username });
                }

                return confirmed;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error confirming notification", ex);
                throw;
            }
        }

        public async Task<IEnumerable<UserNotificationViewModel>> GetUserNotificationsAsync(
            string username, 
            bool includeConfirmed = false)
        {
            try
            {
                var deliveries = await _deliveryRepository.GetUserDeliveriesAsync(username, includeConfirmed);
                var notifications = await BuildUserNotificationViewModelsAsync(deliveries);
                
                // Only filter duplicates in polling mode
                if (!_publisher.IsRealTimeAvailable)
                {
                    notifications = notifications.Where(n => 
                    {
                        var cacheKey = $"{username}:{n.NotificationId}";
                        if (_pollingCache.TryGetValue(cacheKey, out _))
                        {
                            return false; // Already delivered
                        }
                        
                        // Mark as delivered with 5-minute expiry
                        _pollingCache.Set(cacheKey, true, new MemoryCacheEntryOptions
                        {
                            SlidingExpiration = TimeSpan.FromMinutes(5),
                            Size = 1
                        });
                        
                        return true;
                    }).ToList();
                }
                
                return notifications;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error getting user notifications", ex);
                throw;
            }
        }

        public async Task<IEnumerable<UserNotificationViewModel>> GetUserApplicationNotificationsAsync(
            string username, 
            string applicationName,
            bool includeConfirmed = false)
        {
            try
            {
                var allDeliveries = await _deliveryRepository.GetUserDeliveriesAsync(username, includeConfirmed);
                var appDeliveries = allDeliveries.Where(d => 
                    d.NotificationMessage?.SourceApplication == applicationName);
                
                return await BuildUserNotificationViewModelsAsync(appDeliveries);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error getting user application notifications", ex);
                throw;
            }
        }

        public async Task<int> GetUnconfirmedNotificationCountAsync(string username)
        {
            try
            {
                var deliveries = await _deliveryRepository.GetUnconfirmedDeliveriesAsync(username);
                return deliveries.Count();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error getting unconfirmed count", ex);
                throw;
            }
        }

        public async Task<IEnumerable<NotificationDelivery>> RetryFailedDeliveriesAsync(
            Guid notificationId, 
            int maxRetries = 3)
        {
            try
            {
                var failedDeliveries = await _deliveryRepository.GetFailedDeliveriesByNotificationAsync(notificationId);
                var retriedDeliveries = new List<NotificationDelivery>();

                foreach (var delivery in failedDeliveries.Where(d => d.RetryCount < maxRetries))
                {
                    delivery.RetryCount++;
                    await _deliveryRepository.UpdateAsync(delivery);

                    // Retry based on channel
                    if (delivery.DeliveryChannel == (int)DeliveryChannel.SignalR && delivery.NotificationMessage != null)
                    {
                        // Publish via publisher (Redis or polling)
                        await _publisher.PublishAsync(delivery.NotificationMessage, new[] { delivery.Username });
                        
                        // Update delivery status based on mode
                        await _deliveryRepository.UpdateDeliveryStatusAsync(
                            delivery.Id,
                            _publisher.IsRealTimeAvailable ? DeliveryStatus.Delivered : DeliveryStatus.Pending);
                    }
                    else if (delivery.DeliveryChannel == (int)DeliveryChannel.Email)
                    {
                        await _emailService.QueueEmailNotificationAsync(delivery.NotificationMessage!, delivery.Username, EmailPriority.High);
                    }

                    retriedDeliveries.Add(delivery);
                }
                
                _logger.LogInfo("Retried failed deliveries", "DeliveriesRetried", new { NotificationId = notificationId, RetriedCount = retriedDeliveries.Count });

                return retriedDeliveries;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error retrying failed deliveries", ex);
                throw;
            }
        }

        public async Task<int> ArchiveOldNotificationsAsync(int daysToKeep = 30)
        {
            try
            {
                var cutoffDate = DateTime.UtcNow.AddDays(-daysToKeep);
                var oldNotifications = await _notificationRepository.GetByDateRangeAsync(
                    DateTime.MinValue, 
                    cutoffDate);

                var archivedCount = 0;
                foreach (var notification in oldNotifications.Where(n => n.IsActive))
                {
                    await _notificationRepository.DeactivateAsync(notification.Id);
                    archivedCount++;
                }
                
                _logger.LogInfo("Archived old notifications", "NotificationsArchived", new { ArchivedCount = archivedCount });
                
                return archivedCount;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error archiving old notifications", ex);
                throw;
            }
        }

        private async Task<NotificationDelivery> CreateDeliveryAsync(
            NotificationMessage notification, 
            string username, 
            DeliveryChannel channel)
        {
            var delivery = new NotificationDelivery
            {
                Id = Guid.NewGuid(),
                NotificationId = notification.Id,
                Username = username,
                DeliveryChannel = (int)channel,
                DeliveryStatus = (int)DeliveryStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                RetryCount = 0
            };

            await _deliveryRepository.CreateAsync(delivery);
            return delivery;
        }


        private async Task<IEnumerable<UserNotificationViewModel>> BuildUserNotificationViewModelsAsync(
            IEnumerable<NotificationDelivery> deliveries)
        {
            var viewModels = new List<UserNotificationViewModel>();

            foreach (var delivery in deliveries)
            {
                if (delivery.NotificationMessage != null)
                {
                    var viewModel = await BuildUserNotificationViewModelAsync(
                        delivery.NotificationMessage, 
                        delivery.Id);
                    viewModels.Add(viewModel);
                }
            }

            return viewModels;
        }

        private async Task<UserNotificationViewModel> BuildUserNotificationViewModelAsync(
            NotificationMessage notification, 
            Guid deliveryId)
        {
            var delivery = await _deliveryRepository.GetByIdAsync(deliveryId);
            var appProfile = await _appProfileService.GetApplicationProfileAsync(notification.SourceApplication);

            return new UserNotificationViewModel
            {
                NotificationId = notification.Id,
                MessageId = notification.MessageId,
                SourceApplication = notification.SourceApplication,
                ApplicationDisplayName = appProfile?.DisplayName ?? notification.SourceApplication,
                ApplicationIconUrl = appProfile?.IconUrl,
                Title = notification.Title,
                Content = notification.Content,
                Severity = (NotificationSeverity)notification.Severity,
                AlertType = (AlertType)notification.AlertType,
                RequiresConfirmation = notification.RequiresConfirmation,
                CreatedAt = notification.CreatedAt,
                CreatedBy = notification.CreatedBy,
                DeliveryChannel = (DeliveryChannel)(delivery?.DeliveryChannel ?? (int)DeliveryChannel.SignalR),
                DeliveryStatus = (DeliveryStatus)(delivery?.DeliveryStatus ?? (int)DeliveryStatus.Pending),
                DeliveredAt = delivery?.DeliveredAt,
                ConfirmedAt = delivery?.ConfirmedAt,
                Metadata = notification.Metadata
            };
        }
    }
}