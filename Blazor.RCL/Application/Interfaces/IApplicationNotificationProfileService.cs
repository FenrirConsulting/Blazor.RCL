using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazor.RCL.Domain.Entities.Notifications;
using Blazor.RCL.Application.Models.Notifications;

namespace Blazor.RCL.Application.Interfaces
{
    /// <summary>
    /// Service for managing application notification profiles
    /// </summary>
    public interface IApplicationNotificationProfileService
    {
        /// <summary>
        /// Gets all active application profiles
        /// </summary>
        /// <returns>Collection of active application profiles</returns>
        Task<IEnumerable<ApplicationNotificationProfile>> GetActiveApplicationProfilesAsync();

        /// <summary>
        /// Gets an application profile by name
        /// </summary>
        /// <param name="applicationName">The application name</param>
        /// <returns>Application profile or null if not found</returns>
        Task<ApplicationNotificationProfile?> GetApplicationProfileAsync(string applicationName);

        /// <summary>
        /// Creates a new application notification profile
        /// </summary>
        /// <param name="request">Profile creation request</param>
        /// <returns>Created application profile</returns>
        Task<ApplicationNotificationProfile> CreateApplicationProfileAsync(CreateApplicationProfileRequest request);

        /// <summary>
        /// Updates an existing application profile
        /// </summary>
        /// <param name="applicationName">The application name</param>
        /// <param name="request">Profile update request</param>
        /// <returns>Updated application profile</returns>
        Task<ApplicationNotificationProfile> UpdateApplicationProfileAsync(
            string applicationName, 
            UpdateApplicationProfileRequest request);

        /// <summary>
        /// Gets all users subscribed to an application
        /// </summary>
        /// <param name="applicationName">The application name</param>
        /// <param name="activeOnly">Whether to include only active users</param>
        /// <returns>Collection of subscribed usernames</returns>
        Task<IEnumerable<string>> GetSubscribedUsersAsync(string applicationName, bool activeOnly = true);

        /// <summary>
        /// Gets user count for an application by subscription status
        /// </summary>
        /// <param name="applicationName">The application name</param>
        /// <returns>Statistics about application subscriptions</returns>
        Task<ApplicationSubscriptionStats> GetApplicationSubscriptionStatsAsync(string applicationName);

        /// <summary>
        /// Checks if an alert type is supported by an application
        /// </summary>
        /// <param name="applicationName">The application name</param>
        /// <param name="alertType">The alert type to check</param>
        /// <returns>True if the alert type is supported</returns>
        Task<bool> IsAlertTypeSupportedAsync(string applicationName, AlertType alertType);

        /// <summary>
        /// Activates an application profile
        /// </summary>
        /// <param name="applicationName">The application name</param>
        /// <returns>Updated application profile</returns>
        Task<ApplicationNotificationProfile> ActivateApplicationProfileAsync(string applicationName);

        /// <summary>
        /// Deactivates an application profile
        /// </summary>
        /// <param name="applicationName">The application name</param>
        /// <returns>Updated application profile</returns>
        Task<ApplicationNotificationProfile> DeactivateApplicationProfileAsync(string applicationName);

        /// <summary>
        /// Gets applications a user can subscribe to
        /// </summary>
        /// <param name="username">The username</param>
        /// <returns>Collection of available applications with subscription status</returns>
        Task<IEnumerable<ApplicationSubscriptionInfo>> GetAvailableApplicationsForUserAsync(string username);
    }
}