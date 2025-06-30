using System;

namespace Blazor.RCL.Application.Models.Notifications
{
    /// <summary>
    /// Statistics about application notification subscriptions
    /// </summary>
    public class ApplicationSubscriptionStats
    {
        /// <summary>
        /// Application name
        /// </summary>
        public string ApplicationName { get; set; } = string.Empty;

        /// <summary>
        /// Total number of users with settings for this application
        /// </summary>
        public int TotalUsers { get; set; }

        /// <summary>
        /// Number of users subscribed to notifications
        /// </summary>
        public int SubscribedUsers { get; set; }

        /// <summary>
        /// Number of users with real-time notifications enabled
        /// </summary>
        public int RealTimeEnabledUsers { get; set; }

        /// <summary>
        /// Number of users with email notifications enabled
        /// </summary>
        public int EmailEnabledUsers { get; set; }

        /// <summary>
        /// Number of users who have explicitly unsubscribed
        /// </summary>
        public int UnsubscribedUsers { get; set; }

        /// <summary>
        /// Subscription rate percentage
        /// </summary>
        public double SubscriptionRate => TotalUsers > 0 ? (double)SubscribedUsers / TotalUsers * 100 : 0;

        /// <summary>
        /// Last time stats were calculated
        /// </summary>
        public DateTime CalculatedAt { get; set; } = DateTime.UtcNow;
    }
}