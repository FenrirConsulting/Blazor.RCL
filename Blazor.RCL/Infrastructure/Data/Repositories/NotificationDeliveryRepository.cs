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
    /// Repository implementation for NotificationDelivery operations using Entity Framework Core.
    /// </summary>
    public class NotificationDeliveryRepository : INotificationDeliveryRepository
    {
        #region Fields

        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly ILogHelper _logger;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the NotificationDeliveryRepository class.
        /// </summary>
        /// <param name="contextFactory">The factory for creating database contexts.</param>
        /// <param name="logger">The logger for error logging.</param>
        public NotificationDeliveryRepository(IDbContextFactory<AppDbContext> contextFactory, ILogHelper logger)
        {
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #endregion

        #region Query Methods

        public async Task<NotificationDelivery> GetByIdAsync(Guid id, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                return await context.NotificationDeliveries
                    .Include(d => d.NotificationMessage)
                    .Include(d => d.UserNotificationSettings)
                    .FirstOrDefaultAsync(d => d.Id == id, token);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting delivery record by ID: {id}", ex);
                throw;
            }
        }

        public async Task<IEnumerable<NotificationDelivery>> GetByNotificationIdAsync(Guid notificationId, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                return await context.NotificationDeliveries
                    .Include(d => d.UserNotificationSettings)
                    .Where(d => d.NotificationId == notificationId)
                    .OrderBy(d => d.Username)
                    .ToListAsync(token);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting delivery records for notification: {notificationId}", ex);
                throw;
            }
        }

        public async Task<IEnumerable<NotificationDelivery>> GetByUsernameAsync(string username, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                return await context.NotificationDeliveries
                    .Include(d => d.NotificationMessage)
                    .Where(d => d.Username == username)
                    .OrderByDescending(d => d.CreatedAt)
                    .ToListAsync(token);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting delivery records for user: {username}", ex);
                throw;
            }
        }

        public async Task<IEnumerable<NotificationDelivery>> GetByStatusAsync(int deliveryStatus, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                return await context.NotificationDeliveries
                    .Include(d => d.NotificationMessage)
                    .Where(d => d.DeliveryStatus == deliveryStatus)
                    .OrderBy(d => d.CreatedAt)
                    .ToListAsync(token);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting delivery records by status: {deliveryStatus}", ex);
                throw;
            }
        }

        public async Task<IEnumerable<NotificationDelivery>> GetPendingByChannelAsync(int deliveryChannel, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                return await context.NotificationDeliveries
                    .Include(d => d.NotificationMessage)
                    .Include(d => d.UserNotificationSettings)
                    .Where(d => d.DeliveryChannel == deliveryChannel && 
                               d.DeliveryStatus == (int)DeliveryStatus.Pending)
                    .OrderBy(d => d.CreatedAt)
                    .ToListAsync(token);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting pending deliveries for channel: {deliveryChannel}", ex);
                throw;
            }
        }

        public async Task<IEnumerable<NotificationDelivery>> GetFailedForRetryAsync(int maxRetryCount, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                return await context.NotificationDeliveries
                    .Include(d => d.NotificationMessage)
                    .Include(d => d.UserNotificationSettings)
                    .Where(d => d.DeliveryStatus == (int)DeliveryStatus.Failed && 
                               d.RetryCount < maxRetryCount)
                    .OrderBy(d => d.CreatedAt)
                    .ToListAsync(token);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting failed deliveries for retry", ex);
                throw;
            }
        }

        public async Task<IEnumerable<NotificationDelivery>> GetUnconfirmedAsync(DateTime olderThan, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                return await context.NotificationDeliveries
                    .Include(d => d.NotificationMessage)
                    .Where(d => d.DeliveryStatus == (int)DeliveryStatus.Delivered && 
                               d.ConfirmedAt == null &&
                               d.DeliveredAt < olderThan &&
                               d.NotificationMessage.RequiresConfirmation)
                    .ToListAsync(token);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting unconfirmed deliveries older than: {olderThan}", ex);
                throw;
            }
        }

        public async Task<bool> IsDeliveredAsync(Guid notificationId, string username, int deliveryChannel, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                return await context.NotificationDeliveries
                    .AnyAsync(d => d.NotificationId == notificationId && 
                                  d.Username == username && 
                                  d.DeliveryChannel == deliveryChannel &&
                                  d.DeliveryStatus == (int)DeliveryStatus.Delivered, token);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error checking delivery status for notification: {notificationId}, user: {username}", ex);
                throw;
            }
        }

        public async Task<IEnumerable<NotificationDelivery>> GetByNotificationAndUserAsync(Guid notificationId, string username, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                return await context.NotificationDeliveries
                    .Include(d => d.NotificationMessage)
                    .Where(d => d.NotificationId == notificationId && d.Username == username)
                    .ToListAsync(token);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting delivery records for notification: {notificationId}, user: {username}", ex);
                throw;
            }
        }

        public async Task<IEnumerable<NotificationDelivery>> GetUserDeliveriesAsync(string username, bool includeConfirmed, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                var query = context.NotificationDeliveries
                    .Include(d => d.NotificationMessage)
                    .Where(d => d.Username == username);

                if (!includeConfirmed)
                {
                    query = query.Where(d => d.DeliveryStatus != (int)DeliveryStatus.Confirmed);
                }

                return await query
                    .OrderByDescending(d => d.CreatedAt)
                    .ToListAsync(token);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting user deliveries for: {username}", ex);
                throw;
            }
        }

        public async Task<IEnumerable<NotificationDelivery>> GetUnconfirmedDeliveriesAsync(string username, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                return await context.NotificationDeliveries
                    .Include(d => d.NotificationMessage)
                    .Where(d => d.Username == username && 
                           d.DeliveryStatus != (int)DeliveryStatus.Confirmed &&
                           d.NotificationMessage.RequiresConfirmation)
                    .OrderByDescending(d => d.CreatedAt)
                    .ToListAsync(token);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting unconfirmed deliveries for user: {username}", ex);
                throw;
            }
        }

        public async Task<IEnumerable<NotificationDelivery>> GetFailedDeliveriesByNotificationAsync(Guid notificationId, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                return await context.NotificationDeliveries
                    .Include(d => d.NotificationMessage)
                    .Where(d => d.NotificationId == notificationId && 
                           d.DeliveryStatus == (int)DeliveryStatus.Failed)
                    .ToListAsync(token);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting failed deliveries for notification: {notificationId}", ex);
                throw;
            }
        }

        #endregion

        #region Create Methods

        public async Task<NotificationDelivery> CreateAsync(NotificationDelivery delivery, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                delivery.Id = Guid.NewGuid();
                delivery.CreatedAt = DateTime.UtcNow;
                delivery.DeliveryStatus = (int)DeliveryStatus.Pending;
                delivery.RetryCount = 0;

                context.NotificationDeliveries.Add(delivery);
                await context.SaveChangesAsync(token);
                return delivery;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating delivery record for notification: {delivery.NotificationId}", ex);
                throw;
            }
        }

        public async Task<IEnumerable<NotificationDelivery>> CreateBatchAsync(IEnumerable<NotificationDelivery> deliveries, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                var deliveryList = deliveries.ToList();
                var now = DateTime.UtcNow;

                foreach (var delivery in deliveryList)
                {
                    delivery.Id = Guid.NewGuid();
                    delivery.CreatedAt = now;
                    delivery.DeliveryStatus = (int)DeliveryStatus.Pending;
                    delivery.RetryCount = 0;
                }

                context.NotificationDeliveries.AddRange(deliveryList);
                await context.SaveChangesAsync(token);
                return deliveryList;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating batch of {deliveries.Count()} delivery records", ex);
                throw;
            }
        }

        #endregion

        #region Update Methods

        public async Task<NotificationDelivery> UpdateAsync(NotificationDelivery delivery, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                context.NotificationDeliveries.Update(delivery);
                await context.SaveChangesAsync(token);
                return delivery;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating delivery record: {delivery.Id}", ex);
                throw;
            }
        }

        public async Task<bool> MarkAsDeliveredAsync(Guid id, DateTime deliveredAt, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                var delivery = await context.NotificationDeliveries
                    .FirstOrDefaultAsync(d => d.Id == id, token);

                if (delivery == null)
                    return false;

                delivery.DeliveryStatus = (int)DeliveryStatus.Delivered;
                delivery.DeliveredAt = deliveredAt;

                await context.SaveChangesAsync(token);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error marking delivery as delivered: {id}", ex);
                throw;
            }
        }

        public async Task<bool> MarkAsFailedAsync(Guid id, string failureReason, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                var delivery = await context.NotificationDeliveries
                    .FirstOrDefaultAsync(d => d.Id == id, token);

                if (delivery == null)
                    return false;

                delivery.DeliveryStatus = (int)DeliveryStatus.Failed;
                delivery.FailureReason = failureReason;

                await context.SaveChangesAsync(token);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error marking delivery as failed: {id}", ex);
                throw;
            }
        }

        public async Task<bool> MarkAsConfirmedAsync(Guid id, DateTime confirmedAt, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                var delivery = await context.NotificationDeliveries
                    .FirstOrDefaultAsync(d => d.Id == id, token);

                if (delivery == null)
                    return false;

                delivery.DeliveryStatus = (int)DeliveryStatus.Confirmed;
                delivery.ConfirmedAt = confirmedAt;

                await context.SaveChangesAsync(token);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error marking delivery as confirmed: {id}", ex);
                throw;
            }
        }

        public async Task<int> IncrementRetryCountAsync(Guid id, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                var delivery = await context.NotificationDeliveries
                    .FirstOrDefaultAsync(d => d.Id == id, token);

                if (delivery == null)
                    return -1;

                delivery.RetryCount++;
                await context.SaveChangesAsync(token);
                return delivery.RetryCount;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error incrementing retry count for delivery: {id}", ex);
                throw;
            }
        }

        public async Task<bool> UpdateDeliveryStatusAsync(Guid id, DeliveryStatus status, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                var delivery = await context.NotificationDeliveries
                    .FirstOrDefaultAsync(d => d.Id == id, token);

                if (delivery == null)
                    return false;

                delivery.DeliveryStatus = (int)status;
                
                // Update timestamp based on status
                switch (status)
                {
                    case DeliveryStatus.Delivered:
                        delivery.DeliveredAt = DateTime.UtcNow;
                        break;
                    case DeliveryStatus.Confirmed:
                        delivery.ConfirmedAt = DateTime.UtcNow;
                        break;
                }

                await context.SaveChangesAsync(token);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating delivery status for: {id}", ex);
                throw;
            }
        }

        #endregion

        #region Delete Methods

        public async Task<int> DeleteOldRecordsAsync(DateTime olderThan, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                var oldRecords = await context.NotificationDeliveries
                    .Where(d => d.CreatedAt < olderThan)
                    .ToListAsync(token);

                if (!oldRecords.Any())
                    return 0;

                context.NotificationDeliveries.RemoveRange(oldRecords);
                await context.SaveChangesAsync(token);
                return oldRecords.Count;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting delivery records older than: {olderThan}", ex);
                throw;
            }
        }

        #endregion
    }
}