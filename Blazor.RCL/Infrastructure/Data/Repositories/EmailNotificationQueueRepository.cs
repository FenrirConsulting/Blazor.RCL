using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Blazor.RCL.Application.Interfaces;
using Blazor.RCL.Domain.Entities.Notifications;
using Blazor.RCL.NLog.LogService.Interface;
using Microsoft.EntityFrameworkCore;

namespace Blazor.RCL.Infrastructure.Data.Repositories
{
    /// <summary>
    /// Repository implementation for EmailNotificationQueue operations using Entity Framework Core.
    /// </summary>
    public class EmailNotificationQueueRepository : IEmailNotificationQueueRepository
    {
        #region Fields

        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly ILogHelper _logger;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the EmailNotificationQueueRepository class.
        /// </summary>
        /// <param name="contextFactory">The factory for creating database contexts.</param>
        /// <param name="logger">The logger for error logging.</param>
        public EmailNotificationQueueRepository(IDbContextFactory<AppDbContext> contextFactory, ILogHelper logger)
        {
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #endregion

        #region Query Methods

        public async Task<EmailNotificationQueue> GetByIdAsync(Guid id, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                return await context.EmailNotificationQueues
                    .Include(e => e.NotificationMessage)
                    .FirstOrDefaultAsync(e => e.Id == id, token);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting email queue entry by ID: {id}", ex);
                throw;
            }
        }

        public async Task<IEnumerable<EmailNotificationQueue>> GetByNotificationIdAsync(Guid notificationId, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                return await context.EmailNotificationQueues
                    .Where(e => e.NotificationId == notificationId)
                    .OrderBy(e => e.CreatedAt)
                    .ToListAsync(token);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting email queue entries for notification: {notificationId}", ex);
                throw;
            }
        }

