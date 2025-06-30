using System;
using System.ComponentModel.DataAnnotations;

namespace Blazor.RCL.Domain.Entities.Notifications
{
    /// <summary>
    /// Defines notification capabilities and defaults for each Automation application.
    /// </summary>
    public class ApplicationNotificationProfile
    {
        /// <summary>
        /// Gets or sets the unique identifier for the application profile.
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the application name (unique identifier).
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string ApplicationName { get; set; }

        /// <summary>
        /// Gets or sets the display name for the application.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the application description.
        /// </summary>
        [MaxLength(500)]
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the supported alert types as a JSON array.
        /// Contains array of AlertType enums: 0=System, 1=Security, 2=Maintenance, 3=Performance, 4=HealthCheck
        /// </summary>
        [Required]
        public string SupportedAlertTypes { get; set; }

        /// <summary>
        /// Gets or sets the default minimum severity level for notifications.
        /// 0=Info, 1=Warning, 2=Error, 3=Critical
        /// </summary>
        [Required]
        public int DefaultSeverityFilter { get; set; }

        /// <summary>
        /// Gets or sets whether notifications are enabled by default for new users.
        /// </summary>
        [Required]
        public bool EnabledByDefault { get; set; }

        /// <summary>
        /// Gets or sets the default email template key for this application.
        /// </summary>
        [MaxLength(50)]
        public string? DefaultEmailTemplateKey { get; set; }

        /// <summary>
        /// Gets or sets the icon URL for this application.
        /// </summary>
        [MaxLength(255)]
        public string? IconUrl { get; set; }

        /// <summary>
        /// Gets or sets the contact email for this application.
        /// </summary>
        [MaxLength(255)]
        public string? ContactEmail { get; set; }

        /// <summary>
        /// Gets or sets when this profile was created.
        /// </summary>
        [Required]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets when this profile was last updated.
        /// </summary>
        [Required]
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Gets or sets whether the profile is active.
        /// </summary>
        [Required]
        public bool IsActive { get; set; }
    }
}