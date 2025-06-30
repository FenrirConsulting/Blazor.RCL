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
    /// Repository implementation for UserNotificationSettings operations using Entity Framework Core.
    /// </summary>
    public class UserNotificationSettingsRepository : IUserNotificationSettingsRepository
    {
        #region Fields

        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly ILogHelper _logger;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the UserNotificationSettingsRepository class.
        /// </summary>
        /// <param name="contextFactory">The factory for creating database contexts.</param>
        /// <param name="logger">The logger for error logging.</param>
        public UserNotificationSettingsRepository(IDbContextFactory<AppDbContext> contextFactory, ILogHelper logger)
        {
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #endregion

        #region Query Methods

        public async Task<UserNotificationSettings> GetByUsernameAsync(string username, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                return await context.UserNotificationSettings
                    .Include(u => u.UserSettings)
                    .FirstOrDefaultAsync(u => u.Username == username, token);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting user notification settings for username: {username}", ex);
                throw;
            }
        }

        public async Task<UserNotificationSettings> GetByIdAsync(Guid id, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                return await context.UserNotificationSettings
                    .Include(u => u.UserSettings)
                    .FirstOrDefaultAsync(u => u.Id == id, token);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting user notification settings by ID: {id}", ex);
                throw;
            }
        }

        public async Task<IEnumerable<UserNotificationSettings>> GetAllActiveAsync(CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                return await context.UserNotificationSettings
                    .Where(u => u.IsActive)
                    .OrderBy(u => u.Username)
                    .ToListAsync(token);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error getting all active user notification settings", ex);
                throw;
            }
        }

        public async Task<IEnumerable<UserNotificationSettings>> GetEmailEnabledUsersAsync(CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                return await context.UserNotificationSettings
                    .Where(u => u.IsActive && u.EnableEmailNotifications)
                    .OrderBy(u => u.Username)
                    .ToListAsync(token);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error getting email enabled users", ex);
                throw;
            }
        }

        public async Task<IEnumerable<UserNotificationSettings>> GetByEmailFrequencyAsync(int frequency, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                return await context.UserNotificationSettings
                    .Where(u => u.IsActive && u.EnableEmailNotifications && u.GlobalEmailFrequency == frequency)
                    .OrderBy(u => u.Username)
                    .ToListAsync(token);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting users by email frequency: {frequency}", ex);
                throw;
            }
        }

        public async Task<bool> ExistsAsync(string username, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                return await context.UserNotificationSettings
                    .AnyAsync(u => u.Username == username, token);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error checking if user notification settings exist for username: {username}", ex);
                throw;
            }
        }

        #endregion

        #region Create Methods

        public async Task<UserNotificationSettings> CreateAsync(UserNotificationSettings settings, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                settings.Id = Guid.NewGuid();
                settings.CreatedAt = DateTime.UtcNow;
                settings.UpdatedAt = DateTime.UtcNow;
                settings.IsActive = true;

                context.UserNotificationSettings.Add(settings);
                await context.SaveChangesAsync(token);
                return settings;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating user notification settings for username: {settings.Username}", ex);
                throw;
            }
        }

        public async Task<UserNotificationSettings> CreateDefaultAsync(string username, string emailAddress, CancellationToken token = default)
        {
            try
            {
                var settings = new UserNotificationSettings
                {
                    Id = Guid.NewGuid(),
                    Username = username,
                    EmailAddress = emailAddress,
                    EnableEmailNotifications = true,
                    GlobalEmailFrequency = (int)EmailFrequency.Immediate,
                    TimeZone = "Eastern Standard Time",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                using var context = await _contextFactory.CreateDbContextAsync(token);
                context.UserNotificationSettings.Add(settings);
                await context.SaveChangesAsync(token);
                return settings;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating default notification settings for username: {username}", ex);
                throw;
            }
        }

        #endregion

        #region Update Methods

        public async Task<UserNotificationSettings> UpdateAsync(UserNotificationSettings settings, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                settings.UpdatedAt = DateTime.UtcNow;
                context.UserNotificationSettings.Update(settings);
                await context.SaveChangesAsync(token);
                return settings;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating user notification settings for username: {settings.Username}", ex);
                throw;
            }
        }

        public async Task<bool> UpdateEmailPreferencesAsync(string username, bool enableEmail, int frequency, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                var settings = await context.UserNotificationSettings
                    .FirstOrDefaultAsync(u => u.Username == username, token);

                if (settings == null)
                    return false;

                settings.EnableEmailNotifications = enableEmail;
                settings.GlobalEmailFrequency = frequency;
                settings.UpdatedAt = DateTime.UtcNow;

                await context.SaveChangesAsync(token);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating email preferences for username: {username}", ex);
                throw;
            }
        }

        public async Task<bool> UpdateQuietHoursAsync(string username, TimeSpan? quietHoursStart, TimeSpan? quietHoursEnd, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                var settings = await context.UserNotificationSettings
                    .FirstOrDefaultAsync(u => u.Username == username, token);

                if (settings == null)
                    return false;

                settings.QuietHoursStart = quietHoursStart;
                settings.QuietHoursEnd = quietHoursEnd;
                settings.UpdatedAt = DateTime.UtcNow;

                await context.SaveChangesAsync(token);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating quiet hours for username: {username}", ex);
                throw;
            }
        }

        #endregion

        #region Delete Methods

        public async Task<bool> DeactivateAsync(string username, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                var settings = await context.UserNotificationSettings
                    .FirstOrDefaultAsync(u => u.Username == username, token);

                if (settings == null)
                    return false;

                settings.IsActive = false;
                settings.UpdatedAt = DateTime.UtcNow;

                await context.SaveChangesAsync(token);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deactivating user notification settings for username: {username}", ex);
                throw;
            }
        }

        #endregion
    }
}