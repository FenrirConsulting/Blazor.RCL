using System;
using System.Collections.Generic;
using Blazor.RCL.Domain.Entities.Notifications;

namespace Blazor.RCL.Application.Models.Notifications
{
    /// <summary>
    /// Statistics about the email notification queue
    /// </summary>
    public class EmailQueueStatistics
    {
        /// <summary>
        /// Total number of emails in queue
        /// </summary>
        public int TotalInQueue { get; set; }

        /// <summary>
        /// Number of pending emails
        /// </summary>
        public int PendingCount { get; set; }

        /// <summary>
        /// Number of emails currently being processed
        /// </summary>
        public int ProcessingCount { get; set; }

        /// <summary>
        /// Number of successfully sent emails (last 24 hours)
        /// </summary>
        public int SentLast24Hours { get; set; }

        /// <summary>
        /// Number of failed emails
        /// </summary>
        public int FailedCount { get; set; }

        /// <summary>
        /// Breakdown by status
        /// </summary>
        public Dictionary<EmailStatus, int> StatusBreakdown { get; set; } = new();

        /// <summary>
        /// Breakdown by priority
        /// </summary>
        public Dictionary<EmailPriority, int> PriorityBreakdown { get; set; } = new();

        /// <summary>
        /// Oldest pending email timestamp
        /// </summary>
        public DateTime? OldestPendingEmail { get; set; }

        /// <summary>
        /// Average processing time in seconds (last 100 emails)
        /// </summary>
        public double AverageProcessingTimeSeconds { get; set; }

        /// <summary>
        /// Last successful email sent timestamp
        /// </summary>
        public DateTime? LastSuccessfulSend { get; set; }

        /// <summary>
        /// Last failed email timestamp
        /// </summary>
        public DateTime? LastFailedSend { get; set; }

        /// <summary>
        /// When these statistics were calculated
        /// </summary>
        public DateTime CalculatedAt { get; set; } = DateTime.UtcNow;
    }
}