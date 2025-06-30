using System;
using System.ComponentModel.DataAnnotations;

namespace Blazor.RCL.Domain.Entities.Notifications
{
    /// <summary>
    /// Represents an email template for notification messages
    /// </summary>
    public class EmailTemplate
    {
        /// <summary>
        /// Gets or sets the unique identifier for the email template
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the template key (unique identifier used in code)
        /// Examples: "SystemAlert", "Maintenance", "SecurityWarning", "PerformanceAlert"
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string TemplateKey { get; set; }

        /// <summary>
        /// Gets or sets the application name this template belongs to
        /// Null means it's a global/default template
        /// </summary>
        [MaxLength(50)]
        public string? ApplicationName { get; set; }

        /// <summary>
        /// Gets or sets the display name for the template
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the template description
        /// </summary>
        [MaxLength(500)]
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the email subject template with variables
        /// Example: "[{{ApplicationName}}] {{AlertType}}: {{Title}}"
        /// </summary>
        [Required]
        [MaxLength(255)]
        public string SubjectTemplate { get; set; }

        /// <summary>
        /// Gets or sets the HTML body template with variables
        /// Supports variables like {{Title}}, {{Content}}, {{ApplicationName}}, etc.
        /// </summary>
        [Required]
        public string HtmlBodyTemplate { get; set; }

        /// <summary>
        /// Gets or sets the plain text body template with variables
        /// </summary>
        [Required]
        public string TextBodyTemplate { get; set; }

        /// <summary>
        /// Gets or sets the available variables as JSON
        /// Example: [{"name": "Title", "description": "Notification title", "required": true}]
        /// </summary>
        public string? AvailableVariables { get; set; }

        /// <summary>
        /// Gets or sets custom CSS styles for the HTML template
        /// </summary>
        public string? CustomStyles { get; set; }

        /// <summary>
        /// Gets or sets the template version number
        /// </summary>
        [Required]
        public int Version { get; set; }

        /// <summary>
        /// Gets or sets whether this template is active
        /// </summary>
        [Required]
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets whether this is the default template for its type
        /// </summary>
        [Required]
        public bool IsDefault { get; set; }

        /// <summary>
        /// Gets or sets the alert types this template supports (JSON array)
        /// Null means it supports all alert types
        /// </summary>
        public string? SupportedAlertTypes { get; set; }

        /// <summary>
        /// Gets or sets the severity levels this template supports (JSON array)
        /// Null means it supports all severity levels
        /// </summary>
        public string? SupportedSeverityLevels { get; set; }

        /// <summary>
        /// Gets or sets when this template was created
        /// </summary>
        [Required]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets who created this template
        /// </summary>
        [MaxLength(50)]
        public string? CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets when this template was last updated
        /// </summary>
        [Required]
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Gets or sets who last updated this template
        /// </summary>
        [MaxLength(50)]
        public string? UpdatedBy { get; set; }

        /// <summary>
        /// Navigation property to ApplicationNotificationProfile
        /// </summary>
        public virtual ApplicationNotificationProfile? Application { get; set; }
    }
}