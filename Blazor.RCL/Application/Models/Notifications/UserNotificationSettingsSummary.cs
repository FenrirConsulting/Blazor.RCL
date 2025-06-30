using System;
using System.Collections.Generic;
using Blazor.RCL.Domain.Entities.Notifications;

namespace Blazor.RCL.Application.Models.Notifications
{
    /// <summary>
    /// Comprehensive summary of a user's notification settings across all applications
    /// </summary>
    public class UserNotificationSettingsSummary
    {
        /// <summary>
        /// User's global notification settings
        /// </summary>
        public UserNotificationSettings GlobalSettings { get; set; } = null!;

        /// <summary>
        /// Application-specific settings with enriched application info
        /// </summary>
        public List<ApplicationSettingsSummary> ApplicationSettings { get; set; } = new();

        /// <summary>
        /// Total number of applications user is subscribed to
        /// </summary>
        public int SubscribedApplicationCount => ApplicationSettings.Count(a => a.Settings.IsSubscribed);

        /// <summary>
        /// Whether user has any active notification subscriptions
        /// </summary>
        public bool HasActiveSubscriptions => ApplicationSettings.Any(a => a.Settings.IsSubscribed);

        /// <summary>
        /// Current quiet hours status
        /// </summary>
        public QuietHoursStatus QuietHoursStatus { get; set; } = null!;
    }

    /// <summary>
    /// Application-specific settings with enriched application information
    /// </summary>
    public class ApplicationSettingsSummary
    {
        /// <summary>
        /// The application profile
        /// </summary>
        public ApplicationNotificationProfile ApplicationProfile { get; set; } = null!;

        /// <summary>
        /// User's settings for this application
        /// </summary>
        public UserApplicationNotificationSettings Settings { get; set; } = null!;

        /// <summary>
        /// Effective email frequency (considering global override)
        /// </summary>
        public EmailFrequency EffectiveEmailFrequency => 
            Settings.EmailFrequency == (int)EmailFrequency.UseGlobal 
                ? (EmailFrequency)(Settings.UserNotificationSettings?.GlobalEmailFrequency ?? (int)EmailFrequency.Immediate)
                : (EmailFrequency)Settings.EmailFrequency;

        /// <summary>
        /// Whether notifications are effectively enabled (considering all settings)
        /// </summary>
        public bool IsEffectivelyEnabled => 
            Settings.IsSubscribed && 
            (Settings.EnableRealTimeNotifications || 
             (Settings.EnableEmailNotifications && Settings.UserNotificationSettings?.EnableEmailNotifications == true));
    }

    /// <summary>
    /// Current quiet hours status for a user
    /// </summary>
    public class QuietHoursStatus
    {
        /// <summary>
        /// Whether quiet hours are configured
        /// </summary>
        public bool IsConfigured { get; set; }

        /// <summary>
        /// Whether currently in quiet hours
        /// </summary>
        public bool IsCurrentlyActive { get; set; }

        /// <summary>
        /// When quiet hours will end (if active) or start (if not active)
        /// </summary>
        public DateTime? NextTransitionTime { get; set; }

        /// <summary>
        /// User's current local time
        /// </summary>
        public DateTime UserLocalTime { get; set; }
    }
}