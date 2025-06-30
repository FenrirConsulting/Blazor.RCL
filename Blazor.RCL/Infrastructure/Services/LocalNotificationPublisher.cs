using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazor.RCL.Application.Interfaces;
using Blazor.RCL.Domain.Entities.Notifications;
using Blazor.RCL.NLog.LogService.Interface;

namespace Blazor.RCL.Infrastructure.Services
{
    /// <summary>
    /// Local notification publisher for polling-based fallback mode.
    /// This implementation is used when Redis is unavailable.
    /// Notifications are stored in the database and clients poll for updates.
    /// </summary>
    public class LocalNotificationPublisher : INotificationPublisher
    {
        private readonly ILogHelper _logger;
        private DateTime? _lastOperation;

        public LocalNotificationPublisher(ILogHelper logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc/>
        public bool IsRealTimeAvailable => false;

        /// <inheritdoc/>
        public Task PublishAsync(NotificationMessage notification, IEnumerable<string> targetUsers)
        {
            // In polling mode, we don't need to actively publish.
            // Notifications are already saved to the database by NotificationService.
            // Clients will poll the database to retrieve new notifications.
            
            _lastOperation = DateTime.UtcNow;
            _logger.LogDebug($"Notification {notification.MessageId} stored for polling delivery to users", 
                "PollingNotificationStored", 
                new { NotificationId = notification.Id, UserCount = targetUsers.Count() });

            // Return completed task as no additional action is required
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task PublishToApplicationAsync(NotificationMessage notification, string applicationName)
        {
            // In polling mode, we don't need to actively publish.
            // Notifications are already saved to the database by NotificationService.
            // Clients will poll the database to retrieve new notifications.
            
            _lastOperation = DateTime.UtcNow;
            _logger.LogDebug($"Notification {notification.MessageId} stored for polling delivery to application {applicationName}", 
                "PollingApplicationNotificationStored", 
                new { NotificationId = notification.Id, ApplicationName = applicationName });

            // Return completed task as no additional action is required
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task PublishToRoleAsync(NotificationMessage notification, string role)
        {
            // In polling mode, we don't need to actively publish.
            // Notifications are already saved to the database by NotificationService.
            // Clients will poll the database to retrieve new notifications.
            
            _lastOperation = DateTime.UtcNow;
            _logger.LogDebug($"Notification {notification.MessageId} stored for polling delivery to role {role}", 
                "PollingRoleNotificationStored", 
                new { NotificationId = notification.Id, Role = role });

            // Return completed task as no additional action is required
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public Task<NotificationPublisherStatus> GetStatusAsync()
        {
            var status = new NotificationPublisherStatus
            {
                Mode = PublisherMode.Polling,
                IsConnected = true, // Always "connected" in polling mode
                LastError = null,
                LastSuccessfulOperation = _lastOperation,
                Diagnostics = new Dictionary<string, object>
                {
                    ["Mode"] = "Polling",
                    ["Description"] = "Operating in polling mode. Clients poll database for new notifications.",
                    ["PollingInfo"] = "Clients should poll /api/notifications endpoint based on configured intervals.",
                    ["Reason"] = "Redis not configured or unavailable"
                }
            };

            return Task.FromResult(status);
        }
    }
}