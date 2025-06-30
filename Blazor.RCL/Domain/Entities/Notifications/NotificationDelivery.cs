using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blazor.RCL.Domain.Entities.Notifications
{
    /// <summary>
    /// Tracks delivery status for each notification to each user.
    /// </summary>
    public class NotificationDelivery
    {
        /// <summary>
        /// Gets or sets the unique identifier for the delivery record.
        /// </summary>
        [Key]
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the notification identifier.
        /// </summary>
        [Required]
        public Guid NotificationId { get; set; }

        /// <summary>
        /// Gets or sets the username receiving the notification.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the delivery channel.
        /// 0=SignalR, 1=Email
        /// </summary>
        [Required]
        public int DeliveryChannel { get; set; }

        /// <summary>
        /// Gets or sets the delivery status.
        /// 0=Pending, 1=Delivered, 2=Failed, 3=Confirmed
        /// </summary>
        [Required]
        public int DeliveryStatus { get; set; }

        /// <summary>
        /// Gets or sets when the notification was delivered.
        /// </summary>
        public DateTime? DeliveredAt { get; set; }

        /// <summary>
        /// Gets or sets when the notification was confirmed by the user.
        /// </summary>
        public DateTime? ConfirmedAt { get; set; }

        /// <summary>
        /// Gets or sets the reason for delivery failure.
        /// </summary>
        [MaxLength(500)]
        public string? FailureReason { get; set; }

        /// <summary>
        /// Gets or sets the number of retry attempts.
        /// </summary>
        [Required]
        public int RetryCount { get; set; }

        /// <summary>
        /// Gets or sets when this delivery record was created.
        /// </summary>
        [Required]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Navigation property to the NotificationMessage entity.
        /// </summary>
        [ForeignKey("NotificationId")]
        public virtual NotificationMessage? NotificationMessage { get; set; }

        /// <summary>
        /// Navigation property to the UserNotificationSettings entity.
        /// </summary>
        [ForeignKey("Username")]
        public virtual UserNotificationSettings? UserNotificationSettings { get; set; }
    }
}