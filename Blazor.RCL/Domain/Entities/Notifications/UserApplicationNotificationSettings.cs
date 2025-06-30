using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blazor.RCL.Domain.Entities.Notifications
{
    /// <summary>
    /// Represents per-application notification preferences for each user.
    /// </summary>
    public class UserApplicationNotificationSettings
    {
        /// <summary>
        /// Gets or sets the unique identifier for the settings.
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the application name.
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string ApplicationName { get; set; }

        /// <summary>
        /// Gets or sets whether the user is subscribed to notifications from this application.
        /// </summary>
        [Required]
        public bool IsSubscribed { get; set; }

        /// <summary>
        /// Gets or sets whether real-time notifications are enabled for this application.
        /// </summary>
        [Required]
        public bool EnableRealTimeNotifications { get; set; }

        /// <summary>
        /// Gets or sets whether email notifications are enabled for this application.
        /// </summary>
        [Required]
        public bool EnableEmailNotifications { get; set; }

        /// <summary>
        /// Gets or sets the email frequency for this application.
        /// 0=Immediate, 1=Hourly, 2=Daily, 3=UseGlobal
        /// </summary>
        [Required]
        public int EmailFrequency { get; set; }

        /// <summary>
        /// Gets or sets the minimum severity level to receive notifications.
        /// 0=Info, 1=Warning, 2=Error, 3=Critical
        /// </summary>
        [Required]
        public int SeverityFilter { get; set; }

        /// <summary>
        /// Gets or sets the enabled alert types as a JSON array.
        /// Contains array of AlertType enums: 0=System, 1=Security, 2=Maintenance, 3=Performance, 4=HealthCheck
        /// </summary>
        public string? AlertTypeFilter { get; set; }

        /// <summary>
        /// Gets or sets whether critical alerts override all filters.
        /// </summary>
        [Required]
        public bool EnableCriticalAlerts { get; set; }

        /// <summary>
        /// Gets or sets when these settings were created.
        /// </summary>
        [Required]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets when these settings were last updated.
        /// </summary>
        [Required]
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// Navigation property to the UserNotificationSettings entity.
        /// </summary>
        [ForeignKey("Username")]
        public virtual UserNotificationSettings? UserNotificationSettings { get; set; }

        /// <summary>
        /// Navigation property to the ApplicationNotificationProfile entity.
        /// </summary>
        [ForeignKey("ApplicationName")]
        public virtual ApplicationNotificationProfile? ApplicationNotificationProfile { get; set; }
    }
}