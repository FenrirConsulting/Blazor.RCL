using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Blazor.RCL.Application.Interfaces;
using Blazor.RCL.Infrastructure.Services.Interfaces;
using Blazor.RCL.NLog.LogService.Interface;

namespace Blazor.RCL.Infrastructure.Hubs
{
    /// <summary>
    /// SignalR hub for real-time notification delivery with support for Redis and polling modes
    /// </summary>
    [Authorize]
    public class NotificationHub : Hub
    {
        private readonly INotificationService _notificationService;
        private readonly IUserNotificationSettingsService _userSettingsService;
        private readonly IUserSettingsService _userSettingsService2;
        private readonly INotificationPublisher _publisher;
        private readonly ILogHelper _logger;

        public NotificationHub(
            INotificationService notificationService,
            IUserNotificationSettingsService userSettingsService,
            IUserSettingsService userSettingsService2,
            INotificationPublisher publisher,
            ILogHelper logger)
        {
            _notificationService = notificationService;
            _userSettingsService = userSettingsService;
            _userSettingsService2 = userSettingsService2;
            _publisher = publisher;
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            var username = Context.User?.Identity?.Name;
            if (!string.IsNullOrEmpty(username))
            {
                // Add user to their personal group
                await Groups.AddToGroupAsync(Context.ConnectionId, $"user-{username}");
                
                // Add user to application groups they're subscribed to
                var userAppSettings = await _userSettingsService.GetUserApplicationSettingsAsync(username);
                foreach (var appSetting in userAppSettings)
                {
                    if (appSetting.IsSubscribed && appSetting.EnableRealTimeNotifications)
                    {
                        await Groups.AddToGroupAsync(Context.ConnectionId, $"app-{appSetting.ApplicationName}");
                    }
                }
                
                // Add user to role-based groups
                var userRoles = await _userSettingsService2.GetStoredUserRolesAsync(username);
                if (userRoles != null && userRoles.Any())
                {
                    foreach (var role in userRoles)
                    {
                        await Groups.AddToGroupAsync(Context.ConnectionId, $"role-{role}");
                    }
                    
                    _logger.LogInfo($"User {username} added to {userRoles.Count()} role groups", 
                        "UserRoleGroupsJoined", 
                        new { Username = username, Roles = userRoles });
                }
                
                // Inform client about connection mode (Redis or Polling)
                var publisherStatus = await _publisher.GetStatusAsync();
                await Clients.Caller.SendAsync("ConnectionMode", new
                {
                    Mode = publisherStatus.Mode.ToString().ToLower(),
                    IsRealTimeAvailable = _publisher.IsRealTimeAvailable,
                    PollingInterval = _publisher.IsRealTimeAvailable ? 0 : GetPollingInterval(),
                    ServerTime = DateTime.UtcNow
                });
                
                // Send any pending notifications
                var pendingNotifications = await _notificationService.GetUserNotificationsAsync(username, false);
                foreach (var notification in pendingNotifications)
                {
                    await Clients.Caller.SendAsync("ReceiveNotification", notification);
                }
                
                _logger.LogInfo($"User {username} connected to NotificationHub. Mode: {publisherStatus.Mode}", 
                    "NotificationHubConnected", 
                    new { Username = username, Mode = publisherStatus.Mode.ToString() });
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var username = Context.User?.Identity?.Name;
            if (!string.IsNullOrEmpty(username))
            {
                _logger.LogInfo($"User {username} disconnected from NotificationHub", 
                    "NotificationHubDisconnected", 
                    new { Username = username, Exception = exception?.Message });
            }

            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// Confirms receipt of a notification
        /// </summary>
        public async Task ConfirmNotification(Guid notificationId)
        {
            var username = Context.User?.Identity?.Name;
            if (string.IsNullOrEmpty(username))
            {
                throw new HubException("User not authenticated");
            }

            var confirmed = await _notificationService.ConfirmNotificationAsync(notificationId, username);
            if (confirmed)
            {
                await Clients.Caller.SendAsync("NotificationConfirmed", notificationId);
            }
        }

        /// <summary>
        /// Gets the count of unconfirmed notifications
        /// </summary>
        public async Task<int> GetUnconfirmedCount()
        {
            var username = Context.User?.Identity?.Name;
            if (string.IsNullOrEmpty(username))
            {
                throw new HubException("User not authenticated");
            }

            return await _notificationService.GetUnconfirmedNotificationCountAsync(username);
        }

        /// <summary>
        /// Subscribes to notifications from a specific application
        /// </summary>
        public async Task SubscribeToApplication(string applicationName)
        {
            var username = Context.User?.Identity?.Name;
            if (string.IsNullOrEmpty(username))
            {
                throw new HubException("User not authenticated");
            }

            await _userSettingsService.SubscribeToApplicationAsync(username, applicationName);
            await Groups.AddToGroupAsync(Context.ConnectionId, $"app-{applicationName}");
        }

        /// <summary>
        /// Unsubscribes from notifications from a specific application
        /// </summary>
        public async Task UnsubscribeFromApplication(string applicationName)
        {
            var username = Context.User?.Identity?.Name;
            if (string.IsNullOrEmpty(username))
            {
                throw new HubException("User not authenticated");
            }

            await _userSettingsService.UnsubscribeFromApplicationAsync(username, applicationName);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"app-{applicationName}");
        }

        /// <summary>
        /// Gets the current connection status and mode information
        /// </summary>
        public async Task<object> GetConnectionStatus()
        {
            var status = await _publisher.GetStatusAsync();
            return new
            {
                Mode = status.Mode.ToString().ToLower(),
                IsRealTimeAvailable = _publisher.IsRealTimeAvailable,
                IsConnected = status.IsConnected,
                PollingInterval = _publisher.IsRealTimeAvailable ? 0 : GetPollingInterval(),
                LastError = status.LastError,
                Diagnostics = status.Diagnostics
            };
        }

        /// <summary>
        /// Gets the polling interval based on the current environment
        /// </summary>
        private int GetPollingInterval()
        {
            // These values should ideally come from configuration
            // Using defaults based on environment name
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
            
            return environment.ToLower() switch
            {
                "development" => 5000,   // 5 seconds for development
                "dev" => 5000,          // 5 seconds for DEV
                "qa" => 15000,          // 15 seconds for QA
                "production" => 30000,  // 30 seconds for production
                "prod" => 30000,        // 30 seconds for PROD
                _ => 30000              // Default to 30 seconds
            };
        }
    }
}