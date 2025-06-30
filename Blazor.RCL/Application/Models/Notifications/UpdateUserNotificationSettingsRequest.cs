using System;
using Blazor.RCL.Domain.Entities.Notifications;

namespace Blazor.RCL.Application.Models.Notifications
{
    /// <summary>
    /// Request model for updating user's global notification settings
    /// </summary>
    public class UpdateUserNotificationSettingsRequest
    {
        /// <summary>
        /// Email address for notifications
        /// </summary>
        public string? EmailAddress { get; set; }

        /// <summary>
        /// Whether to enable email notifications globally
        /// </summary>
        public bool? EnableEmailNotifications { get; set; }

        /// <summary>
        /// Global email frequency preference
        /// </summary>
        public EmailFrequency? GlobalEmailFrequency { get; set; }

        /// <summary>
        /// Start time for quiet hours (no notifications)
        /// </summary>
        public TimeSpan? QuietHoursStart { get; set; }

        /// <summary>
        /// End time for quiet hours
        /// </summary>
        public TimeSpan? QuietHoursEnd { get; set; }

        /// <summary>
        /// User's timezone (IANA timezone ID)
        /// </summary>
        public string? TimeZone { get; set; }
    }
}