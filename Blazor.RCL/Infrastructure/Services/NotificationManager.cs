using MudBlazor;
using Blazor.RCL.Infrastructure.Services.Interfaces;
using Blazor.RCL.NLog.LogService.Interface;

namespace Blazor.RCL.Infrastructure.Services
{
    /// <summary>
    /// Singleton service for managing snackbar notifications that persists across page navigation
    /// </summary>
    public class NotificationManager : INotificationManager
    {
        private readonly ILogHelper _logger;
        private ISnackbar? _snackbar;
        
        
        /// <summary>
        /// Initializes a new instance of the NotificationManager class
        /// </summary>
        public NotificationManager(ILogHelper logger) 
        {
            _logger = logger;
        }
        
        /// <summary>
        /// Set the snackbar service reference
        /// </summary>
        public void SetSnackbar(ISnackbar snackbar)
        {
            _snackbar = snackbar;
        }
        
        /// <summary>
        /// Show a notification using the snackbar service
        /// </summary>
        public void ShowNotification(string message, Severity severity, int duration = 5000)
        {
            try
            {
                if (_snackbar == null)
                {
                    _logger.LogError("Cannot show notification - Snackbar service not set", 
                        new Exception("Snackbar reference is null"));
                    return;
                }
                
                // Configure snackbar
                _snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopRight;
                _snackbar.Configuration.ShowCloseIcon = true;
                _snackbar.Configuration.ShowTransitionDuration = 300;
                _snackbar.Configuration.HideTransitionDuration = 300;
                _snackbar.Configuration.MaxDisplayedSnackbars = 5;
                
                // Show notification
                _snackbar.Add(message, severity, options => {
                    options.VisibleStateDuration = duration;
                    options.CloseAfterNavigation = false;
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error showing notification: {ex.Message}", ex);
            }
        }
    }
} 