using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Blazor.RCL.Domain.Entities.Notifications;

namespace Blazor.RCL.Application.Interfaces
{
    /// <summary>
    /// Interface for operations on ApplicationNotificationProfile entities.
    /// </summary>
    public interface IApplicationNotificationProfileRepository
    {
        #region Query Methods

        /// <summary>
        /// Gets an application notification profile by ID.
        /// </summary>
        /// <param name="id">The profile ID.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The application profile if found; otherwise, null.</returns>
        Task<ApplicationNotificationProfile> GetByIdAsync(Guid id, CancellationToken token = default);

        /// <summary>
        /// Gets an application notification profile by application name.
        /// </summary>
        /// <param name="applicationName">The application name.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The application profile if found; otherwise, null.</returns>
        Task<ApplicationNotificationProfile> GetByApplicationNameAsync(string applicationName, CancellationToken token = default);

        /// <summary>
        /// Gets all active application profiles.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A list of active application profiles.</returns>
        Task<IEnumerable<ApplicationNotificationProfile>> GetAllActiveAsync(CancellationToken token = default);

        /// <summary>
        /// Gets application profiles that are enabled by default.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A list of application profiles enabled by default.</returns>
        Task<IEnumerable<ApplicationNotificationProfile>> GetEnabledByDefaultAsync(CancellationToken token = default);

        /// <summary>
        /// Gets application profiles that support a specific alert type.
        /// </summary>
        /// <param name="alertType">The alert type to check for.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A list of application profiles supporting the alert type.</returns>
        Task<IEnumerable<ApplicationNotificationProfile>> GetBySupportedAlertTypeAsync(int alertType, CancellationToken token = default);

        /// <summary>
        /// Checks if an application profile exists.
        /// </summary>
        /// <param name="applicationName">The application name.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>True if the profile exists; otherwise, false.</returns>
        Task<bool> ExistsAsync(string applicationName, CancellationToken token = default);

        /// <summary>
        /// Gets an application notification profile by name.
        /// </summary>
        /// <param name="name">The application name.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The application profile if found; otherwise, null.</returns>
        Task<ApplicationNotificationProfile> GetByNameAsync(string name, CancellationToken token = default);

        /// <summary>
        /// Gets all active application profiles.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A list of active application profiles.</returns>
        Task<IEnumerable<ApplicationNotificationProfile>> GetActiveProfilesAsync(CancellationToken token = default);

        #endregion

        #region Create Methods

        /// <summary>
        /// Creates a new application notification profile.
        /// </summary>
        /// <param name="profile">The profile to create.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The created profile with assigned ID.</returns>
        Task<ApplicationNotificationProfile> CreateAsync(ApplicationNotificationProfile profile, CancellationToken token = default);

        #endregion

        #region Update Methods

        /// <summary>
        /// Updates an existing application notification profile.
        /// </summary>
        /// <param name="profile">The profile to update.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The updated profile.</returns>
        Task<ApplicationNotificationProfile> UpdateAsync(ApplicationNotificationProfile profile, CancellationToken token = default);

        /// <summary>
        /// Updates the supported alert types for an application.
        /// </summary>
        /// <param name="applicationName">The application name.</param>
        /// <param name="supportedAlertTypes">JSON array of supported alert types.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>True if successful; otherwise, false.</returns>
        Task<bool> UpdateSupportedAlertTypesAsync(string applicationName, string supportedAlertTypes, CancellationToken token = default);

        /// <summary>
        /// Updates the default severity filter for an application.
        /// </summary>
        /// <param name="applicationName">The application name.</param>
        /// <param name="defaultSeverityFilter">The new default severity filter.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>True if successful; otherwise, false.</returns>
        Task<bool> UpdateDefaultSeverityFilterAsync(string applicationName, int defaultSeverityFilter, CancellationToken token = default);

        #endregion

        #region Delete Methods

        /// <summary>
        /// Deactivates an application notification profile.
        /// </summary>
        /// <param name="applicationName">The application name.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>True if successful; otherwise, false.</returns>
        Task<bool> DeactivateAsync(string applicationName, CancellationToken token = default);

        #endregion
    }
}