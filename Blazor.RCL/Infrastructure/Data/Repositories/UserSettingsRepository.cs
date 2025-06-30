using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Blazor.RCL.Application.Interfaces;
using Blazor.RCL.Domain.Entities.Configuration;
using Blazor.RCL.NLog.LogService.Interface;
using Microsoft.EntityFrameworkCore;

namespace Blazor.RCL.Infrastructure.Data.Repositories
{
    /// <summary>
    /// Repository implementation for UserSettings operations using Entity Framework Core.
    /// This class handles the data access logic for user settings in the application.
    /// </summary>
    public class UserSettingsRepository : IUserSettingsRepository
    {
        #region Fields

        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly ILogHelper _logger;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the UserSettingsRepository class.
        /// </summary>
        /// <param name="contextFactory">The factory for creating database contexts.</param>
        /// <param name="logger">The logger for error logging.</param>
        /// <exception cref="ArgumentNullException">Thrown if contextFactory or logger is null.</exception>
        public UserSettingsRepository(IDbContextFactory<AppDbContext> contextFactory, ILogHelper logger)
        {
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Retrieves user settings for a specified username.
        /// </summary>
        /// <param name="username">The username to retrieve settings for.</param>
        /// <returns>The UserSettings if found; otherwise, null.</returns>
        public async Task<UserSettings?> GetUserSettingsAsync(string username)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync();
                return await context.UserSettings
                    .FirstOrDefaultAsync(x => x.Username == username);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while getting user settings for username: {username}", ex);
                throw;
            }
        }

        /// <summary>
        /// Creates new user settings for a specified username.
        /// </summary>
        /// <param name="username">The username to create settings for.</param>
        /// <returns>The newly created UserSettings.</returns>
        public async Task<UserSettings?> CreateUserSettingsAsync(string username)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync();
                var settings = new UserSettings
                {
                    Username = username,
                    DarkMode = "false",
                    AdditionalSettings = "{}"
                };
                context.UserSettings.Add(settings);
                await context.SaveChangesAsync();
                return settings;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while creating user settings for username: {username}", ex);
                throw;
            }
        }

        /// <summary>
        /// Updates existing user settings.
        /// </summary>
        /// <param name="userSettings">The UserSettings to update.</param>
        public async Task UpdateUserSettingsAsync(UserSettings userSettings)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync();
                context.UserSettings.Update(userSettings);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while updating user settings for username: {userSettings.Username}", ex);
                throw;
            }
        }

        /// <summary>
        /// Gets user settings by username.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <returns>The user settings if found; otherwise, null.</returns>
        public async Task<UserSettings?> GetByUsernameAsync(string username)
        {
            return await GetUserSettingsAsync(username);
        }

        /// <summary>
        /// Gets all usernames that have a specific role.
        /// </summary>
        /// <param name="role">The role to search for.</param>
        /// <returns>A list of usernames that have the specified role.</returns>
        public async Task<IEnumerable<string>> GetUsernamesWithRoleAsync(string role)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync();
                
                // Query all users with non-null roles
                var usersWithRoles = await context.UserSettings
                    .Where(u => !string.IsNullOrEmpty(u.Roles))
                    .Select(u => new { u.Username, u.Roles })
                    .ToListAsync();
                
                // Filter in memory to find users with the specific role
                var usernamesWithRole = new List<string>();
                foreach (var user in usersWithRoles)
                {
                    try
                    {
                        // Deserialize the roles JSON array
                        var roles = JsonSerializer.Deserialize<List<string>>(user.Roles);
                        if (roles != null && roles.Contains(role, StringComparer.OrdinalIgnoreCase))
                        {
                            usernamesWithRole.Add(user.Username);
                        }
                    }
                    catch (JsonException)
                    {
                        // Skip users with invalid JSON in roles field
                        _logger.LogMessage($"Invalid JSON in Roles field for user: {user.Username}");
                    }
                }
                
                _logger.LogMessage($"Found {usernamesWithRole.Count} users with role: {role}");
                return usernamesWithRole;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while getting usernames with role: {role}", ex);
                throw;
            }
        }

        #endregion
    }
}