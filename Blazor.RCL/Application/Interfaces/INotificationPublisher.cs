using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazor.RCL.Domain.Entities.Notifications;

namespace Blazor.RCL.Application.Interfaces
{
    /// <summary>
    /// Abstraction for publishing notifications across different backplane implementations.
    /// Supports Redis-based real-time delivery and local polling-based fallback.
    /// </summary>
    public interface INotificationPublisher
    {
        /// <summary>
        /// Gets a value indicating whether real-time notification delivery is available.
        /// Returns true when Redis is connected, false when in polling mode.
        /// </summary>
        bool IsRealTimeAvailable { get; }

        /// <summary>
        /// Publishes a notification to specific target users.
        /// In Redis mode: Publishes to Redis for cross-server delivery.
        /// In Polling mode: No-op as clients will poll the database.
        /// </summary>
        /// <param name="notification">The notification message to publish</param>
        /// <param name="targetUsers">Collection of usernames to receive the notification</param>
        /// <returns>Task representing the asynchronous operation</returns>
        Task PublishAsync(NotificationMessage notification, IEnumerable<string> targetUsers);

        /// <summary>
        /// Publishes a notification to all users subscribed to a specific application.
        /// In Redis mode: Publishes to Redis for cross-server delivery.
        /// In Polling mode: No-op as clients will poll the database.
        /// </summary>
        /// <param name="notification">The notification message to publish</param>
        /// <param name="applicationName">The application name for targeting subscribers</param>
        /// <returns>Task representing the asynchronous operation</returns>
        Task PublishToApplicationAsync(NotificationMessage notification, string applicationName);

        /// <summary>
        /// Publishes a notification to all users with a specific role.
        /// In Redis mode: Publishes to Redis for cross-server delivery.
        /// In Polling mode: No-op as clients will poll the database.
        /// </summary>
        /// <param name="notification">The notification message to publish</param>
        /// <param name="role">The role name for targeting users</param>
        /// <returns>Task representing the asynchronous operation</returns>
        Task PublishToRoleAsync(NotificationMessage notification, string role);

        /// <summary>
        /// Gets the current connection status and mode information.
        /// Useful for diagnostics and client configuration.
        /// </summary>
        /// <returns>Connection status information</returns>
        Task<NotificationPublisherStatus> GetStatusAsync();
    }

    /// <summary>
    /// Represents the status of the notification publisher
    /// </summary>
    public class NotificationPublisherStatus
    {
        /// <summary>
        /// Gets or sets the publisher mode (Redis or Polling)
        /// </summary>
        public PublisherMode Mode { get; set; }

        /// <summary>
        /// Gets or sets whether the publisher is connected and operational
        /// </summary>
        public bool IsConnected { get; set; }

        /// <summary>
        /// Gets or sets the last connection error message, if any
        /// </summary>
        public string? LastError { get; set; }

        /// <summary>
        /// Gets or sets the timestamp of the last successful operation
        /// </summary>
        public DateTime? LastSuccessfulOperation { get; set; }

        /// <summary>
        /// Gets or sets additional diagnostic information
        /// </summary>
        public Dictionary<string, object>? Diagnostics { get; set; }
    }

    /// <summary>
    /// Defines the available publisher modes
    /// </summary>
    public enum PublisherMode
    {
        /// <summary>
        /// Redis-based real-time publishing
        /// </summary>
        Redis,

        /// <summary>
        /// Local polling-based fallback mode
        /// </summary>
        Polling
    }
}