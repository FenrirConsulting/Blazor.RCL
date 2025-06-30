using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Blazor.RCL.Domain.Entities.Notifications;

namespace Blazor.RCL.Application.Interfaces
{
    /// <summary>
    /// Interface for operations on EmailNotificationQueue entities.
    /// </summary>
    public interface IEmailNotificationQueueRepository
    {
        #region Query Methods

        /// <summary>
        /// Gets an email queue entry by ID.
        /// </summary>
        /// <param name="id">The queue entry ID.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The email queue entry if found; otherwise, null.</returns>
        Task<EmailNotificationQueue> GetByIdAsync(Guid id, CancellationToken token = default);

        /// <summary>
        /// Gets email queue entries for a notification.
        /// </summary>
        /// <param name="notificationId">The notification ID.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A list of email queue entries for the notification.</returns>
        Task<IEnumerable<EmailNotificationQueue>> GetByNotificationIdAsync(Guid notificationId, CancellationToken token = default);

        /// <summary>
        /// Gets email queue entries for a user.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A list of email queue entries for the user.</returns>
        Task<IEnumerable<EmailNotificationQueue>> GetByUsernameAsync(string username, CancellationToken token = default);

        /// <summary>
        /// Gets pending emails ready to be sent.
        /// </summary>
        /// <param name="batchSize">Maximum number of emails to retrieve.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A list of pending emails ready for sending.</returns>
        Task<IEnumerable<EmailNotificationQueue>> GetPendingEmailsAsync(int batchSize, CancellationToken token = default);

        /// <summary>
        /// Gets emails by status.
        /// </summary>
        /// <param name="status">The email status.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A list of emails with the specified status.</returns>
        Task<IEnumerable<EmailNotificationQueue>> GetByStatusAsync(int status, CancellationToken token = default);

        /// <summary>
        /// Gets high priority pending emails.
        /// </summary>
        /// <param name="batchSize">Maximum number of emails to retrieve.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A list of high priority pending emails.</returns>
        Task<IEnumerable<EmailNotificationQueue>> GetHighPriorityPendingAsync(int batchSize, CancellationToken token = default);

        /// <summary>
        /// Gets failed emails for retry.
        /// </summary>
        /// <param name="maxRetryCount">Maximum retry count to consider.</param>
        /// <param name="batchSize">Maximum number of emails to retrieve.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A list of failed emails eligible for retry.</returns>
        Task<IEnumerable<EmailNotificationQueue>> GetFailedForRetryAsync(int maxRetryCount, int batchSize, CancellationToken token = default);

        /// <summary>
        /// Gets email statistics for a time period.
        /// </summary>
        /// <param name="startDate">Start date of the period.</param>
        /// <param name="endDate">End date of the period.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Email statistics including sent, failed, and pending counts.</returns>
        Task<(int sent, int failed, int pending)> GetStatisticsAsync(DateTime startDate, DateTime endDate, CancellationToken token = default);

        /// <summary>
        /// Gets failed emails.
        /// </summary>
        /// <param name="maxRetries">Maximum retry count.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A list of failed emails.</returns>
        Task<IEnumerable<EmailNotificationQueue>> GetFailedEmailsAsync(int maxRetries, CancellationToken token = default);

        /// <summary>
        /// Gets user emails since a specific date.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="sinceDate">The date to get emails from.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A list of user emails since the specified date.</returns>
        Task<IEnumerable<EmailNotificationQueue>> GetUserEmailsAsync(string username, DateTime sinceDate, CancellationToken token = default);

        /// <summary>
        /// Gets queue statistics.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>Queue statistics including pending, processing, sent, and failed counts.</returns>
        Task<Dictionary<string, int>> GetQueueStatisticsAsync(CancellationToken token = default);

