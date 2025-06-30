using System.Collections.Generic;
using Blazor.RCL.Domain.Entities.Notifications;

namespace Blazor.RCL.Application.Models.Notifications
{
    /// <summary>
    /// Request model for updating user's application-specific notification settings
    /// </summary>
    public class UpdateUserApplicationSettingsRequest
    {
        /// <summary>
        /// Whether the user is subscribed to notifications from this application
        /// </summary>
        public bool? IsSubscribed { get; set; }

        /// <summary>
        /// Whether to enable real-time notifications via SignalR
        /// </summary>
        public bool? EnableRealTimeNotifications { get; set; }

        /// <summary>
        /// Whether to enable email notifications for this application
        /// </summary>
        public bool? EnableEmailNotifications { get; set; }

        /// <summary>
        /// Email frequency for this application (overrides global setting)
        /// </summary>
        public EmailFrequency? EmailFrequency { get; set; }

        /// <summary>
        /// Minimum severity level to receive notifications
        /// </summary>
        public NotificationSeverity? SeverityFilter { get; set; }

        /// <summary>
        /// Alert types to receive (null means all types)
        /// </summary>
        public List<AlertType>? AlertTypeFilter { get; set; }

        /// <summary>
        /// Whether critical alerts should bypass all filters
        /// </summary>
        public bool? EnableCriticalAlerts { get; set; }
    }
}