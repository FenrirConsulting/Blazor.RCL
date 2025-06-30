using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Blazor.RCL.Application.Interfaces;
using Blazor.RCL.Application.Models.Notifications;
using Blazor.RCL.Domain.Entities.Notifications;
using Blazor.RCL.NLog.LogService.Interface;

namespace Blazor.RCL.Infrastructure.Services
{
    /// <summary>
    /// Service implementation for managing application notification profiles
    /// </summary>
    public class ApplicationNotificationProfileService : IApplicationNotificationProfileService
    {
        private readonly IApplicationNotificationProfileRepository _profileRepository;
        private readonly IUserApplicationNotificationSettingsRepository _userAppSettingsRepository;
        private readonly ILogHelper _logger;

        public ApplicationNotificationProfileService(
            IApplicationNotificationProfileRepository profileRepository,
            IUserApplicationNotificationSettingsRepository userAppSettingsRepository,
            ILogHelper logger)
        {
            _profileRepository = profileRepository;
            _userAppSettingsRepository = userAppSettingsRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<ApplicationNotificationProfile>> GetActiveApplicationProfilesAsync()
        {
            try
            {
                return await _profileRepository.GetActiveProfilesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting active application profiles: {ex.Message}", ex);
                throw;
            }
        }

        public async Task<ApplicationNotificationProfile?> GetApplicationProfileAsync(string applicationName)
        {
            try
            {
                return await _profileRepository.GetByNameAsync(applicationName);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting application profile: {ex.Message}", ex);
                throw;
            }
        }

        public async Task<ApplicationNotificationProfile> CreateApplicationProfileAsync(CreateApplicationProfileRequest request)
        {
            try
            {
                // Check if application already exists
                var existing = await _profileRepository.GetByNameAsync(request.ApplicationName);
                if (existing != null)
                {
                    throw new InvalidOperationException($"Application profile '{request.ApplicationName}' already exists");
                }

                var profile = new ApplicationNotificationProfile
                {
                    Id = Guid.NewGuid(),
                    ApplicationName = request.ApplicationName,
                    DisplayName = request.DisplayName,
                    Description = request.Description,
                    SupportedAlertTypes = JsonSerializer.Serialize(request.SupportedAlertTypes),
                    DefaultSeverityFilter = (int)request.DefaultSeverityFilter,
                    EnabledByDefault = request.EnabledByDefault,
                    DefaultEmailTemplateKey = request.DefaultEmailTemplateKey,
                    IconUrl = request.IconUrl,
                    ContactEmail = request.ContactEmail,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _profileRepository.CreateAsync(profile);

                return profile;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating application profile: {ex.Message}", ex);
                throw;
            }
        }

        public async Task<ApplicationNotificationProfile> UpdateApplicationProfileAsync(
            string applicationName, 
            UpdateApplicationProfileRequest request)
        {
            try
            {
                var profile = await _profileRepository.GetByNameAsync(applicationName);
                if (profile == null)
                {
                    throw new InvalidOperationException($"Application profile '{applicationName}' not found");
                }

                if (request.DisplayName != null)
                    profile.DisplayName = request.DisplayName;
                
                if (request.Description != null)
                    profile.Description = request.Description;
                
                if (request.SupportedAlertTypes != null)
                    profile.SupportedAlertTypes = JsonSerializer.Serialize(request.SupportedAlertTypes);
                
                if (request.DefaultSeverityFilter.HasValue)
                    profile.DefaultSeverityFilter = (int)request.DefaultSeverityFilter.Value;
                
                if (request.EnabledByDefault.HasValue)
                    profile.EnabledByDefault = request.EnabledByDefault.Value;
                
                if (request.DefaultEmailTemplateKey != null)
                    profile.DefaultEmailTemplateKey = request.DefaultEmailTemplateKey;
                
                if (request.IconUrl != null)
                    profile.IconUrl = request.IconUrl;
                
                if (request.ContactEmail != null)
                    profile.ContactEmail = request.ContactEmail;

                profile.UpdatedAt = DateTime.UtcNow;
                await _profileRepository.UpdateAsync(profile);

                return profile;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating application profile: {ex.Message}", ex);
                throw;
            }
        }

        public async Task<IEnumerable<string>> GetSubscribedUsersAsync(string applicationName, bool activeOnly = true)
        {
            try
            {
                var userSettings = await _userAppSettingsRepository.GetSubscribedUsersAsync(applicationName);
                return userSettings.Select(s => s.Username);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting subscribed users: {ex.Message}", ex);
                throw;
            }
        }

        public async Task<ApplicationSubscriptionStats> GetApplicationSubscriptionStatsAsync(string applicationName)
        {
            try
            {
                var profile = await _profileRepository.GetByNameAsync(applicationName);
                if (profile == null)
                {
                    throw new InvalidOperationException($"Application profile '{applicationName}' not found");
                }

                var allSettings = await _userAppSettingsRepository.GetByApplicationAsync(applicationName);
                
                var stats = new ApplicationSubscriptionStats
                {
                    ApplicationName = applicationName,
                    TotalUsers = allSettings.Count(),
                    SubscribedUsers = allSettings.Count(s => s.IsSubscribed),
                    RealTimeEnabledUsers = allSettings.Count(s => s.IsSubscribed && s.EnableRealTimeNotifications),
                    EmailEnabledUsers = allSettings.Count(s => s.IsSubscribed && s.EnableEmailNotifications),
                    UnsubscribedUsers = allSettings.Count(s => !s.IsSubscribed),
                    CalculatedAt = DateTime.UtcNow
                };

                return stats;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting application subscription stats: {ex.Message}", ex);
                throw;
            }
        }

        public async Task<bool> IsAlertTypeSupportedAsync(string applicationName, AlertType alertType)
        {
            try
            {
                var profile = await _profileRepository.GetByNameAsync(applicationName);
                if (profile == null || !profile.IsActive)
                {
                    return false;
                }

                if (string.IsNullOrEmpty(profile.SupportedAlertTypes))
                {
                    return true; // If no filter, all types are supported
                }

                var supportedTypes = JsonSerializer.Deserialize<List<AlertType>>(profile.SupportedAlertTypes);
                return supportedTypes?.Contains(alertType) ?? true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error checking alert type support: {ex.Message}", ex);
                throw;
            }
        }

        public async Task<ApplicationNotificationProfile> ActivateApplicationProfileAsync(string applicationName)
        {
            try
            {
                var profile = await _profileRepository.GetByNameAsync(applicationName);
                if (profile == null)
                {
                    throw new InvalidOperationException($"Application profile '{applicationName}' not found");
                }

                if (!profile.IsActive)
                {
                    profile.IsActive = true;
                    profile.UpdatedAt = DateTime.UtcNow;
                    await _profileRepository.UpdateAsync(profile);

                }

                return profile;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error activating application profile: {ex.Message}", ex);
                throw;
            }
        }

        public async Task<ApplicationNotificationProfile> DeactivateApplicationProfileAsync(string applicationName)
        {
            try
            {
                var profile = await _profileRepository.GetByNameAsync(applicationName);
                if (profile == null)
                {
                    throw new InvalidOperationException($"Application profile '{applicationName}' not found");
                }

                if (profile.IsActive)
                {
                    await _profileRepository.DeactivateAsync(profile.ApplicationName);
                    profile.IsActive = false;
                    
                }

                return profile;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deactivating application profile: {ex.Message}", ex);
                throw;
            }
        }

        public async Task<IEnumerable<ApplicationSubscriptionInfo>> GetAvailableApplicationsForUserAsync(string username)
        {
            try
            {
                var activeProfiles = await _profileRepository.GetActiveProfilesAsync();
                var userSettings = await _userAppSettingsRepository.GetByUsernameAsync(username);
                var userSettingsDict = userSettings.ToDictionary(s => s.ApplicationName);

                var subscriptionInfos = new List<ApplicationSubscriptionInfo>();

                foreach (var profile in activeProfiles)
                {
                    var info = new ApplicationSubscriptionInfo
                    {
                        ApplicationProfile = profile,
                        UserSettings = userSettingsDict.GetValueOrDefault(profile.ApplicationName),
                        IsSubscribed = userSettingsDict.GetValueOrDefault(profile.ApplicationName)?.IsSubscribed ?? false
                    };

                    subscriptionInfos.Add(info);
                }

                return subscriptionInfos.OrderBy(i => i.ApplicationProfile.DisplayName);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting available applications for user: {ex.Message}", ex);
                throw;
            }
        }
    }
}