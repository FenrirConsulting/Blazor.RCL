using System.Collections.Generic;
using Blazor.RCL.Domain.Entities.Notifications;

namespace Blazor.RCL.Application.Models.Notifications
{
    /// <summary>
    /// Request model for creating a new application notification profile
    /// </summary>
    public class CreateApplicationProfileRequest
    {
        /// <summary>
        /// Unique application name/identifier
        /// </summary>
        public string ApplicationName { get; set; } = string.Empty;

        /// <summary>
        /// Display name for the application
        /// </summary>
        public string DisplayName { get; set; } = string.Empty;

        /// <summary>
        /// Description of the application
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Alert types supported by this application
        /// </summary>
        public List<AlertType> SupportedAlertTypes { get; set; } = new();

        /// <summary>
        /// Default minimum severity level for notifications
        /// </summary>
        public NotificationSeverity DefaultSeverityFilter { get; set; } = NotificationSeverity.Info;

        /// <summary>
        /// Whether new users should be subscribed by default
        /// </summary>
        public bool EnabledByDefault { get; set; }

        /// <summary>
        /// Default email template key for this application
        /// </summary>
        public string? DefaultEmailTemplateKey { get; set; }

        /// <summary>
        /// URL to the application's icon
        /// </summary>
        public string? IconUrl { get; set; }

        /// <summary>
        /// Contact email for notification-related issues
        /// </summary>
        public string? ContactEmail { get; set; }
    }
}