using System;

namespace Blazor.RCL.Domain.Entities.Notifications
{
    /// <summary>
    /// Defines the severity levels for notifications.
    /// </summary>
    public enum NotificationSeverity
    {
        /// <summary>
        /// Informational notification.
        /// </summary>
        Info = 0,

        /// <summary>
        /// Warning notification.
        /// </summary>
        Warning = 1,

        /// <summary>
        /// Error notification.
        /// </summary>
        Error = 2,

        /// <summary>
        /// Critical notification requiring immediate attention.
        /// </summary>
        Critical = 3
    }

    /// <summary>
    /// Defines the types of alerts.
    /// </summary>
    public enum AlertType
    {
        /// <summary>
        /// System-related alert.
        /// </summary>
        System = 0,

        /// <summary>
        /// Security-related alert.
        /// </summary>
        Security = 1,

        /// <summary>
        /// Maintenance-related alert.
        /// </summary>
        Maintenance = 2,

        /// <summary>
        /// Performance-related alert.
        /// </summary>
        Performance = 3,

        /// <summary>
        /// Health check alert.
        /// </summary>
        HealthCheck = 4
    }

    /// <summary>
    /// Defines email notification frequency options.
    /// </summary>
    public enum EmailFrequency
    {
        /// <summary>
        /// Send emails immediately.
        /// </summary>
        Immediate = 0,

        /// <summary>
        /// Batch and send emails hourly.
        /// </summary>
        Hourly = 1,

        /// <summary>
        /// Batch and send emails daily.
        /// </summary>
        Daily = 2,

        /// <summary>
        /// Email notifications disabled.
        /// </summary>
        Disabled = 3,

        /// <summary>
        /// Use global user settings.
        /// </summary>
        UseGlobal = 3
    }

    /// <summary>
    /// Defines notification delivery channels.
    /// </summary>
    [Flags]
    public enum DeliveryChannel
    {
        /// <summary>
        /// No delivery channel.
        /// </summary>
        None = 0,

        /// <summary>
        /// Real-time delivery via SignalR.
        /// </summary>
        SignalR = 1,

        /// <summary>
        /// Email delivery.
        /// </summary>
        Email = 2
    }

    /// <summary>
    /// Defines notification delivery status.
    /// </summary>
    public enum DeliveryStatus
    {
        /// <summary>
        /// Notification is pending delivery.
        /// </summary>
        Pending = 0,

        /// <summary>
        /// Notification has been delivered.
        /// </summary>
        Delivered = 1,

        /// <summary>
        /// Notification delivery failed.
        /// </summary>
        Failed = 2,

        /// <summary>
        /// Notification was confirmed by user.
        /// </summary>
        Confirmed = 3
    }

    /// <summary>
    /// Defines email processing status.
    /// </summary>
    public enum EmailStatus
    {
        /// <summary>
        /// Email is pending processing.
        /// </summary>
        Pending = 0,

        /// <summary>
        /// Email is being processed.
        /// </summary>
        Processing = 1,

        /// <summary>
        /// Email has been sent.
        /// </summary>
        Sent = 2,

        /// <summary>
        /// Email sending failed.
        /// </summary>
        Failed = 3
    }

    /// <summary>
    /// Defines email priority levels.
    /// </summary>
    public enum EmailPriority
    {
        /// <summary>
        /// Normal priority.
        /// </summary>
        Normal = 0,

        /// <summary>
        /// High priority.
        /// </summary>
        High = 1
    }
}