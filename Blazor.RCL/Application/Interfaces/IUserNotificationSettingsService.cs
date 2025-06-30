using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazor.RCL.Domain.Entities.Notifications;
using Blazor.RCL.Application.Models.Notifications;

namespace Blazor.RCL.Application.Interfaces
{
    /// <summary>
    /// Service for managing user notification preferences and settings
    /// </summary>
    public interface IUserNotificationSettingsService
    {
        /// <summary>
        /// Gets or creates user notification settings
        /// </summary>
        /// <param name="username">The username</param>
        /// <returns>User notification settings</returns>
        Task<UserNotificationSettings> GetOrCreateUserSettingsAsync(string username);

        /// <summary>
        /// Updates global notification settings for a user
        /// </summary>
        /// <param name="username">The username</param>
        /// <param name="request">Update request with new settings</param>
        /// <returns>Updated user notification settings</returns>
        Task<UserNotificationSettings> UpdateUserSettingsAsync(string username, UpdateUserNotificationSettingsRequest request);

        /// <summary>
        /// Gets all application-specific settings for a user
        /// </summary>
        /// <param name="username">The username</param>
        /// <returns>Collection of application-specific settings</returns>
        Task<IEnumerable<UserApplicationNotificationSettings>> GetUserApplicationSettingsAsync(string username);

        /// <summary>
        /// Gets user settings for a specific application
        /// </summary>
        /// <param name="username">The username</param>
        /// <param name="applicationName">The application name</param>
        /// <returns>Application-specific settings or null if not found</returns>
        Task<UserApplicationNotificationSettings?> GetUserApplicationSettingsAsync(string username, string applicationName);

        /// <summary>
        /// Updates user settings for a specific application
        /// </summary>
        /// <param name="username">The username</param>
        /// <param name="applicationName">The application name</param>
        /// <param name="request">Update request with new settings</param>
        /// <returns>Updated application-specific settings</returns>
        Task<UserApplicationNotificationSettings> UpdateUserApplicationSettingsAsync(
            string username, 
            string applicationName, 
            UpdateUserApplicationSettingsRequest request);

        /// <summary>
        /// Subscribes a user to notifications from an application
        /// </summary>
        /// <param name="username">The username</param>
        /// <param name="applicationName">The application name</param>
        /// <returns>Updated application-specific settings</returns>
        Task<UserApplicationNotificationSettings> SubscribeToApplicationAsync(string username, string applicationName);

        /// <summary>
        /// Unsubscribes a user from notifications from an application
        /// </summary>
        /// <param name="username">The username</param>
        /// <param name="applicationName">The application name</param>
        /// <returns>Updated application-specific settings</returns>
        Task<UserApplicationNotificationSettings> UnsubscribeFromApplicationAsync(string username, string applicationName);

        /// <summary>
        /// Gets a summary of all notification settings for a user
        /// </summary>
        /// <param name="username">The username</param>
        /// <returns>Comprehensive view of user's notification preferences</returns>
        Task<UserNotificationSettingsSummary> GetUserNotificationSummaryAsync(string username);

        /// <summary>
        /// Checks if a user should receive a notification based on their settings
        /// </summary>
        /// <param name="username">The username</param>
        /// <param name="notification">The notification to check</param>
        /// <returns>Tuple indicating if user should receive it and through which channels</returns>
        Task<(bool shouldReceive, DeliveryChannel channels)> ShouldUserReceiveNotificationAsync(
            string username, 
            NotificationMessage notification);

        /// <summary>
        /// Initializes default application settings for a new user
        /// </summary>
        /// <param name="username">The username</param>
        /// <returns>Number of application settings created</returns>
        Task<int> InitializeDefaultApplicationSettingsAsync(string username);

        /// <summary>
        /// Validates if current time is within user's quiet hours
        /// </summary>
        /// <param name="username">The username</param>
        /// <returns>True if currently in quiet hours</returns>
        Task<bool> IsInQuietHoursAsync(string username);
    }
}