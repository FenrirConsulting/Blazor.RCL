using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Blazor.RCL.Domain.Entities.Notifications;

namespace Blazor.RCL.Application.Interfaces
{
    /// <summary>
    /// Interface for operations on UserApplicationNotificationSettings entities.
    /// </summary>
    public interface IUserApplicationNotificationSettingsRepository
    {
        #region Query Methods

        /// <summary>
        /// Gets user application notification settings by ID.
        /// </summary>
        /// <param name="id">The settings ID.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The settings if found; otherwise, null.</returns>
        Task<UserApplicationNotificationSettings> GetByIdAsync(Guid id, CancellationToken token = default);

        /// <summary>
        /// Gets user application notification settings by username and application.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="applicationName">The application name.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The settings if found; otherwise, null.</returns>
        Task<UserApplicationNotificationSettings> GetByUserAndApplicationAsync(string username, string applicationName, CancellationToken token = default);

        /// <summary>
        /// Gets all application settings for a user.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A list of application settings for the user.</returns>
        Task<IEnumerable<UserApplicationNotificationSettings>> GetByUsernameAsync(string username, CancellationToken token = default);

        /// <summary>
        /// Gets all user settings for an application.
        /// </summary>
        /// <param name="applicationName">The application name.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A list of user settings for the application.</returns>
        Task<IEnumerable<UserApplicationNotificationSettings>> GetByApplicationAsync(string applicationName, CancellationToken token = default);

        /// <summary>
        /// Gets subscribed users for an application.
        /// </summary>
        /// <param name="applicationName">The application name.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A list of subscribed user settings.</returns>
        Task<IEnumerable<UserApplicationNotificationSettings>> GetSubscribedUsersAsync(string applicationName, CancellationToken token = default);

        /// <summary>
        /// Gets users subscribed to an application with specific severity filter.
        /// </summary>
        /// <param name="applicationName">The application name.</param>
        /// <param name="minSeverity">The minimum severity level.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A list of user settings matching the criteria.</returns>
        Task<IEnumerable<UserApplicationNotificationSettings>> GetSubscribedUsersBySeverityAsync(string applicationName, int minSeverity, CancellationToken token = default);

        /// <summary>
        /// Checks if user is subscribed to an application.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="applicationName">The application name.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>True if subscribed; otherwise, false.</returns>
        Task<bool> IsSubscribedAsync(string username, string applicationName, CancellationToken token = default);

        #endregion

        #region Create Methods

        /// <summary>
        /// Creates new user application notification settings.
        /// </summary>
        /// <param name="settings">The settings to create.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The created settings with assigned ID.</returns>
        Task<UserApplicationNotificationSettings> CreateAsync(UserApplicationNotificationSettings settings, CancellationToken token = default);

        /// <summary>
        /// Creates default settings for a user and application.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="applicationName">The application name.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The created default settings.</returns>
        Task<UserApplicationNotificationSettings> CreateDefaultAsync(string username, string applicationName, CancellationToken token = default);

        /// <summary>
        /// Creates settings for multiple applications for a user.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="applicationNames">The application names.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The created settings.</returns>
        Task<IEnumerable<UserApplicationNotificationSettings>> CreateBatchForUserAsync(string username, IEnumerable<string> applicationNames, CancellationToken token = default);

        /// <summary>
        /// Creates user application settings from application defaults.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="applicationName">The application name.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The created settings based on application defaults.</returns>
        Task<UserApplicationNotificationSettings> CreateFromApplicationDefaultsAsync(string username, string applicationName, CancellationToken token = default);

        #endregion

        #region Update Methods

        /// <summary>
        /// Updates existing user application notification settings.
        /// </summary>
        /// <param name="settings">The settings to update.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The updated settings.</returns>
        Task<UserApplicationNotificationSettings> UpdateAsync(UserApplicationNotificationSettings settings, CancellationToken token = default);

        /// <summary>
        /// Updates subscription status for a user and application.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="applicationName">The application name.</param>
        /// <param name="isSubscribed">The subscription status.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>True if successful; otherwise, false.</returns>
        Task<bool> UpdateSubscriptionAsync(string username, string applicationName, bool isSubscribed, CancellationToken token = default);

        /// <summary>
        /// Updates notification preferences for a user and application.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="applicationName">The application name.</param>
        /// <param name="enableRealTime">Enable real-time notifications.</param>
        /// <param name="enableEmail">Enable email notifications.</param>
        /// <param name="emailFrequency">Email frequency preference.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>True if successful; otherwise, false.</returns>
        Task<bool> UpdateNotificationPreferencesAsync(
            string username, 
            string applicationName, 
            bool enableRealTime, 
            bool enableEmail, 
            int emailFrequency, 
            CancellationToken token = default);

        #endregion

        #region Delete Methods

        /// <summary>
        /// Deletes user application notification settings.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="applicationName">The application name.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>True if successful; otherwise, false.</returns>
        Task<bool> DeleteAsync(string username, string applicationName, CancellationToken token = default);

        /// <summary>
        /// Deletes all application settings for a user.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The number of deleted settings.</returns>
        Task<int> DeleteAllForUserAsync(string username, CancellationToken token = default);

        #endregion
    }
}