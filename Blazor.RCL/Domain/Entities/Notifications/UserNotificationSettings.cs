using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blazor.RCL.Domain.Entities.Notifications
{
    /// <summary>
    /// Represents global notification settings for a user.
    /// </summary>
    public class UserNotificationSettings
    {
        /// <summary>
        /// Gets or sets the unique identifier for the user notification settings.
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the username associated with these settings.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the user's email address.
        /// </summary>
        [Required]
        [MaxLength(255)]
        public string EmailAddress { get; set; }

        /// <summary>
        /// Gets or sets whether email notifications are enabled globally.
        /// </summary>
        [Required]
        public bool EnableEmailNotifications { get; set; }

        /// <summary>
        /// Gets or sets the global email frequency preference.
        /// 0=Immediate, 1=Hourly, 2=Daily, 3=Disabled
        /// </summary>
        [Required]
        public int GlobalEmailFrequency { get; set; }

        /// <summary>
        /// Gets or sets the start time for quiet hours (no notifications).
        /// </summary>
        public TimeSpan? QuietHoursStart { get; set; }

        /// <summary>
        /// Gets or sets the end time for quiet hours.
        /// </summary>
        public TimeSpan? QuietHoursEnd { get; set; }

        /// <summary>
        /// Gets or sets the user's time zone.
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string TimeZone { get; set; }

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
        /// Gets or sets whether the settings are active.
        /// </summary>
        [Required]
        public bool IsActive { get; set; }

        /// <summary>
        /// Navigation property to the UserSettings entity.
        /// </summary>
        [ForeignKey("Username")]
        public virtual Configuration.UserSettings? UserSettings { get; set; }
    }
}