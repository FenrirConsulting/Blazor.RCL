using System;
using Blazor.RCL.Domain.Entities.Notifications;

namespace Blazor.RCL.Application.Models.Notifications
{
    /// <summary>
    /// View model for displaying user notifications with delivery status
    /// </summary>
    public class UserNotificationViewModel
    {
        /// <summary>
        /// The notification ID
        /// </summary>
        public Guid NotificationId { get; set; }

        /// <summary>
        /// The message ID for tracking
        /// </summary>
        public string MessageId { get; set; } = string.Empty;

        /// <summary>
        /// Source application name
        /// </summary>
        public string SourceApplication { get; set; } = string.Empty;

        /// <summary>
        /// Application display name
        /// </summary>
        public string ApplicationDisplayName { get; set; } = string.Empty;

        /// <summary>
        /// Application icon URL
        /// </summary>
        public string? ApplicationIconUrl { get; set; }

        /// <summary>
        /// Notification title
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Notification content
        /// </summary>
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// Severity level
        /// </summary>
        public NotificationSeverity Severity { get; set; }

        /// <summary>
        /// Alert type
        /// </summary>
        public AlertType AlertType { get; set; }

        /// <summary>
        /// Whether confirmation is required
        /// </summary>
        public bool RequiresConfirmation { get; set; }

        /// <summary>
        /// When the notification was created
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Who created the notification
        /// </summary>
        public string CreatedBy { get; set; } = string.Empty;

        /// <summary>
        /// Delivery channel used
        /// </summary>
        public DeliveryChannel DeliveryChannel { get; set; }

        /// <summary>
        /// Current delivery status
        /// </summary>
        public DeliveryStatus DeliveryStatus { get; set; }

        /// <summary>
        /// When the notification was delivered
        /// </summary>
        public DateTime? DeliveredAt { get; set; }

        /// <summary>
        /// When the user confirmed/acknowledged the notification
        /// </summary>
        public DateTime? ConfirmedAt { get; set; }

        /// <summary>
        /// Additional metadata
        /// </summary>
        public string? Metadata { get; set; }

        /// <summary>
        /// Whether the notification is read (delivered or confirmed)
        /// </summary>
        public bool IsRead => DeliveryStatus == DeliveryStatus.Delivered || DeliveryStatus == DeliveryStatus.Confirmed;

        /// <summary>
        /// Whether the notification is confirmed
        /// </summary>
        public bool IsConfirmed => DeliveryStatus == DeliveryStatus.Confirmed;
    }
}