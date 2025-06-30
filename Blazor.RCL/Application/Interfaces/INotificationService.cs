using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazor.RCL.Domain.Entities.Notifications;
using Blazor.RCL.Application.Models.Notifications;

namespace Blazor.RCL.Application.Interfaces
{
    /// <summary>
    /// Core notification service for creating and managing notifications across all Automation applications
    /// </summary>
    public interface INotificationService
    {
        /// <summary>
        /// Creates a new notification message and queues it for delivery
        /// </summary>
        /// <param name="request">The notification creation request</param>
        /// <returns>The created notification message with delivery tracking information</returns>
        Task<NotificationMessage> CreateNotificationAsync(CreateNotificationRequest request);

        /// <summary>
        /// Creates multiple notification messages in a batch
        /// </summary>
        /// <param name="requests">Collection of notification creation requests</param>
        /// <returns>Collection of created notification messages</returns>
        Task<IEnumerable<NotificationMessage>> CreateNotificationBatchAsync(IEnumerable<CreateNotificationRequest> requests);

        /// <summary>
        /// Sends a notification to specific users
        /// </summary>
        /// <param name="notificationId">The notification ID to send</param>
        /// <param name="usernames">Target usernames</param>
        /// <param name="channels">Delivery channels to use (defaults to user preferences)</param>
        /// <returns>Delivery tracking information for each user</returns>
        Task<IEnumerable<NotificationDelivery>> SendNotificationAsync(
            Guid notificationId, 
            IEnumerable<string> usernames, 
            DeliveryChannel? channels = null);

        /// <summary>
        /// Sends a notification to all subscribed users of an application
        /// </summary>
        /// <param name="notificationId">The notification ID to send</param>
        /// <param name="applicationName">The application name</param>
        /// <param name="channels">Delivery channels to use (defaults to user preferences)</param>
        /// <returns>Delivery tracking information for each user</returns>
        Task<IEnumerable<NotificationDelivery>> SendNotificationToApplicationUsersAsync(
            Guid notificationId, 
            string applicationName,
            DeliveryChannel? channels = null);

        /// <summary>
        /// Sends a notification to all users with a specific role
        /// </summary>
        /// <param name="notificationId">The notification ID to send</param>
        /// <param name="role">The role name</param>
        /// <param name="channels">Delivery channels to use (defaults to user preferences)</param>
        /// <returns>Delivery tracking information for each user</returns>
        Task<IEnumerable<NotificationDelivery>> SendNotificationToRoleAsync(
            Guid notificationId,
            string role,
            DeliveryChannel? channels = null);

        /// <summary>
        /// Marks a notification as confirmed/acknowledged by a user
        /// </summary>
        /// <param name="notificationId">The notification ID</param>
        /// <param name="username">The username confirming the notification</param>
        /// <returns>True if successfully confirmed</returns>
        Task<bool> ConfirmNotificationAsync(Guid notificationId, string username);

        /// <summary>
        /// Gets all active notifications for a user
        /// </summary>
        /// <param name="username">The username</param>
        /// <param name="includeConfirmed">Whether to include confirmed notifications</param>
        /// <returns>Collection of notifications with delivery status</returns>
        Task<IEnumerable<UserNotificationViewModel>> GetUserNotificationsAsync(
            string username, 
            bool includeConfirmed = false);

        /// <summary>
        /// Gets notifications for a user from a specific application
        /// </summary>
        /// <param name="username">The username</param>
        /// <param name="applicationName">The application name</param>
        /// <param name="includeConfirmed">Whether to include confirmed notifications</param>
        /// <returns>Collection of notifications with delivery status</returns>
        Task<IEnumerable<UserNotificationViewModel>> GetUserApplicationNotificationsAsync(
            string username, 
            string applicationName,
            bool includeConfirmed = false);

        /// <summary>
        /// Gets unconfirmed notification count for a user
        /// </summary>
        /// <param name="username">The username</param>
        /// <returns>Count of unconfirmed notifications</returns>
        Task<int> GetUnconfirmedNotificationCountAsync(string username);

        /// <summary>
        /// Retries failed notification deliveries
        /// </summary>
        /// <param name="notificationId">The notification ID to retry</param>
        /// <param name="maxRetries">Maximum retry attempts</param>
        /// <returns>Updated delivery tracking information</returns>
        Task<IEnumerable<NotificationDelivery>> RetryFailedDeliveriesAsync(
            Guid notificationId, 
            int maxRetries = 3);

        /// <summary>
        /// Archives old notifications based on retention policy
        /// </summary>
        /// <param name="daysToKeep">Number of days to keep notifications</param>
        /// <returns>Number of notifications archived</returns>
        Task<int> ArchiveOldNotificationsAsync(int daysToKeep = 30);
    }
}