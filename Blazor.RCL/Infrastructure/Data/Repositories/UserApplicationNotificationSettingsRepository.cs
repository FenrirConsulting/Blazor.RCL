using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Blazor.RCL.Application.Interfaces;
using Blazor.RCL.Domain.Entities.Notifications;
using Blazor.RCL.NLog.LogService.Interface;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Blazor.RCL.Infrastructure.Data.Repositories
{
    /// <summary>
    /// Repository implementation for UserApplicationNotificationSettings operations using Entity Framework Core.
    /// </summary>
    public class UserApplicationNotificationSettingsRepository : IUserApplicationNotificationSettingsRepository
    {
        #region Fields

        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly ILogHelper _logger;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the UserApplicationNotificationSettingsRepository class.
        /// </summary>
        /// <param name="contextFactory">The factory for creating database contexts.</param>
        /// <param name="logger">The logger for error logging.</param>
        public UserApplicationNotificationSettingsRepository(IDbContextFactory<AppDbContext> contextFactory, ILogHelper logger)
        {
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #endregion

        #region Query Methods

        public async Task<UserApplicationNotificationSettings> GetByIdAsync(Guid id, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                return await context.UserApplicationNotificationSettings
                    .Include(u => u.UserNotificationSettings)
                    .Include(u => u.ApplicationNotificationProfile)
                    .FirstOrDefaultAsync(u => u.Id == id, token);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting user application settings by ID: {id}", ex);
                throw;
            }
        }

        public async Task<UserApplicationNotificationSettings> GetByUserAndApplicationAsync(string username, string applicationName, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                return await context.UserApplicationNotificationSettings
                    .Include(u => u.UserNotificationSettings)
                    .Include(u => u.ApplicationNotificationProfile)
                    .FirstOrDefaultAsync(u => u.Username == username && u.ApplicationName == applicationName, token);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting settings for user: {username}, application: {applicationName}", ex);
                throw;
            }
        }

        public async Task<IEnumerable<UserApplicationNotificationSettings>> GetByUsernameAsync(string username, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                return await context.UserApplicationNotificationSettings
                    .Include(u => u.ApplicationNotificationProfile)
                    .Where(u => u.Username == username)
                    .OrderBy(u => u.ApplicationName)
                    .ToListAsync(token);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting application settings for user: {username}", ex);
                throw;
            }
        }

        public async Task<IEnumerable<UserApplicationNotificationSettings>> GetByApplicationAsync(string applicationName, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                return await context.UserApplicationNotificationSettings
                    .Include(u => u.UserNotificationSettings)
                    .Where(u => u.ApplicationName == applicationName)
                    .OrderBy(u => u.Username)
                    .ToListAsync(token);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting user settings for application: {applicationName}", ex);
                throw;
            }
        }

        public async Task<IEnumerable<UserApplicationNotificationSettings>> GetSubscribedUsersAsync(string applicationName, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                return await context.UserApplicationNotificationSettings
                    .Include(u => u.UserNotificationSettings)
                    .Where(u => u.ApplicationName == applicationName && u.IsSubscribed)
                    .OrderBy(u => u.Username)
                    .ToListAsync(token);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting subscribed users for application: {applicationName}", ex);
                throw;
            }
        }

        public async Task<IEnumerable<UserApplicationNotificationSettings>> GetSubscribedUsersBySeverityAsync(string applicationName, int minSeverity, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                return await context.UserApplicationNotificationSettings
                    .Include(u => u.UserNotificationSettings)
                    .Where(u => u.ApplicationName == applicationName && 
                               u.IsSubscribed && 
                               u.SeverityFilter <= minSeverity)
                    .OrderBy(u => u.Username)
                    .ToListAsync(token);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting subscribed users by severity for application: {applicationName}", ex);
                throw;
            }
        }

        public async Task<bool> IsSubscribedAsync(string username, string applicationName, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                return await context.UserApplicationNotificationSettings
                    .AnyAsync(u => u.Username == username && u.ApplicationName == applicationName && u.IsSubscribed, token);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error checking subscription for user: {username}, application: {applicationName}", ex);
                throw;
            }
        }

        #endregion

        #region Create Methods

        public async Task<UserApplicationNotificationSettings> CreateAsync(UserApplicationNotificationSettings settings, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                settings.Id = Guid.NewGuid();
                settings.CreatedAt = DateTime.UtcNow;
                settings.UpdatedAt = DateTime.UtcNow;

                context.UserApplicationNotificationSettings.Add(settings);
                await context.SaveChangesAsync(token);
                return settings;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating settings for user: {settings.Username}, application: {settings.ApplicationName}", ex);
                throw;
            }
        }

        public async Task<UserApplicationNotificationSettings> CreateDefaultAsync(string username, string applicationName, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                
                // Get application profile to use defaults
                var appProfile = await context.ApplicationNotificationProfiles
                    .FirstOrDefaultAsync(a => a.ApplicationName == applicationName, token);

                var settings = new UserApplicationNotificationSettings
                {
                    Id = Guid.NewGuid(),
                    Username = username,
                    ApplicationName = applicationName,
                    IsSubscribed = appProfile?.EnabledByDefault ?? true,
                    EnableRealTimeNotifications = true,
                    EnableEmailNotifications = true,
                    EmailFrequency = (int)EmailFrequency.UseGlobal,
                    SeverityFilter = appProfile?.DefaultSeverityFilter ?? 0,
                    AlertTypeFilter = appProfile?.SupportedAlertTypes,
                    EnableCriticalAlerts = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                context.UserApplicationNotificationSettings.Add(settings);
                await context.SaveChangesAsync(token);
                return settings;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating default settings for user: {username}, application: {applicationName}", ex);
                throw;
            }
        }

        public async Task<IEnumerable<UserApplicationNotificationSettings>> CreateBatchForUserAsync(string username, IEnumerable<string> applicationNames, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                var settingsList = new List<UserApplicationNotificationSettings>();

                foreach (var appName in applicationNames)
                {
                    var appProfile = await context.ApplicationNotificationProfiles
                        .FirstOrDefaultAsync(a => a.ApplicationName == appName, token);

                    var settings = new UserApplicationNotificationSettings
                    {
                        Id = Guid.NewGuid(),
                        Username = username,
                        ApplicationName = appName,
                        IsSubscribed = appProfile?.EnabledByDefault ?? true,
                        EnableRealTimeNotifications = true,
                        EnableEmailNotifications = true,
                        EmailFrequency = (int)EmailFrequency.UseGlobal,
                        SeverityFilter = appProfile?.DefaultSeverityFilter ?? 0,
                        AlertTypeFilter = appProfile?.SupportedAlertTypes,
                        EnableCriticalAlerts = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    settingsList.Add(settings);
                }

                context.UserApplicationNotificationSettings.AddRange(settingsList);
                await context.SaveChangesAsync(token);
                return settingsList;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating batch settings for user: {username}", ex);
                throw;
            }
        }

        public async Task<UserApplicationNotificationSettings> CreateFromApplicationDefaultsAsync(string username, string applicationName, CancellationToken token = default)
        {
            return await CreateDefaultAsync(username, applicationName, token);
        }

        #endregion

        #region Update Methods

        public async Task<UserApplicationNotificationSettings> UpdateAsync(UserApplicationNotificationSettings settings, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                settings.UpdatedAt = DateTime.UtcNow;
                context.UserApplicationNotificationSettings.Update(settings);
                await context.SaveChangesAsync(token);
                return settings;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating settings for user: {settings.Username}, application: {settings.ApplicationName}", ex);
                throw;
            }
        }

        public async Task<bool> UpdateSubscriptionAsync(string username, string applicationName, bool isSubscribed, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                var settings = await context.UserApplicationNotificationSettings
                    .FirstOrDefaultAsync(u => u.Username == username && u.ApplicationName == applicationName, token);

                if (settings == null)
                    return false;

                settings.IsSubscribed = isSubscribed;
                settings.UpdatedAt = DateTime.UtcNow;

                await context.SaveChangesAsync(token);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating subscription for user: {username}, application: {applicationName}", ex);
                throw;
            }
        }

        public async Task<bool> UpdateNotificationPreferencesAsync(
            string username, 
            string applicationName, 
            bool enableRealTime, 
            bool enableEmail, 
            int emailFrequency, 
            CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                var settings = await context.UserApplicationNotificationSettings
                    .FirstOrDefaultAsync(u => u.Username == username && u.ApplicationName == applicationName, token);

                if (settings == null)
                    return false;

                settings.EnableRealTimeNotifications = enableRealTime;
                settings.EnableEmailNotifications = enableEmail;
                settings.EmailFrequency = emailFrequency;
                settings.UpdatedAt = DateTime.UtcNow;

                await context.SaveChangesAsync(token);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating preferences for user: {username}, application: {applicationName}", ex);
                throw;
            }
        }

        #endregion

        #region Delete Methods

        public async Task<bool> DeleteAsync(string username, string applicationName, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                var settings = await context.UserApplicationNotificationSettings
                    .FirstOrDefaultAsync(u => u.Username == username && u.ApplicationName == applicationName, token);

                if (settings == null)
                    return false;

                context.UserApplicationNotificationSettings.Remove(settings);
                await context.SaveChangesAsync(token);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting settings for user: {username}, application: {applicationName}", ex);
                throw;
            }
        }

        public async Task<int> DeleteAllForUserAsync(string username, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                var settings = await context.UserApplicationNotificationSettings
                    .Where(u => u.Username == username)
                    .ToListAsync(token);

                if (!settings.Any())
                    return 0;

                context.UserApplicationNotificationSettings.RemoveRange(settings);
                await context.SaveChangesAsync(token);
                return settings.Count;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting all settings for user: {username}", ex);
                throw;
            }
        }

        #endregion
    }
}