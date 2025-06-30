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
    /// Repository implementation for NotificationMessage operations using Entity Framework Core.
    /// </summary>
    public class NotificationMessageRepository : INotificationMessageRepository
    {
        #region Fields

        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly ILogHelper _logger;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the NotificationMessageRepository class.
        /// </summary>
        /// <param name="contextFactory">The factory for creating database contexts.</param>
        /// <param name="logger">The logger for error logging.</param>
        public NotificationMessageRepository(IDbContextFactory<AppDbContext> contextFactory, ILogHelper logger)
        {
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #endregion

        #region Query Methods

        public async Task<NotificationMessage> GetByIdAsync(Guid id, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                return await context.NotificationMessages
                    .FirstOrDefaultAsync(n => n.Id == id, token);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting notification by ID: {id}", ex);
                throw;
            }
        }

        public async Task<NotificationMessage> GetByMessageIdAsync(string messageId, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                return await context.NotificationMessages
                    .FirstOrDefaultAsync(n => n.MessageId == messageId, token);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting notification by MessageId: {messageId}", ex);
                throw;
            }
        }

        public async Task<IEnumerable<NotificationMessage>> GetActiveByApplicationAsync(string applicationName, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                return await context.NotificationMessages
                    .Where(n => n.SourceApplication == applicationName && n.IsActive)
                    .OrderByDescending(n => n.CreatedAt)
                    .ToListAsync(token);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting active notifications for application: {applicationName}", ex);
                throw;
            }
        }

        public async Task<IEnumerable<NotificationMessage>> GetBySeverityAsync(int severity, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                return await context.NotificationMessages
                    .Where(n => n.Severity >= severity && n.IsActive)
                    .OrderByDescending(n => n.CreatedAt)
                    .ToListAsync(token);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting notifications by severity: {severity}", ex);
                throw;
            }
        }

        public async Task<IEnumerable<NotificationMessage>> GetByAlertTypeAsync(int alertType, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                return await context.NotificationMessages
                    .Where(n => n.AlertType == alertType && n.IsActive)
                    .OrderByDescending(n => n.CreatedAt)
                    .ToListAsync(token);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting notifications by alert type: {alertType}", ex);
                throw;
            }
        }

        public async Task<IEnumerable<NotificationMessage>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                return await context.NotificationMessages
                    .Where(n => n.CreatedAt >= startDate && n.CreatedAt <= endDate)
                    .OrderByDescending(n => n.CreatedAt)
                    .ToListAsync(token);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting notifications by date range: {startDate} to {endDate}", ex);
                throw;
            }
        }

        public async Task<IEnumerable<NotificationMessage>> GetRequiringConfirmationAsync(CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                return await context.NotificationMessages
                    .Where(n => n.RequiresConfirmation && n.IsActive)
                    .OrderByDescending(n => n.CreatedAt)
                    .ToListAsync(token);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error getting notifications requiring confirmation", ex);
                throw;
            }
        }

        public async Task<IEnumerable<NotificationMessage>> GetExpiredAsync(CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                var now = DateTime.UtcNow;
                return await context.NotificationMessages
                    .Where(n => n.ExpiresAt.HasValue && n.ExpiresAt.Value < now)
                    .ToListAsync(token);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error getting expired notifications", ex);
                throw;
            }
        }

        #endregion

        #region Create Methods

        public async Task<NotificationMessage> CreateAsync(NotificationMessage notification, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                notification.Id = Guid.NewGuid();
                notification.CreatedAt = DateTime.UtcNow;
                notification.IsActive = true;

                context.NotificationMessages.Add(notification);
                await context.SaveChangesAsync(token);
                return notification;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating notification: {notification.Title}", ex);
                throw;
            }
        }

        public async Task<IEnumerable<NotificationMessage>> CreateBatchAsync(IEnumerable<NotificationMessage> notifications, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                var notificationList = notifications.ToList();
                var now = DateTime.UtcNow;

                foreach (var notification in notificationList)
                {
                    notification.Id = Guid.NewGuid();
                    notification.CreatedAt = now;
                    notification.IsActive = true;
                }

                context.NotificationMessages.AddRange(notificationList);
                await context.SaveChangesAsync(token);
                return notificationList;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating batch of {notifications.Count()} notifications", ex);
                throw;
            }
        }

        #endregion

        #region Update Methods

        public async Task<NotificationMessage> UpdateAsync(NotificationMessage notification, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                context.NotificationMessages.Update(notification);
                await context.SaveChangesAsync(token);
                return notification;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating notification: {notification.Id}", ex);
                throw;
            }
        }

        public async Task<bool> DeactivateAsync(Guid id, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                var notification = await context.NotificationMessages
                    .FirstOrDefaultAsync(n => n.Id == id, token);

                if (notification == null)
                    return false;

                notification.IsActive = false;
                await context.SaveChangesAsync(token);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deactivating notification: {id}", ex);
                throw;
            }
        }

        #endregion

        #region Delete Methods

        public async Task<int> DeleteExpiredAsync(DateTime olderThan, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                var expiredNotifications = await context.NotificationMessages
                    .Where(n => n.ExpiresAt.HasValue && n.ExpiresAt.Value < olderThan)
                    .ToListAsync(token);

                if (!expiredNotifications.Any())
                    return 0;

                context.NotificationMessages.RemoveRange(expiredNotifications);
                await context.SaveChangesAsync(token);
                return expiredNotifications.Count;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting expired notifications older than: {olderThan}", ex);
                throw;
            }
        }

        #endregion
    }
}