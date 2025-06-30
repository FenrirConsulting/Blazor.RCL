using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Blazor.RCL.Application.Interfaces;
using Blazor.RCL.Application.Models.Notifications;
using Blazor.RCL.Domain.Entities.Notifications;
using Blazor.RCL.NLog.LogService.Interface;

namespace Blazor.RCL.Infrastructure.Services
{
    /// <summary>
    /// Service implementation for managing user notification preferences and settings
    /// </summary>
    public class UserNotificationSettingsService : IUserNotificationSettingsService
    {
        private readonly IUserNotificationSettingsRepository _userSettingsRepository;
        private readonly IUserApplicationNotificationSettingsRepository _appSettingsRepository;
        private readonly IApplicationNotificationProfileRepository _appProfileRepository;
        private readonly IUserSettingsRepository _userRepository;
        private readonly ILogHelper _logger;

        public UserNotificationSettingsService(
            IUserNotificationSettingsRepository userSettingsRepository,
            IUserApplicationNotificationSettingsRepository appSettingsRepository,
            IApplicationNotificationProfileRepository appProfileRepository,
            IUserSettingsRepository userRepository,
            ILogHelper logger)
        {
            _userSettingsRepository = userSettingsRepository;
            _appSettingsRepository = appSettingsRepository;
            _appProfileRepository = appProfileRepository;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<UserNotificationSettings> GetOrCreateUserSettingsAsync(string username)
        {
            try
            {
                var settings = await _userSettingsRepository.GetByUsernameAsync(username);
                if (settings != null)
                {
                    return settings;
                }

                // Generate default email from username
                // Clean username if it contains domain prefix (e.g., CORP\username)
                var cleanUsername = CleanUsername(username);
                var email = $"{cleanUsername}@Company.com";

                // Create default settings
                settings = new UserNotificationSettings
                {
                    Id = Guid.NewGuid(),
                    Username = username,
                    EmailAddress = email,
                    EnableEmailNotifications = true,
                    GlobalEmailFrequency = (int)EmailFrequency.Immediate,
                    TimeZone = "America/New_York",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await _userSettingsRepository.CreateAsync(settings);
                
                // Initialize default application settings
                await InitializeDefaultApplicationSettingsAsync(username);

                return settings;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting/creating user settings: {ex.Message}", ex);
                throw;
            }
        }

        public async Task<UserNotificationSettings> UpdateUserSettingsAsync(
            string username, 
            UpdateUserNotificationSettingsRequest request)
        {
            try
            {
                var settings = await GetOrCreateUserSettingsAsync(username);

                if (request.EmailAddress != null)
                {
                    if (!IsValidEmail(request.EmailAddress))
                    {
                        throw new ArgumentException($"Invalid email address format: {request.EmailAddress}. Email addresses cannot contain backslashes or other invalid characters.");
                    }
                    settings.EmailAddress = request.EmailAddress;
                }
                
                if (request.EnableEmailNotifications.HasValue)
                    settings.EnableEmailNotifications = request.EnableEmailNotifications.Value;
                
                if (request.GlobalEmailFrequency.HasValue)
                    settings.GlobalEmailFrequency = (int)request.GlobalEmailFrequency.Value;
                
                if (request.QuietHoursStart.HasValue)
                    settings.QuietHoursStart = request.QuietHoursStart;
                
                if (request.QuietHoursEnd.HasValue)
                    settings.QuietHoursEnd = request.QuietHoursEnd;
                
                if (request.TimeZone != null)
                    settings.TimeZone = request.TimeZone;

                settings.UpdatedAt = DateTime.UtcNow;
                await _userSettingsRepository.UpdateAsync(settings);

                return settings;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating user settings: {ex.Message}", ex);
                throw;
            }
        }

        public async Task<IEnumerable<UserApplicationNotificationSettings>> GetUserApplicationSettingsAsync(string username)
        {
            try
            {
                return await _appSettingsRepository.GetByUsernameAsync(username);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting user application settings: {ex.Message}", ex);
                throw;
            }
        }

        public async Task<UserApplicationNotificationSettings?> GetUserApplicationSettingsAsync(
            string username, 
            string applicationName)
        {
            try
            {
                return await _appSettingsRepository.GetByUserAndApplicationAsync(username, applicationName);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting user application settings: {ex.Message}", ex);
                throw;
            }
        }

        public async Task<UserApplicationNotificationSettings> UpdateUserApplicationSettingsAsync(
            string username, 
            string applicationName, 
            UpdateUserApplicationSettingsRequest request)
        {
            try
            {
                var settings = await _appSettingsRepository.GetByUserAndApplicationAsync(username, applicationName);
                if (settings == null)
                {
                    // Create new settings from application defaults
                    var appProfile = await _appProfileRepository.GetByNameAsync(applicationName);
                    if (appProfile == null)
                    {
                        throw new InvalidOperationException($"Application '{applicationName}' not found");
                    }

                    settings = await _appSettingsRepository.CreateFromApplicationDefaultsAsync(username, applicationName);
                }

                // Update settings
                if (request.IsSubscribed.HasValue)
                    settings.IsSubscribed = request.IsSubscribed.Value;
                
                if (request.EnableRealTimeNotifications.HasValue)
                    settings.EnableRealTimeNotifications = request.EnableRealTimeNotifications.Value;
                
                if (request.EnableEmailNotifications.HasValue)
                    settings.EnableEmailNotifications = request.EnableEmailNotifications.Value;
                
                if (request.EmailFrequency.HasValue)
                    settings.EmailFrequency = (int)request.EmailFrequency.Value;
                
                if (request.SeverityFilter.HasValue)
                    settings.SeverityFilter = (int)request.SeverityFilter.Value;
                
                if (request.AlertTypeFilter != null)
                    settings.AlertTypeFilter = JsonSerializer.Serialize(request.AlertTypeFilter);
                
                if (request.EnableCriticalAlerts.HasValue)
                    settings.EnableCriticalAlerts = request.EnableCriticalAlerts.Value;

                settings.UpdatedAt = DateTime.UtcNow;
                await _appSettingsRepository.UpdateAsync(settings);

                return settings;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating user application settings: {ex.Message}", ex);
                throw;
            }
        }

        public async Task<UserApplicationNotificationSettings> SubscribeToApplicationAsync(
            string username, 
            string applicationName)
        {
            try
            {
                var request = new UpdateUserApplicationSettingsRequest
                {
                    IsSubscribed = true,
                    EnableRealTimeNotifications = true,
                    EnableEmailNotifications = true
                };

                return await UpdateUserApplicationSettingsAsync(username, applicationName, request);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error subscribing to application: {ex.Message}", ex);
                throw;
            }
        }

        public async Task<UserApplicationNotificationSettings> UnsubscribeFromApplicationAsync(
            string username, 
            string applicationName)
        {
            try
            {
                var request = new UpdateUserApplicationSettingsRequest
                {
                    IsSubscribed = false
                };

                return await UpdateUserApplicationSettingsAsync(username, applicationName, request);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error unsubscribing from application: {ex.Message}", ex);
                throw;
            }
        }

        public async Task<UserNotificationSettingsSummary> GetUserNotificationSummaryAsync(string username)
        {
            try
            {
                var globalSettings = await GetOrCreateUserSettingsAsync(username);
                var appSettings = await _appSettingsRepository.GetByUsernameAsync(username);
                
                var summary = new UserNotificationSettingsSummary
                {
                    GlobalSettings = globalSettings,
                    ApplicationSettings = new List<ApplicationSettingsSummary>()
                };

                foreach (var appSetting in appSettings)
                {
                    var appProfile = await _appProfileRepository.GetByNameAsync(appSetting.ApplicationName);
                    if (appProfile != null)
                    {
                        summary.ApplicationSettings.Add(new ApplicationSettingsSummary
                        {
                            ApplicationProfile = appProfile,
                            Settings = appSetting
                        });
                    }
                }

                // Calculate quiet hours status
                summary.QuietHoursStatus = CalculateQuietHoursStatus(globalSettings);

                return summary;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting user notification summary: {ex.Message}", ex);
                throw;
            }
        }

        public async Task<(bool shouldReceive, DeliveryChannel channels)> ShouldUserReceiveNotificationAsync(
            string username, 
            NotificationMessage notification)
        {
            try
            {
                // Get user settings
                var userSettings = await GetOrCreateUserSettingsAsync(username);
                var appSettings = await _appSettingsRepository.GetByUserAndApplicationAsync(
                    username, 
                    notification.SourceApplication);

                // Check if user is subscribed to this application
                if (appSettings == null || !appSettings.IsSubscribed)
                {
                    return (false, 0);
                }

                // Check severity filter
                if (notification.Severity < appSettings.SeverityFilter && 
                    !(notification.Severity == (int)NotificationSeverity.Critical && appSettings.EnableCriticalAlerts))
                {
                    return (false, 0);
                }

                // Check alert type filter
                if (!string.IsNullOrEmpty(appSettings.AlertTypeFilter))
                {
                    var allowedTypes = JsonSerializer.Deserialize<List<AlertType>>(appSettings.AlertTypeFilter);
                    if (allowedTypes?.Contains((AlertType)notification.AlertType) == false)
                    {
                        return (false, 0);
                    }
                }

                // Check quiet hours (unless critical)
                if (notification.Severity != (int)NotificationSeverity.Critical)
                {
                    if (await IsInQuietHoursAsync(username))
                    {
                        return (false, 0);
                    }
                }

                // Determine delivery channels
                var channels = DeliveryChannel.None;
                
                if (appSettings.EnableRealTimeNotifications)
                {
                    channels |= DeliveryChannel.SignalR;
                }

                if (appSettings.EnableEmailNotifications && userSettings.EnableEmailNotifications)
                {
                    channels |= DeliveryChannel.Email;
                }

                return (channels != DeliveryChannel.None, channels);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error checking if user should receive notification: {ex.Message}", ex);
                return (false, 0);
            }
        }

        public async Task<int> InitializeDefaultApplicationSettingsAsync(string username)
        {
            try
            {
                var activeProfiles = await _appProfileRepository.GetActiveProfilesAsync();
                var existingSettings = await _appSettingsRepository.GetByUsernameAsync(username);
                var existingApps = existingSettings.Select(s => s.ApplicationName).ToHashSet();

                var createdCount = 0;
                foreach (var profile in activeProfiles.Where(p => p.EnabledByDefault && !existingApps.Contains(p.ApplicationName)))
                {
                    await _appSettingsRepository.CreateFromApplicationDefaultsAsync(username, profile.ApplicationName);
                    createdCount++;
                }

                if (createdCount > 0)
                {
              
                }

                return createdCount;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error initializing default application settings: {ex.Message}", ex);
                throw;
            }
        }

        public async Task<bool> IsInQuietHoursAsync(string username)
        {
            try
            {
                var settings = await GetOrCreateUserSettingsAsync(username);
                
                if (!settings.QuietHoursStart.HasValue || !settings.QuietHoursEnd.HasValue)
                {
                    return false;
                }

                var userTimeZone = TimeZoneInfo.FindSystemTimeZoneById(settings.TimeZone ?? "America/New_York");
                var userLocalTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, userTimeZone);
                var currentTime = userLocalTime.TimeOfDay;

                var start = settings.QuietHoursStart.Value;
                var end = settings.QuietHoursEnd.Value;

                // Handle quiet hours that span midnight
                if (start > end)
                {
                    return currentTime >= start || currentTime <= end;
                }
                else
                {
                    return currentTime >= start && currentTime <= end;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error checking quiet hours: {ex.Message}", ex);
                return false;
            }
        }

        private QuietHoursStatus CalculateQuietHoursStatus(UserNotificationSettings settings)
        {
            var status = new QuietHoursStatus
            {
                IsConfigured = settings.QuietHoursStart.HasValue && settings.QuietHoursEnd.HasValue
            };

            if (!status.IsConfigured)
            {
                return status;
            }

            try
            {
                var userTimeZone = TimeZoneInfo.FindSystemTimeZoneById(settings.TimeZone ?? "America/New_York");
                var userLocalTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, userTimeZone);
                status.UserLocalTime = userLocalTime;

                var currentTime = userLocalTime.TimeOfDay;
                var start = settings.QuietHoursStart!.Value;
                var end = settings.QuietHoursEnd!.Value;

                // Handle quiet hours that span midnight
                if (start > end)
                {
                    status.IsCurrentlyActive = currentTime >= start || currentTime <= end;
                    
                    if (status.IsCurrentlyActive)
                    {
                        // Calculate when quiet hours end
                        if (currentTime >= start)
                        {
                            // We're in the evening part, quiet hours end tomorrow
                            status.NextTransitionTime = userLocalTime.Date.AddDays(1).Add(end);
                        }
                        else
                        {
                            // We're in the morning part, quiet hours end today
                            status.NextTransitionTime = userLocalTime.Date.Add(end);
                        }
                    }
                    else
                    {
                        // Calculate when quiet hours start
                        status.NextTransitionTime = userLocalTime.Date.Add(start);
                        if (currentTime > start)
                        {
                            status.NextTransitionTime = status.NextTransitionTime.Value.AddDays(1);
                        }
                    }
                }
                else
                {
                    status.IsCurrentlyActive = currentTime >= start && currentTime <= end;
                    
                    if (status.IsCurrentlyActive)
                    {
                        // Quiet hours end today
                        status.NextTransitionTime = userLocalTime.Date.Add(end);
                    }
                    else
                    {
                        // Calculate when quiet hours start
                        if (currentTime < start)
                        {
                            status.NextTransitionTime = userLocalTime.Date.Add(start);
                        }
                        else
                        {
                            status.NextTransitionTime = userLocalTime.Date.AddDays(1).Add(start);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error calculating quiet hours status: {ex.Message}", ex);
            }

            return status;
        }

        /// <summary>
        /// Validates if an email address is in a valid format
        /// </summary>
        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            // Check for invalid characters like backslashes
            if (email.Contains('\\'))
            {
                _logger.LogWarn($"Email address contains invalid backslash character: {email}");
                return false;
            }

            try
            {
                // Basic email validation pattern
                var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
                var regex = new Regex(emailPattern, RegexOptions.IgnoreCase);
                return regex.IsMatch(email);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error validating email address: {ex.Message}", ex);
                return false;
            }
        }

        /// <summary>
        /// Cleans a username by removing domain prefix (e.g., CORP\username becomes username)
        /// </summary>
        private string CleanUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return username;

            // Check if username contains domain\username format
            var backslashIndex = username.LastIndexOf('\\');
            if (backslashIndex >= 0 && backslashIndex < username.Length - 1)
            {
                var cleanedUsername = username.Substring(backslashIndex + 1);
                return cleanedUsername;
            }

            return username;
        }
    }
}