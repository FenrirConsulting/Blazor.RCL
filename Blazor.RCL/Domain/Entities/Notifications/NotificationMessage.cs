using System;
using System.ComponentModel.DataAnnotations;

namespace Blazor.RCL.Domain.Entities.Notifications
{
    /// <summary>
    /// Represents a notification message in the system.
    /// Core table for storing all notification messages across Automation applications.
    /// </summary>
    public class NotificationMessage
    {
        /// <summary>
        /// Gets or sets the unique identifier for the notification message.
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the unique message identifier.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string MessageId { get; set; }

        /// <summary>
        /// Gets or sets the source application that created this notification.
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string SourceApplication { get; set; }

        /// <summary>
        /// Gets or sets the notification title.
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the notification content.
        /// </summary>
        [Required]
        public string Content { get; set; }

        /// <summary>
        /// Gets or sets the severity level of the notification.
        /// 0=Info, 1=Warning, 2=Error, 3=Critical
        /// </summary>
        [Required]
        public int Severity { get; set; }

        /// <summary>
        /// Gets or sets the alert type.
        /// 0=System, 1=Security, 2=Maintenance, 3=Performance, 4=HealthCheck
        /// </summary>
        [Required]
        public int AlertType { get; set; }

        /// <summary>
        /// Gets or sets when the notification was created.
        /// </summary>
        [Required]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets when the notification expires.
        /// </summary>
        public DateTime? ExpiresAt { get; set; }

        /// <summary>
        /// Gets or sets whether this notification requires user confirmation.
        /// </summary>
        [Required]
        public bool RequiresConfirmation { get; set; }

        /// <summary>
        /// Gets or sets additional metadata as JSON.
        /// </summary>
        public string? Metadata { get; set; }

        /// <summary>
        /// Gets or sets the username who created this notification.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string CreatedBy { get; set; }

        /// <summary>
        /// Gets or sets whether the notification is active.
        /// </summary>
        [Required]
        public bool IsActive { get; set; }
    }
}