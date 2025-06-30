using System;
using System.Collections.Generic;
using Blazor.RCL.Domain.Entities.Notifications;

namespace Blazor.RCL.Application.Models.Notifications
{
    /// <summary>
    /// Request model for creating a new notification
    /// </summary>
    public class CreateNotificationRequest
    {
        /// <summary>
        /// The application creating the notification
        /// </summary>
        public string SourceApplication { get; set; } = string.Empty;

        /// <summary>
        /// Notification title
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Notification content/body
        /// </summary>
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// Severity level of the notification
        /// </summary>
        public NotificationSeverity Severity { get; set; }

        /// <summary>
        /// Type of alert
        /// </summary>
        public AlertType AlertType { get; set; }

        /// <summary>
        /// Whether the notification requires user confirmation
        /// </summary>
        public bool RequiresConfirmation { get; set; }

        /// <summary>
        /// Additional metadata as JSON
        /// </summary>
        public string? Metadata { get; set; }

        /// <summary>
        /// Username creating the notification
        /// </summary>
        public string CreatedBy { get; set; } = string.Empty;

        /// <summary>
        /// Optional: Specific users to notify (if null, will use application subscribers)
        /// </summary>
        public List<string>? TargetUsernames { get; set; }

        /// <summary>
        /// Optional: Specific roles to notify (all users with any of these roles)
        /// </summary>
        public List<string>? TargetRoles { get; set; }

        /// <summary>
        /// Optional: Override delivery channels (if null, will use user preferences)
        /// </summary>
        public DeliveryChannel? DeliveryChannels { get; set; }
    }
}