        /// <summary>
        /// Atomically claims pending emails for processing by a specific instance.
        /// This method prevents duplicate processing in multi-server environments by using
        /// an atomic UPDATE with OUTPUT clause to claim emails in a single operation.
        /// Also recovers stuck emails that have been processing for more than 5 minutes.
        /// </summary>
        /// <param name="instanceId">The unique identifier of the processing instance (e.g., "SERVER01_12345").</param>
        /// <param name="batchSize">The maximum number of emails to claim.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A list of emails that were successfully claimed by this instance.</returns>
        Task<List<EmailNotificationQueue>> ClaimPendingEmailsAsync(string instanceId, int batchSize, CancellationToken cancellationToken = default);

        #endregion

        #region Create Methods

        /// <summary>
        /// Creates a new email queue entry.
        /// </summary>
        /// <param name="emailQueue">The email queue entry to create.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The created email queue entry with assigned ID.</returns>
        Task<EmailNotificationQueue> CreateAsync(EmailNotificationQueue emailQueue, CancellationToken token = default);

        /// <summary>
        /// Creates multiple email queue entries.
        /// </summary>
        /// <param name="emailQueues">The email queue entries to create.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The created email queue entries.</returns>
        Task<IEnumerable<EmailNotificationQueue>> CreateBatchAsync(IEnumerable<EmailNotificationQueue> emailQueues, CancellationToken token = default);

        #endregion

        #region Update Methods

        /// <summary>
        /// Updates an email queue entry.
        /// </summary>
        /// <param name="emailQueue">The email queue entry to update.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The updated email queue entry.</returns>
        Task<EmailNotificationQueue> UpdateAsync(EmailNotificationQueue emailQueue, CancellationToken token = default);

        /// <summary>
        /// Marks an email as processing.
        /// </summary>
        /// <param name="id">The email queue entry ID.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>True if successful; otherwise, false.</returns>
        Task<bool> MarkAsProcessingAsync(Guid id, CancellationToken token = default);

        /// <summary>
        /// Marks an email as sent.
        /// </summary>
        /// <param name="id">The email queue entry ID.</param>
        /// <param name="sentTime">The time the email was sent.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>True if successful; otherwise, false.</returns>
        Task<bool> MarkAsSentAsync(Guid id, DateTime sentTime, CancellationToken token = default);

        /// <summary>
        /// Marks an email as failed.
        /// </summary>
        /// <param name="id">The email queue entry ID.</param>
        /// <param name="failureReason">The reason for failure.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>True if successful; otherwise, false.</returns>
        Task<bool> MarkAsFailedAsync(Guid id, string failureReason, CancellationToken token = default);

        /// <summary>
        /// Increments the retry count for an email.
        /// </summary>
        /// <param name="id">The email queue entry ID.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The new retry count.</returns>
        Task<int> IncrementRetryCountAsync(Guid id, CancellationToken token = default);

        /// <summary>
        /// Updates the scheduled send time for an email.
        /// </summary>
        /// <param name="id">The email queue entry ID.</param>
        /// <param name="newScheduledTime">The new scheduled send time.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>True if successful; otherwise, false.</returns>
        Task<bool> RescheduleAsync(Guid id, DateTime newScheduledTime, CancellationToken token = default);

        /// <summary>
        /// Updates the status of an email notification.
        /// </summary>
        /// <param name="id">The email queue entry ID.</param>
        /// <param name="status">The new email status.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>True if successful; otherwise, false.</returns>
        Task<bool> UpdateStatusAsync(Guid id, EmailStatus status, CancellationToken token = default);

        #endregion

        #region Delete Methods

        /// <summary>
        /// Deletes old sent emails.
        /// </summary>
        /// <param name="olderThan">The cutoff date.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The number of deleted emails.</returns>
        Task<int> DeleteSentEmailsAsync(DateTime olderThan, CancellationToken token = default);

        /// <summary>
        /// Deletes a specific email from the queue.
        /// </summary>
        /// <param name="id">The email queue entry ID.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>True if successful; otherwise, false.</returns>
        Task<bool> DeleteAsync(Guid id, CancellationToken token = default);

        /// <summary>
        /// Deletes old emails from the queue.
        /// </summary>
        /// <param name="cutoffDate">The cutoff date.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The number of deleted emails.</returns>
        Task<int> DeleteOldEmailsAsync(DateTime cutoffDate, CancellationToken token = default);

        #endregion
    }
}