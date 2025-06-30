using System.Collections.Concurrent;
using System.Security.Claims;
using System.Text.Json;
using Blazor.RCL.Application.Interfaces;
using Blazor.RCL.Domain.Entities.Configuration;
using Blazor.RCL.Infrastructure.Services.Interfaces;
using Blazor.RCL.NLog.LogService.Interface;

namespace Blazor.RCL.Infrastructure.Services
{
    /// <summary>
    /// Service for managing user preferences and settings.
    /// </summary>
    public class UserSettingsService : IUserSettingsService
    {
        #region Fields

        private readonly IUserSettingsRepository _repository;
        private readonly ILogHelper _logger;
        private readonly ConcurrentDictionary<string, UserSettings> _userSettingsCache = new ConcurrentDictionary<string, UserSettings>();

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the UserSettingsService class.
        /// </summary>
        /// <param name="repository">Repository for user settings persistence.</param>
        /// <param name="logger">Logging service.</param>
        public UserSettingsService(IUserSettingsRepository repository, ILogHelper logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Retrieves user settings for the given principal.
        /// </summary>
        /// <param name="principal">The ClaimsPrincipal representing the user.</param>
        /// <returns>The user's settings, or default settings if the user is not authenticated.</returns>
        public async Task<UserSettings> GetUserSettingsAsync(ClaimsPrincipal principal)
        {
            try
            {
                if (principal.Identity?.IsAuthenticated == true)
                {
                    var username = principal.FindFirst(ClaimTypes.Name)?.Value;
                    if (!string.IsNullOrEmpty(username))
                    {
                        var settings = await GetOrCreateUserSettingsAsync(username);
                        
                        // Check if roles need to be captured (first time or stale)
                        if (string.IsNullOrEmpty(settings.Roles) ||
                            !settings.RolesLastUpdated.HasValue ||
                            settings.RolesLastUpdated.Value < DateTime.UtcNow.AddDays(-1))
                        {
                            // Update roles from claims
                            await UpdateUserRolesFromClaimsAsync(principal);
                            // Refresh settings from cache after update
                            settings = await GetOrCreateUserSettingsAsync(username);
                        }
                        
                        return settings;
                    }
                }
                return new UserSettings(); // Return default settings for unauthenticated users
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while getting user settings.", ex);
                return new UserSettings();
            }
        }

        /// <summary>
        /// Retrieves or creates user settings for the given username.
        /// </summary>
        /// <param name="username">The username of the user.</param>
        /// <returns>The user's settings, either from cache or newly created.</returns>
        public async Task<UserSettings> GetOrCreateUserSettingsAsync(string username)
        {
            if (!_userSettingsCache.TryGetValue(username, out var cachedSettings))
            {
                var fetchedSettings = await _repository.GetUserSettingsAsync(username);
                if (fetchedSettings == null)
                {
                    fetchedSettings = await _repository.CreateUserSettingsAsync(username);
                }
                cachedSettings = _userSettingsCache.GetOrAdd(username, fetchedSettings!);
            }
            return cachedSettings;
        }

        /// <summary>
        /// Updates the given user settings.
        /// </summary>
        /// <param name="settings">The user settings to update.</param>
        public async Task UpdateUserSettingsAsync(UserSettings settings)
        {
            try
            {
                await _repository.UpdateUserSettingsAsync(settings);
                _userSettingsCache[settings.Username!] = settings;
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while updating user settings.", ex);
            }
        }

        /// <summary>
        /// Retrieves the dark mode setting for the given principal.
        /// </summary>
        /// <param name="principal">The ClaimsPrincipal representing the user.</param>
        /// <returns>True if dark mode is enabled, false otherwise.</returns>
        public async Task<bool> GetIsDarkModeAsync(ClaimsPrincipal principal)
        {
            var settings = await GetUserSettingsAsync(principal);
            return settings.DarkMode == "true";
        }

        /// <summary>
        /// Sets the dark mode setting for the given principal.
        /// </summary>
        /// <param name="principal">The ClaimsPrincipal representing the user.</param>
        /// <param name="value">The dark mode value to set.</param>
        public async Task SetIsDarkModeAsync(ClaimsPrincipal principal, bool value)
        {
            var settings = await GetUserSettingsAsync(principal);
            settings.DarkMode = value.ToString().ToLower();
            await UpdateUserSettingsAsync(settings);
        }

        /// <summary>
        /// Retrieves additional settings for the given principal.
        /// </summary>
        /// <param name="principal">The ClaimsPrincipal representing the user.</param>
        /// <returns>The additional settings as a JSON string, or an empty JSON object if not set.</returns>
        public async Task<string> GetAdditionalSettingsAsync(ClaimsPrincipal principal)
        {
            var settings = await GetUserSettingsAsync(principal);
            return settings.AdditionalSettings ?? "{}";
        }

        /// <summary>
        /// Sets additional settings for the given principal.
        /// </summary>
        /// <param name="principal">The ClaimsPrincipal representing the user.</param>
        /// <param name="value">The additional settings JSON string to set.</param>
        public async Task SetAdditionalSettingsAsync(ClaimsPrincipal principal, string value)
        {
            var settings = await GetUserSettingsAsync(principal);
            settings.AdditionalSettings = value;
            await UpdateUserSettingsAsync(settings);
        }

        /// <summary>
        /// Gets user roles from UserSettings, updating from claims if needed.
        /// </summary>
        public async Task<List<string>> GetUserRolesAsync(ClaimsPrincipal principal)
        {
            var settings = await GetUserSettingsAsync(principal);

            // Check if we need to update roles (first time or stale)
            if (string.IsNullOrEmpty(settings.Roles) ||
                !settings.RolesLastUpdated.HasValue ||
                settings.RolesLastUpdated.Value < DateTime.UtcNow.AddDays(-1)) // Refresh daily
            {
                return await UpdateUserRolesFromClaimsAsync(principal);
            }

            // Deserialize and return existing roles
            try
            {
                return JsonSerializer.Deserialize<List<string>>(settings.Roles) ?? new List<string>();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deserializing roles for user {settings.Username}: {ex.Message}", ex);
                return new List<string>();
            }
        }

        /// <summary>
        /// Updates user roles from authentication claims.
        /// </summary>
        public async Task<List<string>> UpdateUserRolesFromClaimsAsync(ClaimsPrincipal principal)
        {
            // Get username from principal to avoid circular call
            var username = principal.FindFirst(ClaimTypes.Name)?.Value;
            if (string.IsNullOrEmpty(username))
            {
                return new List<string>();
            }

            // Get settings directly without going through GetUserSettingsAsync
            var settings = await GetOrCreateUserSettingsAsync(username);

            // Extract roles from claims
            var roles = principal.Claims
                .Where(c => c.Type == ClaimTypes.Role || c.Type == "role" || c.Type == "roles")
                .Select(c => c.Value)
                .Distinct()
                .OrderBy(r => r)
                .ToList();

            // Update settings
            settings.Roles = JsonSerializer.Serialize(roles);
            settings.RolesLastUpdated = DateTime.UtcNow;

            await UpdateUserSettingsAsync(settings);

            _logger.LogInfo($"Updated roles for user {settings.Username}: {string.Join(", ", roles)}",
                "RolesUpdated",
                new { Username = settings.Username, RoleCount = roles.Count });

            return roles;
        }

        /// <summary>
        /// Gets user roles directly from stored settings without refresh.
        /// </summary>
        public async Task<List<string>> GetStoredUserRolesAsync(string username)
        {
            var settings = await GetOrCreateUserSettingsAsync(username);

            if (string.IsNullOrEmpty(settings.Roles))
            {
                return new List<string>();
            }

            try
            {
                return JsonSerializer.Deserialize<List<string>>(settings.Roles) ?? new List<string>();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deserializing stored roles for user {username}: {ex.Message}", ex);
                return new List<string>();
            }
        }

        #endregion
    }
}