        public async Task<IEnumerable<EmailNotificationQueue>> GetByUsernameAsync(string username, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                return await context.EmailNotificationQueues
                    .Include(e => e.NotificationMessage)
                    .Where(e => e.Username == username)
                    .OrderByDescending(e => e.CreatedAt)
                    .ToListAsync(token);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting email queue entries for user: {username}", ex);
                throw;
            }
        }

        public async Task<IEnumerable<EmailNotificationQueue>> GetPendingEmailsAsync(int batchSize, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                var now = DateTime.UtcNow;
                
                return await context.EmailNotificationQueues
                    .Include(e => e.NotificationMessage)
                    .Where(e => e.Status == (int)EmailStatus.Pending && 
                               e.ScheduledSendTime <= now)
                    .OrderBy(e => e.Priority == (int)EmailPriority.High ? 0 : 1)
                    .ThenBy(e => e.ScheduledSendTime)
                    .Take(batchSize)
                    .ToListAsync(token);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting pending emails", ex);
                throw;
            }
        }

        public async Task<IEnumerable<EmailNotificationQueue>> GetByStatusAsync(int status, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                return await context.EmailNotificationQueues
                    .Include(e => e.NotificationMessage)
                    .Where(e => e.Status == status)
                    .OrderBy(e => e.CreatedAt)
                    .ToListAsync(token);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting emails by status: {status}", ex);
                throw;
            }
        }

        public async Task<IEnumerable<EmailNotificationQueue>> GetHighPriorityPendingAsync(int batchSize, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                var now = DateTime.UtcNow;
                
                return await context.EmailNotificationQueues
                    .Include(e => e.NotificationMessage)
                    .Where(e => e.Status == (int)EmailStatus.Pending && 
                               e.Priority == (int)EmailPriority.High &&
                               e.ScheduledSendTime <= now)
                    .OrderBy(e => e.ScheduledSendTime)
                    .Take(batchSize)
                    .ToListAsync(token);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error getting high priority pending emails", ex);
                throw;
            }
        }

        public async Task<IEnumerable<EmailNotificationQueue>> GetFailedForRetryAsync(int maxRetryCount, int batchSize, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                return await context.EmailNotificationQueues
                    .Include(e => e.NotificationMessage)
                    .Where(e => e.Status == (int)EmailStatus.Failed && 
                               e.RetryCount < maxRetryCount)
                    .OrderBy(e => e.RetryCount)
                    .ThenBy(e => e.CreatedAt)
                    .Take(batchSize)
                    .ToListAsync(token);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error getting failed emails for retry", ex);
                throw;
            }
        }

        public async Task<(int sent, int failed, int pending)> GetStatisticsAsync(DateTime startDate, DateTime endDate, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                var stats = await context.EmailNotificationQueues
                    .Where(e => e.CreatedAt >= startDate && e.CreatedAt <= endDate)
                    .GroupBy(e => e.Status)
                    .Select(g => new { Status = g.Key, Count = g.Count() })
                    .ToListAsync(token);

                var sent = stats.FirstOrDefault(s => s.Status == (int)EmailStatus.Sent)?.Count ?? 0;
                var failed = stats.FirstOrDefault(s => s.Status == (int)EmailStatus.Failed)?.Count ?? 0;
                var pending = stats.FirstOrDefault(s => s.Status == (int)EmailStatus.Pending)?.Count ?? 0;

                return (sent, failed, pending);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting email statistics for period: {startDate} to {endDate}", ex);
                throw;
            }
        }

        public async Task<IEnumerable<EmailNotificationQueue>> GetFailedEmailsAsync(int maxRetries, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                return await context.EmailNotificationQueues
                    .Include(e => e.NotificationMessage)
                    .Where(e => e.Status == (int)EmailStatus.Failed && e.RetryCount < maxRetries)
                    .OrderBy(e => e.RetryCount)
                    .ThenBy(e => e.CreatedAt)
                    .ToListAsync(token);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting failed emails with max retries: {maxRetries}", ex);
                throw;
            }
        }

        public async Task<IEnumerable<EmailNotificationQueue>> GetUserEmailsAsync(string username, DateTime sinceDate, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                return await context.EmailNotificationQueues
                    .Include(e => e.NotificationMessage)
                    .Where(e => e.Username == username && e.CreatedAt >= sinceDate)
                    .OrderByDescending(e => e.CreatedAt)
                    .ToListAsync(token);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting user emails for: {username} since: {sinceDate}", ex);
                throw;
            }
        }

        public async Task<Dictionary<string, int>> GetQueueStatisticsAsync(CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                var stats = await context.EmailNotificationQueues
                    .GroupBy(e => e.Status)
                    .Select(g => new { Status = g.Key, Count = g.Count() })
                    .ToListAsync(token);

                var result = new Dictionary<string, int>
                {
                    ["Pending"] = stats.FirstOrDefault(s => s.Status == (int)EmailStatus.Pending)?.Count ?? 0,
                    ["Processing"] = stats.FirstOrDefault(s => s.Status == (int)EmailStatus.Processing)?.Count ?? 0,
                    ["Sent"] = stats.FirstOrDefault(s => s.Status == (int)EmailStatus.Sent)?.Count ?? 0,
                    ["Failed"] = stats.FirstOrDefault(s => s.Status == (int)EmailStatus.Failed)?.Count ?? 0
                };

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error getting email queue statistics", ex);
                throw;
            }
        }

        #endregion

        #region Create Methods

        public async Task<EmailNotificationQueue> CreateAsync(EmailNotificationQueue emailQueue, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                emailQueue.Id = Guid.NewGuid();
                emailQueue.CreatedAt = DateTime.UtcNow;
                emailQueue.Status = (int)EmailStatus.Pending;
                emailQueue.RetryCount = 0;

                context.EmailNotificationQueues.Add(emailQueue);
                await context.SaveChangesAsync(token);
                return emailQueue;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating email queue entry for notification: {emailQueue.NotificationId}", ex);
                throw;
            }
        }

        public async Task<IEnumerable<EmailNotificationQueue>> CreateBatchAsync(IEnumerable<EmailNotificationQueue> emailQueues, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                var emailList = emailQueues.ToList();
                var now = DateTime.UtcNow;

                foreach (var email in emailList)
                {
                    email.Id = Guid.NewGuid();
                    email.CreatedAt = now;
                    email.Status = (int)EmailStatus.Pending;
                    email.RetryCount = 0;
                }

                context.EmailNotificationQueues.AddRange(emailList);
                await context.SaveChangesAsync(token);
                return emailList;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating batch of {emailQueues.Count()} email queue entries", ex);
                throw;
            }
        }

        #endregion

        #region Update Methods

        public async Task<EmailNotificationQueue> UpdateAsync(EmailNotificationQueue emailQueue, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                context.EmailNotificationQueues.Update(emailQueue);
                await context.SaveChangesAsync(token);
                return emailQueue;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating email queue entry: {emailQueue.Id}", ex);
                throw;
            }
        }

        public async Task<bool> MarkAsProcessingAsync(Guid id, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                var email = await context.EmailNotificationQueues
                    .FirstOrDefaultAsync(e => e.Id == id, token);

                if (email == null)
                    return false;

                email.Status = (int)EmailStatus.Processing;
                await context.SaveChangesAsync(token);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error marking email as processing: {id}", ex);
                throw;
            }
        }

        public async Task<bool> MarkAsSentAsync(Guid id, DateTime sentTime, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                var email = await context.EmailNotificationQueues
                    .FirstOrDefaultAsync(e => e.Id == id, token);

                if (email == null)
                    return false;

                email.Status = (int)EmailStatus.Sent;
                email.ActualSendTime = sentTime;
                await context.SaveChangesAsync(token);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error marking email as sent: {id}", ex);
                throw;
            }
        }

        public async Task<bool> MarkAsFailedAsync(Guid id, string failureReason, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                var email = await context.EmailNotificationQueues
                    .FirstOrDefaultAsync(e => e.Id == id, token);

                if (email == null)
                    return false;

                email.Status = (int)EmailStatus.Failed;
                email.FailureReason = failureReason;
                await context.SaveChangesAsync(token);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error marking email as failed: {id}", ex);
                throw;
            }
        }

        public async Task<int> IncrementRetryCountAsync(Guid id, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                var email = await context.EmailNotificationQueues
                    .FirstOrDefaultAsync(e => e.Id == id, token);

                if (email == null)
                    return -1;

                email.RetryCount++;
                email.Status = (int)EmailStatus.Pending; // Reset to pending for retry
                await context.SaveChangesAsync(token);
                return email.RetryCount;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error incrementing retry count for email: {id}", ex);
                throw;
            }
        }

        public async Task<bool> RescheduleAsync(Guid id, DateTime newScheduledTime, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                var email = await context.EmailNotificationQueues
                    .FirstOrDefaultAsync(e => e.Id == id, token);

                if (email == null)
                    return false;

                email.ScheduledSendTime = newScheduledTime;
                email.Status = (int)EmailStatus.Pending;
                await context.SaveChangesAsync(token);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error rescheduling email: {id}", ex);
                throw;
            }
        }

        public async Task<bool> UpdateStatusAsync(Guid id, EmailStatus status, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                var email = await context.EmailNotificationQueues
                    .FirstOrDefaultAsync(e => e.Id == id, token);

                if (email == null)
                    return false;

                email.Status = (int)status;
                
                await context.SaveChangesAsync(token);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating email status for: {id}", ex);
                throw;
            }
        }

        #endregion

        #region Delete Methods

        public async Task<int> DeleteSentEmailsAsync(DateTime olderThan, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                var sentEmails = await context.EmailNotificationQueues
                    .Where(e => e.Status == (int)EmailStatus.Sent && 
                               e.ActualSendTime < olderThan)
                    .ToListAsync(token);

                if (!sentEmails.Any())
                    return 0;

                context.EmailNotificationQueues.RemoveRange(sentEmails);
                await context.SaveChangesAsync(token);
                return sentEmails.Count;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting sent emails older than: {olderThan}", ex);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                var email = await context.EmailNotificationQueues
                    .FirstOrDefaultAsync(e => e.Id == id, token);

                if (email == null)
                    return false;

                context.EmailNotificationQueues.Remove(email);
                await context.SaveChangesAsync(token);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting email queue entry: {id}", ex);
                throw;
            }
        }

        public async Task<int> DeleteOldEmailsAsync(DateTime cutoffDate, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                var oldEmails = await context.EmailNotificationQueues
                    .Where(e => e.CreatedAt < cutoffDate && 
                               (e.Status == (int)EmailStatus.Sent || e.Status == (int)EmailStatus.Failed))
                    .ToListAsync(token);

                if (!oldEmails.Any())
                    return 0;

                context.EmailNotificationQueues.RemoveRange(oldEmails);
                await context.SaveChangesAsync(token);
                return oldEmails.Count;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting old emails before: {cutoffDate}", ex);
                throw;
            }
        }

        #endregion

        #region Concurrency Control Methods

        public async Task<List<EmailNotificationQueue>> ClaimPendingEmailsAsync(string instanceId, int batchSize, CancellationToken cancellationToken = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
                
                // Use raw SQL for atomic UPDATE with OUTPUT clause
                var sql = @"
                    UPDATE TOP (@batchSize) enq
                    SET Status = @processingStatus,
                        ProcessingInstance = @instanceId,
                        ProcessingStartedAt = GETUTCDATE()
                    OUTPUT INSERTED.*
                    FROM EmailNotificationQueue enq
                    WHERE Status = @pendingStatus
                      AND ScheduledSendTime <= GETUTCDATE()
                      AND (ProcessingStartedAt IS NULL 
                           OR DATEDIFF(MINUTE, ProcessingStartedAt, GETUTCDATE()) > 5)";

                var pendingStatus = (int)EmailStatus.Pending;
                var processingStatus = (int)EmailStatus.Processing;

                // Execute the atomic claim operation
                var claimedEmails = await context.EmailNotificationQueues
                    .FromSqlRaw(sql, 
                        new Microsoft.Data.SqlClient.SqlParameter("@batchSize", batchSize),
                        new Microsoft.Data.SqlClient.SqlParameter("@processingStatus", processingStatus),
                        new Microsoft.Data.SqlClient.SqlParameter("@pendingStatus", pendingStatus),
                        new Microsoft.Data.SqlClient.SqlParameter("@instanceId", instanceId))
                    .ToListAsync(cancellationToken);

                // Include navigation properties if needed
                if (claimedEmails.Any())
                {
                    var ids = claimedEmails.Select(e => e.Id).ToList();
                    var emailsWithNavigation = await context.EmailNotificationQueues
                        .Include(e => e.NotificationMessage)
                        .Where(e => ids.Contains(e.Id))
                        .ToListAsync(cancellationToken);
                    
                    return emailsWithNavigation;
                }

                return claimedEmails;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error claiming pending emails for instance: {instanceId}", ex);
                throw;
            }
        }

        #endregion
    }
}