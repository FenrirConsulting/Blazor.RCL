using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Blazor.RCL.Domain.Entities.Notifications;

namespace Blazor.RCL.Application.Interfaces
{
    /// <summary>
    /// Interface for operations on UserNotificationSettings entities.
    /// </summary>
    public interface IUserNotificationSettingsRepository
    {
        #region Query Methods

        /// <summary>
        /// Gets user notification settings by username.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The user notification settings if found; otherwise, null.</returns>
        Task<UserNotificationSettings> GetByUsernameAsync(string username, CancellationToken token = default);

        /// <summary>
        /// Gets user notification settings by ID.
        /// </summary>
        /// <param name="id">The settings ID.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The user notification settings if found; otherwise, null.</returns>
        Task<UserNotificationSettings> GetByIdAsync(Guid id, CancellationToken token = default);

        /// <summary>
        /// Gets all active user notification settings.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A list of active user notification settings.</returns>
        Task<IEnumerable<UserNotificationSettings>> GetAllActiveAsync(CancellationToken token = default);

        /// <summary>
        /// Gets users with email notifications enabled.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A list of users with email notifications enabled.</returns>
        Task<IEnumerable<UserNotificationSettings>> GetEmailEnabledUsersAsync(CancellationToken token = default);

        /// <summary>
        /// Gets users by email frequency.
        /// </summary>
        /// <param name="frequency">The email frequency.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A list of users with the specified email frequency.</returns>
        Task<IEnumerable<UserNotificationSettings>> GetByEmailFrequencyAsync(int frequency, CancellationToken token = default);

        /// <summary>
        /// Checks if user notification settings exist for a username.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>True if settings exist; otherwise, false.</returns>
        Task<bool> ExistsAsync(string username, CancellationToken token = default);

        #endregion

        #region Create Methods

        /// <summary>
        /// Creates new user notification settings.
        /// </summary>
        /// <param name="settings">The settings to create.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The created settings with assigned ID.</returns>
        Task<UserNotificationSettings> CreateAsync(UserNotificationSettings settings, CancellationToken token = default);

        /// <summary>
        /// Creates default notification settings for a new user.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="emailAddress">The user's email address.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The created default settings.</returns>
        Task<UserNotificationSettings> CreateDefaultAsync(string username, string emailAddress, CancellationToken token = default);

        #endregion

        #region Update Methods

        /// <summary>
        /// Updates existing user notification settings.
        /// </summary>
        /// <param name="settings">The settings to update.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The updated settings.</returns>
        Task<UserNotificationSettings> UpdateAsync(UserNotificationSettings settings, CancellationToken token = default);

        /// <summary>
        /// Updates user's email preferences.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="enableEmail">Whether to enable email notifications.</param>
        /// <param name="frequency">The email frequency.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>True if successful; otherwise, false.</returns>
        Task<bool> UpdateEmailPreferencesAsync(string username, bool enableEmail, int frequency, CancellationToken token = default);

        /// <summary>
        /// Updates user's quiet hours settings.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="quietHoursStart">Start time for quiet hours.</param>
        /// <param name="quietHoursEnd">End time for quiet hours.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>True if successful; otherwise, false.</returns>
        Task<bool> UpdateQuietHoursAsync(string username, TimeSpan? quietHoursStart, TimeSpan? quietHoursEnd, CancellationToken token = default);

        #endregion

        #region Delete Methods

        /// <summary>
        /// Deactivates user notification settings.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>True if successful; otherwise, false.</returns>
        Task<bool> DeactivateAsync(string username, CancellationToken token = default);

        #endregion
    }
}