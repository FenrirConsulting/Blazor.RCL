using MudBlazor;

namespace Blazor.RCL.Infrastructure.Services.Interfaces
{
    /// <summary>
    /// Singleton service for managing snackbar notifications that persists across page navigation
    /// </summary>
    public interface INotificationManager
    {
        /// <summary>
        /// Set the snackbar service reference
        /// </summary>
        /// <param name="snackbar">The MudBlazor snackbar service</param>
        void SetSnackbar(ISnackbar snackbar);
        
        /// <summary>
        /// Show a notification using the snackbar service
        /// </summary>
        /// <param name="message">The notification message</param>
        /// <param name="severity">The severity level</param>
        /// <param name="duration">Duration in milliseconds</param>
        void ShowNotification(string message, Severity severity, int duration = 5000);
    }
} 