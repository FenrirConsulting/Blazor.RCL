﻿using System.Security.Claims;
using System.Threading.Tasks;
using Blazor.RCL.Domain.Entities.Configuration;

namespace Blazor.RCL.Infrastructure.Services.Interfaces
{
    /// <summary>
    /// Provides methods to manage user settings.
    /// </summary>
    public interface IUserSettingsService
    {
        /// <summary>
        /// Retrieves user settings for the given principal.
        /// </summary>
        /// <param name="principal">The ClaimsPrincipal representing the user.</param>
        /// <returns>The user's settings.</returns>
        Task<UserSettings> GetUserSettingsAsync(ClaimsPrincipal principal);

        /// <summary>
        /// Retrieves or creates user settings for the given username.
        /// </summary>
        /// <param name="username">The username of the user.</param>
        /// <returns>The user's settings, either existing or newly created.</returns>
        Task<UserSettings> GetOrCreateUserSettingsAsync(string username);

        /// <summary>
        /// Updates the given user settings.
        /// </summary>
        /// <param name="settings">The user settings to update.</param>
        Task UpdateUserSettingsAsync(UserSettings settings);

        /// <summary>
        /// Retrieves the dark mode setting for the given principal.
        /// </summary>
        /// <param name="principal">The ClaimsPrincipal representing the user.</param>
        /// <returns>True if dark mode is enabled, false otherwise.</returns>
        Task<bool> GetIsDarkModeAsync(ClaimsPrincipal principal);

        /// <summary>
        /// Sets the dark mode setting for the given principal.
        /// </summary>
        /// <param name="principal">The ClaimsPrincipal representing the user.</param>
        /// <param name="value">The dark mode value to set.</param>
        Task SetIsDarkModeAsync(ClaimsPrincipal principal, bool value);

        /// <summary>
        /// Retrieves additional settings for the given principal.
        /// </summary>
        /// <param name="principal">The ClaimsPrincipal representing the user.</param>
        /// <returns>The additional settings as a JSON string.</returns>
        Task<string> GetAdditionalSettingsAsync(ClaimsPrincipal principal);

        /// <summary>
        /// Sets additional settings for the given principal.
        /// </summary>
        /// <param name="principal">The ClaimsPrincipal representing the user.</param>
        /// <param name="value">The additional settings JSON string to set.</param>
        Task SetAdditionalSettingsAsync(ClaimsPrincipal principal, string value);

        /// <summary>
        /// Gets user roles from UserSettings, updating from claims if needed.
        /// </summary>
        /// <param name="principal">The ClaimsPrincipal representing the user.</param>
        /// <returns>List of role names the user belongs to.</returns>
        Task<List<string>> GetUserRolesAsync(ClaimsPrincipal principal);

        /// <summary>
        /// Updates user roles from authentication claims.
        /// </summary>
        /// <param name="principal">The ClaimsPrincipal representing the user.</param>
        /// <returns>Updated list of role names.</returns>
        Task<List<string>> UpdateUserRolesFromClaimsAsync(ClaimsPrincipal principal);

        /// <summary>
        /// Gets user roles directly from stored settings without refresh.
        /// </summary>
        /// <param name="username">The username to get roles for.</param>
        /// <returns>List of role names from storage.</returns>
        Task<List<string>> GetStoredUserRolesAsync(string username);
    }
}