using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blazor.RCL.Domain.Entities.Notifications
{
    /// <summary>
    /// Represents an email notification in the processing queue.
    /// </summary>
    public class EmailNotificationQueue
    {
        /// <summary>
        /// Gets or sets the unique identifier for the queue entry.
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the notification identifier.
        /// </summary>
        [Required]
        public Guid NotificationId { get; set; }

        /// <summary>
        /// Gets or sets the username receiving the email.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the email address.
        /// </summary>
        [Required]
        [MaxLength(255)]
        public string EmailAddress { get; set; }

        /// <summary>
        /// Gets or sets the email subject.
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the HTML body content.
        /// </summary>
        [Required]
        public string HtmlBody { get; set; }

        /// <summary>
        /// Gets or sets the text body content (fallback).
        /// </summary>
        public string? TextBody { get; set; }

        /// <summary>
        /// Gets or sets the email priority.
        /// 0=Normal, 1=High
        /// </summary>
        [Required]
        public int Priority { get; set; }

        /// <summary>
        /// Gets or sets the processing status.
        /// 0=Pending, 1=Processing, 2=Sent, 3=Failed
        /// </summary>
        [Required]
        public int Status { get; set; }

        /// <summary>
        /// Gets or sets when the email should be sent.
        /// </summary>
        [Required]
        public DateTime ScheduledSendTime { get; set; }

        /// <summary>
        /// Gets or sets when the email was actually sent.
        /// </summary>
        public DateTime? ActualSendTime { get; set; }

        /// <summary>
        /// Gets or sets the reason for send failure.
        /// </summary>
        [MaxLength(500)]
        public string? FailureReason { get; set; }

        /// <summary>
        /// Gets or sets the number of retry attempts.
        /// </summary>
        [Required]
        public int RetryCount { get; set; }

        /// <summary>
        /// Gets or sets when this queue entry was created.
        /// </summary>
        [Required]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Gets or sets the instance identifier (server + process) that is processing this email.
        /// Used for debugging and preventing duplicate processing in multi-server environments.
        /// </summary>
        [MaxLength(255)]
        public string? ProcessingInstance { get; set; }

        /// <summary>
        /// Gets or sets when processing started for this email.
        /// Used to detect and recover stuck emails (e.g., if a server crashes during processing).
        /// </summary>
        public DateTime? ProcessingStartedAt { get; set; }

        /// <summary>
        /// Gets or sets the row version for optimistic concurrency control.
        /// This is automatically managed by SQL Server to prevent concurrent updates.
        /// </summary>
        [Timestamp]
        public byte[]? RowVersion { get; set; }

        /// <summary>
        /// Navigation property to the NotificationMessage entity.
        /// </summary>
        [ForeignKey("NotificationId")]
        public virtual NotificationMessage? NotificationMessage { get; set; }
    }
}