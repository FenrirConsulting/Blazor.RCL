using Blazor.RCL.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Blazor.RCL.NLog.LogService.Interface;
using Blazor.RCL.UIComponents.Components;
using Blazor.RCL.Application.Common.Configuration.Interfaces;
using Blazor.RCL.Application.Interfaces;
using MudBlazor;

namespace Blazor.RCL.UIComponents.Layouts
{
    /// <summary>
    /// Represents the module layout component of the application.
    /// </summary>
    public partial class ModuleLayout : IDisposable
    {
        #region Injected Services
        /// <summary>
        /// Service for managing the application theme.
        /// </summary>
        [Inject] private IThemeService _themeService { get; set; } = default!;

        /// <summary>
        /// Service for logging.
        /// </summary>
        [Inject] private ILogHelper _logger { get; set; } = default!;

        /// <summary>
        /// Application configuration service.
        /// </summary>
        [Inject] private IAppConfiguration _appConfiguration { get; set; } = default!;

        /// <summary>
        /// Repository for tools configuration.
        /// </summary>
        [Inject] private IToolsConfigurationRepository _toolsRepository { get; set; } = default!;

        /// <summary>
        /// Notification manager for handling snackbar notifications.
        /// </summary>
        [Inject] private INotificationManager _notificationManager { get; set; } = default!;

        /// <summary>
        /// MudBlazor snackbar service.
        /// </summary>
        [Inject] private ISnackbar _snackbar { get; set; } = default!;
        #endregion

        #region Private Fields
        private bool _sideBarOpen = true;
        private bool _isLoading = true; // Loading flag
        private HelpDocumentInfo _helpDocumentInfo = new HelpDocumentInfo();
        private bool _disposed = false;
        #endregion

        #region Private Methods
        /// <summary>
        /// Toggles the sidebar open/closed state.
        /// </summary>
        private void ToggleSidebar() => _sideBarOpen = !_sideBarOpen;
        #endregion

        #region Lifecycle Methods
        /// <summary>
        /// Initializes the component and sets up the theme.
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            try
            {
                await _themeService.InitializeThemeAsync();
                _themeService.OnChange += StateHasChanged;
                
                // Initialize help document information
                _helpDocumentInfo = new HelpDocumentInfo
                {
                    DocumentLibrary = "Documentation",
                    Document = $"{_appConfiguration.AppName}Documentation.md"
                };

                // Try to load from configuration if available
                try
                {
                    var helpDocConfig = await _toolsRepository.GetValueAsync(_appConfiguration.AppName, "HelpDocument");
                    if (!string.IsNullOrEmpty(helpDocConfig))
                    {
                        var configHelpDoc = System.Text.Json.JsonSerializer.Deserialize<HelpDocumentInfo>(helpDocConfig);
                        if (configHelpDoc != null)
                        {
                            _helpDocumentInfo = configHelpDoc;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Could not load help document configuration:",ex);
                    // Use default values initialized above
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurred while initializing ModuleLayout", ex);
            }
        }

        /// <summary>
        /// Performs actions after the component has rendered.
        /// </summary>
        /// <param name="firstRender">Indicates if this is the first time the component has been rendered.</param>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                try
                {
                    // Initialize the Snackbar in the NotificationManager
                    _notificationManager.SetSnackbar(_snackbar);
                }
                catch (Exception ex)
                {
                    _logger.LogError("Error initializing Snackbar in ModuleLayout", ex);
                }

                _isLoading = false; // Set loading flag to false after initialization
                StateHasChanged(); // Trigger re-render after initialization
            }
            await Task.CompletedTask; // Add an await statement to satisfy the async method requirement
        }

        /// <summary>
        /// Disposes the resources used by the ModuleLayout component.
        /// </summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                _themeService.OnChange -= StateHasChanged;
            }
        }
        #endregion
    }
}