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
    /// Repository implementation for ApplicationNotificationProfile operations using Entity Framework Core.
    /// </summary>
    public class ApplicationNotificationProfileRepository : IApplicationNotificationProfileRepository
    {
        #region Fields

        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly ILogHelper _logger;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the ApplicationNotificationProfileRepository class.
        /// </summary>
        /// <param name="contextFactory">The factory for creating database contexts.</param>
        /// <param name="logger">The logger for error logging.</param>
        public ApplicationNotificationProfileRepository(IDbContextFactory<AppDbContext> contextFactory, ILogHelper logger)
        {
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #endregion

        #region Query Methods

        public async Task<ApplicationNotificationProfile> GetByIdAsync(Guid id, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                return await context.ApplicationNotificationProfiles
                    .FirstOrDefaultAsync(a => a.Id == id, token);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting application profile by ID: {id}", ex);
                throw;
            }
        }

        public async Task<ApplicationNotificationProfile> GetByApplicationNameAsync(string applicationName, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                return await context.ApplicationNotificationProfiles
                    .FirstOrDefaultAsync(a => a.ApplicationName == applicationName, token);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting application profile by name: {applicationName}", ex);
                throw;
            }
        }

        public async Task<IEnumerable<ApplicationNotificationProfile>> GetAllActiveAsync(CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                return await context.ApplicationNotificationProfiles
                    .Where(a => a.IsActive)
                    .OrderBy(a => a.DisplayName)
                    .ToListAsync(token);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error getting all active application profiles", ex);
                throw;
            }
        }

        public async Task<IEnumerable<ApplicationNotificationProfile>> GetEnabledByDefaultAsync(CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                return await context.ApplicationNotificationProfiles
                    .Where(a => a.IsActive && a.EnabledByDefault)
                    .OrderBy(a => a.DisplayName)
                    .ToListAsync(token);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error getting application profiles enabled by default", ex);
                throw;
            }
        }

        public async Task<IEnumerable<ApplicationNotificationProfile>> GetBySupportedAlertTypeAsync(int alertType, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                var profiles = await context.ApplicationNotificationProfiles
                    .Where(a => a.IsActive)
                    .ToListAsync(token);

                // Filter in memory since JSON operations may not be supported by the database provider
                return profiles.Where(p =>
                {
                    try
                    {
                        var alertTypes = JsonConvert.DeserializeObject<List<int>>(p.SupportedAlertTypes);
                        return alertTypes != null && alertTypes.Contains(alertType);
                    }
                    catch
                    {
                        return false;
                    }
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting application profiles by alert type: {alertType}", ex);
                throw;
            }
        }

        public async Task<bool> ExistsAsync(string applicationName, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                return await context.ApplicationNotificationProfiles
                    .AnyAsync(a => a.ApplicationName == applicationName, token);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error checking if application profile exists: {applicationName}", ex);
                throw;
            }
        }

        public async Task<ApplicationNotificationProfile> GetByNameAsync(string name, CancellationToken token = default)
        {
            return await GetByApplicationNameAsync(name, token);
        }

        public async Task<IEnumerable<ApplicationNotificationProfile>> GetActiveProfilesAsync(CancellationToken token = default)
        {
            return await GetAllActiveAsync(token);
        }

        #endregion

        #region Create Methods

        public async Task<ApplicationNotificationProfile> CreateAsync(ApplicationNotificationProfile profile, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                profile.Id = Guid.NewGuid();
                profile.CreatedAt = DateTime.UtcNow;
                profile.UpdatedAt = DateTime.UtcNow;
                profile.IsActive = true;

                context.ApplicationNotificationProfiles.Add(profile);
                await context.SaveChangesAsync(token);
                return profile;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating application profile: {profile.ApplicationName}", ex);
                throw;
            }
        }

        #endregion

        #region Update Methods

        public async Task<ApplicationNotificationProfile> UpdateAsync(ApplicationNotificationProfile profile, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                profile.UpdatedAt = DateTime.UtcNow;
                context.ApplicationNotificationProfiles.Update(profile);
                await context.SaveChangesAsync(token);
                return profile;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating application profile: {profile.ApplicationName}", ex);
                throw;
            }
        }

        public async Task<bool> UpdateSupportedAlertTypesAsync(string applicationName, string supportedAlertTypes, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                var profile = await context.ApplicationNotificationProfiles
                    .FirstOrDefaultAsync(a => a.ApplicationName == applicationName, token);

                if (profile == null)
                    return false;

                profile.SupportedAlertTypes = supportedAlertTypes;
                profile.UpdatedAt = DateTime.UtcNow;

                await context.SaveChangesAsync(token);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating supported alert types for application: {applicationName}", ex);
                throw;
            }
        }

        public async Task<bool> UpdateDefaultSeverityFilterAsync(string applicationName, int defaultSeverityFilter, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                var profile = await context.ApplicationNotificationProfiles
                    .FirstOrDefaultAsync(a => a.ApplicationName == applicationName, token);

                if (profile == null)
                    return false;

                profile.DefaultSeverityFilter = defaultSeverityFilter;
                profile.UpdatedAt = DateTime.UtcNow;

                await context.SaveChangesAsync(token);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating default severity filter for application: {applicationName}", ex);
                throw;
            }
        }

        #endregion

        #region Delete Methods

        public async Task<bool> DeactivateAsync(string applicationName, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                var profile = await context.ApplicationNotificationProfiles
                    .FirstOrDefaultAsync(a => a.ApplicationName == applicationName, token);

                if (profile == null)
                    return false;

                profile.IsActive = false;
                profile.UpdatedAt = DateTime.UtcNow;

                await context.SaveChangesAsync(token);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deactivating application profile: {applicationName}", ex);
                throw;
            }
        }

        #endregion
    }
}