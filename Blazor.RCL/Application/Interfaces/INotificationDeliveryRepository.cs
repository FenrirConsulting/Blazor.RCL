using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Blazor.RCL.Domain.Entities.Notifications;

namespace Blazor.RCL.Application.Interfaces
{
    /// <summary>
    /// Interface for operations on NotificationDelivery entities.
    /// </summary>
    public interface INotificationDeliveryRepository
    {
        #region Query Methods

        /// <summary>
        /// Gets a notification delivery record by ID.
        /// </summary>
        /// <param name="id">The delivery record ID.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The delivery record if found; otherwise, null.</returns>
        Task<NotificationDelivery> GetByIdAsync(Guid id, CancellationToken token = default);

        /// <summary>
        /// Gets all delivery records for a notification.
        /// </summary>
        /// <param name="notificationId">The notification ID.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A list of delivery records for the notification.</returns>
        Task<IEnumerable<NotificationDelivery>> GetByNotificationIdAsync(Guid notificationId, CancellationToken token = default);

        /// <summary>
        /// Gets delivery records for a user.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A list of delivery records for the user.</returns>
        Task<IEnumerable<NotificationDelivery>> GetByUsernameAsync(string username, CancellationToken token = default);

        /// <summary>
        /// Gets delivery records by status.
        /// </summary>
        /// <param name="deliveryStatus">The delivery status.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A list of delivery records with the specified status.</returns>
        Task<IEnumerable<NotificationDelivery>> GetByStatusAsync(int deliveryStatus, CancellationToken token = default);

        /// <summary>
        /// Gets pending deliveries for a channel.
        /// </summary>
        /// <param name="deliveryChannel">The delivery channel.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A list of pending deliveries for the channel.</returns>
        Task<IEnumerable<NotificationDelivery>> GetPendingByChannelAsync(int deliveryChannel, CancellationToken token = default);

        /// <summary>
        /// Gets failed deliveries for retry.
        /// </summary>
        /// <param name="maxRetryCount">Maximum retry count to consider.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A list of failed deliveries eligible for retry.</returns>
        Task<IEnumerable<NotificationDelivery>> GetFailedForRetryAsync(int maxRetryCount, CancellationToken token = default);

        /// <summary>
        /// Gets unconfirmed deliveries older than specified time.
        /// </summary>
        /// <param name="olderThan">The cutoff date.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A list of unconfirmed deliveries.</returns>
        Task<IEnumerable<NotificationDelivery>> GetUnconfirmedAsync(DateTime olderThan, CancellationToken token = default);

        /// <summary>
        /// Gets delivery records for a specific notification and user.
        /// </summary>
        /// <param name="notificationId">The notification ID.</param>
        /// <param name="username">The username.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A list of delivery records for the notification and user.</returns>
        Task<IEnumerable<NotificationDelivery>> GetByNotificationAndUserAsync(Guid notificationId, string username, CancellationToken token = default);

        /// <summary>
        /// Gets user deliveries with optional filtering for confirmed notifications.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="includeConfirmed">Whether to include confirmed deliveries.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A list of delivery records for the user.</returns>
        Task<IEnumerable<NotificationDelivery>> GetUserDeliveriesAsync(string username, bool includeConfirmed, CancellationToken token = default);

        /// <summary>
        /// Gets unconfirmed deliveries for a user.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A list of unconfirmed delivery records for the user.</returns>
        Task<IEnumerable<NotificationDelivery>> GetUnconfirmedDeliveriesAsync(string username, CancellationToken token = default);

        /// <summary>
        /// Gets failed deliveries for a specific notification.
        /// </summary>
        /// <param name="notificationId">The notification ID.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A list of failed delivery records for the notification.</returns>
        Task<IEnumerable<NotificationDelivery>> GetFailedDeliveriesByNotificationAsync(Guid notificationId, CancellationToken token = default);

        /// <summary>
        /// Checks if a notification was delivered to a user.
        /// </summary>
        /// <param name="notificationId">The notification ID.</param>
        /// <param name="username">The username.</param>
        /// <param name="deliveryChannel">The delivery channel.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>True if delivered; otherwise, false.</returns>
        Task<bool> IsDeliveredAsync(Guid notificationId, string username, int deliveryChannel, CancellationToken token = default);

        #endregion

        #region Create Methods

        /// <summary>
        /// Creates a new notification delivery record.
        /// </summary>
        /// <param name="delivery">The delivery record to create.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The created delivery record with assigned ID.</returns>
        Task<NotificationDelivery> CreateAsync(NotificationDelivery delivery, CancellationToken token = default);

        /// <summary>
        /// Creates multiple delivery records for a notification.
        /// </summary>
        /// <param name="deliveries">The delivery records to create.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The created delivery records.</returns>
        Task<IEnumerable<NotificationDelivery>> CreateBatchAsync(IEnumerable<NotificationDelivery> deliveries, CancellationToken token = default);

        #endregion

        #region Update Methods

        /// <summary>
        /// Updates a notification delivery record.
        /// </summary>
        /// <param name="delivery">The delivery record to update.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The updated delivery record.</returns>
        Task<NotificationDelivery> UpdateAsync(NotificationDelivery delivery, CancellationToken token = default);

        /// <summary>
        /// Marks a delivery as delivered.
        /// </summary>
        /// <param name="id">The delivery record ID.</param>
        /// <param name="deliveredAt">The delivery timestamp.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>True if successful; otherwise, false.</returns>
        Task<bool> MarkAsDeliveredAsync(Guid id, DateTime deliveredAt, CancellationToken token = default);

        /// <summary>
        /// Marks a delivery as failed.
        /// </summary>
        /// <param name="id">The delivery record ID.</param>
        /// <param name="failureReason">The reason for failure.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>True if successful; otherwise, false.</returns>
        Task<bool> MarkAsFailedAsync(Guid id, string failureReason, CancellationToken token = default);

        /// <summary>
        /// Marks a delivery as confirmed by user.
        /// </summary>
        /// <param name="id">The delivery record ID.</param>
        /// <param name="confirmedAt">The confirmation timestamp.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>True if successful; otherwise, false.</returns>
        Task<bool> MarkAsConfirmedAsync(Guid id, DateTime confirmedAt, CancellationToken token = default);

        /// <summary>
        /// Increments the retry count for a delivery.
        /// </summary>
        /// <param name="id">The delivery record ID.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The new retry count.</returns>
        Task<int> IncrementRetryCountAsync(Guid id, CancellationToken token = default);

        /// <summary>
        /// Updates the delivery status of a notification delivery.
        /// </summary>
        /// <param name="id">The delivery record ID.</param>
        /// <param name="status">The new delivery status.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>True if successful; otherwise, false.</returns>
        Task<bool> UpdateDeliveryStatusAsync(Guid id, DeliveryStatus status, CancellationToken token = default);

        #endregion

        #region Delete Methods

        /// <summary>
        /// Deletes old delivery records.
        /// </summary>
        /// <param name="olderThan">The cutoff date.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The number of deleted records.</returns>
        Task<int> DeleteOldRecordsAsync(DateTime olderThan, CancellationToken token = default);

        #endregion
    }
}