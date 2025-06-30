using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Blazor.RCL.Domain.Entities.Notifications;

namespace Blazor.RCL.Application.Interfaces
{
    /// <summary>
    /// Interface for operations on NotificationMessage entities.
    /// </summary>
    public interface INotificationMessageRepository
    {
        #region Query Methods

        /// <summary>
        /// Gets a notification message by its ID.
        /// </summary>
        /// <param name="id">The notification ID.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The notification message if found; otherwise, null.</returns>
        Task<NotificationMessage> GetByIdAsync(Guid id, CancellationToken token = default);

        /// <summary>
        /// Gets a notification message by its message ID.
        /// </summary>
        /// <param name="messageId">The unique message identifier.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The notification message if found; otherwise, null.</returns>
        Task<NotificationMessage> GetByMessageIdAsync(string messageId, CancellationToken token = default);

        /// <summary>
        /// Gets all active notifications for a specific application.
        /// </summary>
        /// <param name="applicationName">The source application name.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A list of active notifications.</returns>
        Task<IEnumerable<NotificationMessage>> GetActiveByApplicationAsync(string applicationName, CancellationToken token = default);

        /// <summary>
        /// Gets notifications by severity level.
        /// </summary>
        /// <param name="severity">The minimum severity level.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A list of notifications matching the severity criteria.</returns>
        Task<IEnumerable<NotificationMessage>> GetBySeverityAsync(int severity, CancellationToken token = default);

        /// <summary>
        /// Gets notifications by alert type.
        /// </summary>
        /// <param name="alertType">The alert type.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A list of notifications matching the alert type.</returns>
        Task<IEnumerable<NotificationMessage>> GetByAlertTypeAsync(int alertType, CancellationToken token = default);

        /// <summary>
        /// Gets notifications created within a date range.
        /// </summary>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A list of notifications within the date range.</returns>
        Task<IEnumerable<NotificationMessage>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken token = default);

        /// <summary>
        /// Gets notifications that require confirmation.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A list of notifications requiring confirmation.</returns>
        Task<IEnumerable<NotificationMessage>> GetRequiringConfirmationAsync(CancellationToken token = default);

        /// <summary>
        /// Gets expired notifications for cleanup.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A list of expired notifications.</returns>
        Task<IEnumerable<NotificationMessage>> GetExpiredAsync(CancellationToken token = default);

        #endregion

        #region Create Methods

        /// <summary>
        /// Creates a new notification message.
        /// </summary>
        /// <param name="notification">The notification to create.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The created notification with its assigned ID.</returns>
        Task<NotificationMessage> CreateAsync(NotificationMessage notification, CancellationToken token = default);

        /// <summary>
        /// Creates multiple notification messages in a batch.
        /// </summary>
        /// <param name="notifications">The notifications to create.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The created notifications.</returns>
        Task<IEnumerable<NotificationMessage>> CreateBatchAsync(IEnumerable<NotificationMessage> notifications, CancellationToken token = default);

        #endregion

        #region Update Methods

        /// <summary>
        /// Updates an existing notification message.
        /// </summary>
        /// <param name="notification">The notification to update.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The updated notification.</returns>
        Task<NotificationMessage> UpdateAsync(NotificationMessage notification, CancellationToken token = default);

        /// <summary>
        /// Marks a notification as inactive.
        /// </summary>
        /// <param name="id">The notification ID.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>True if successful; otherwise, false.</returns>
        Task<bool> DeactivateAsync(Guid id, CancellationToken token = default);

        #endregion

        #region Delete Methods

        /// <summary>
        /// Deletes expired notifications older than the specified date.
        /// </summary>
        /// <param name="olderThan">The cutoff date.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The number of deleted notifications.</returns>
        Task<int> DeleteExpiredAsync(DateTime olderThan, CancellationToken token = default);

        #endregion
    }
}