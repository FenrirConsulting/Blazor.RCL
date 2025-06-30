using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazor.RCL.Domain.Entities.Notifications;
using Blazor.RCL.Application.Models.Notifications;

namespace Blazor.RCL.Application.Interfaces
{
    /// <summary>
    /// Service for managing email notification delivery
    /// </summary>
    public interface IEmailNotificationService
    {
        /// <summary>
        /// Queues an email notification for delivery
        /// </summary>
        /// <param name="notification">The notification to send</param>
        /// <param name="username">Target username</param>
        /// <param name="priority">Email priority</param>
        /// <returns>Created email queue entry</returns>
        Task<EmailNotificationQueue> QueueEmailNotificationAsync(
            NotificationMessage notification, 
            string username,
            EmailPriority priority = EmailPriority.Normal);

        /// <summary>
        /// Queues email notifications for multiple users
        /// </summary>
        /// <param name="notification">The notification to send</param>
        /// <param name="usernames">Target usernames</param>
        /// <param name="priority">Email priority</param>
        /// <returns>Collection of created email queue entries</returns>
        Task<IEnumerable<EmailNotificationQueue>> QueueEmailNotificationBatchAsync(
            NotificationMessage notification,
            IEnumerable<string> usernames,
            EmailPriority priority = EmailPriority.Normal);

        /// <summary>
        /// Processes pending emails in the queue
        /// </summary>
        /// <param name="batchSize">Number of emails to process</param>
        /// <returns>Number of emails processed</returns>
        Task<int> ProcessEmailQueueAsync(int batchSize = 50);

        /// <summary>
        /// Sends an email immediately (bypasses queue)
        /// </summary>
        /// <param name="request">Email send request</param>
        /// <returns>True if sent successfully</returns>
        Task<bool> SendEmailImmediatelyAsync(SendEmailRequest request);

        /// <summary>
        /// Gets pending email statistics
        /// </summary>
        /// <returns>Email queue statistics</returns>
        Task<EmailQueueStatistics> GetEmailQueueStatisticsAsync();

        /// <summary>
        /// Retries failed emails
        /// </summary>
        /// <param name="maxRetries">Maximum retry attempts</param>
        /// <param name="hoursOld">Only retry emails older than this many hours</param>
        /// <returns>Number of emails retried</returns>
        Task<int> RetryFailedEmailsAsync(int maxRetries = 3, int hoursOld = 1);

        /// <summary>
        /// Cleans up old processed emails from the queue
        /// </summary>
        /// <param name="daysToKeep">Number of days to keep processed emails</param>
        /// <returns>Number of emails cleaned up</returns>
        Task<int> CleanupOldEmailsAsync(int daysToKeep = 7);

        /// <summary>
        /// Gets email delivery history for a user
        /// </summary>
        /// <param name="username">The username</param>
        /// <param name="days">Number of days of history</param>
        /// <returns>Collection of email queue entries</returns>
        Task<IEnumerable<EmailNotificationQueue>> GetUserEmailHistoryAsync(string username, int days = 7);

        /// <summary>
        /// Validates email configuration and connectivity
        /// </summary>
        /// <returns>Validation result with any issues found</returns>
        Task<EmailConfigurationValidation> ValidateEmailConfigurationAsync();

        /// <summary>
        /// Processes scheduled digest emails for users with hourly/daily frequency
        /// </summary>
        /// <returns>Number of digest emails sent</returns>
        Task<int> ProcessScheduledDigestsAsync();
    }
}