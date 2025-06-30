using Blazor.RCL.Domain.Entities.Notifications;

namespace Blazor.RCL.Application.Models.Notifications
{
    /// <summary>
    /// Information about an application's subscription status for a user
    /// </summary>
    public class ApplicationSubscriptionInfo
    {
        /// <summary>
        /// The application profile
        /// </summary>
        public ApplicationNotificationProfile ApplicationProfile { get; set; } = null!;

        /// <summary>
        /// Whether the user is currently subscribed
        /// </summary>
        public bool IsSubscribed { get; set; }

        /// <summary>
        /// User's current settings for this application (null if not configured)
        /// </summary>
        public UserApplicationNotificationSettings? UserSettings { get; set; }

        /// <summary>
        /// Whether the application is available for subscription
        /// </summary>
        public bool IsAvailable => ApplicationProfile.IsActive;

        /// <summary>
        /// Whether this is a new application the user hasn't configured yet
        /// </summary>
        public bool IsNew => UserSettings == null;
    }
}