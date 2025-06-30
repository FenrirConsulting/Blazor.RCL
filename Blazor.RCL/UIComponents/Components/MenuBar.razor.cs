using Microsoft.AspNetCore.Components;
using Blazor.RCL.Infrastructure.Common.Configuration;
using Blazor.RCL.NLog.LogService.Interface;
using Blazor.RCL.Infrastructure.Services;
using Blazor.RCL.Infrastructure.Services.Interfaces;

namespace Blazor.RCL.UIComponents.Components
{
    /// <summary>
    /// Represents the menu bar component of the application.
    /// </summary>
    public partial class MenuBar : ComponentBase, IDisposable
    {
        #region Injected Services
        [Inject] protected ILogHelper _Logger { get; set; } = default!;
        [Inject] protected IRequestRefresh? _RequestRefresh { get; set; }
        [Inject] protected NavigationManager _NavigationManager { get; set; } = default!;
        [Inject] protected NavLinksInfoList _Links { get; set; } = default!;
        #endregion

        #region Private Methods
        /// <summary>
        /// Determines if the given href matches the current page.
        /// </summary>
        /// <param name="href">The href to check.</param>
        /// <returns>True if the current page matches the href, false otherwise.</returns>
        private bool IsActive(string href)
        {
            // Implement logic to determine if the current page matches the href
            // This is a placeholder and needs to be implemented based on your routing logic
            return _NavigationManager.Uri.EndsWith(href);
        }

        /// <summary>
        /// Refreshes the component when requested.
        /// </summary>
        private void RefreshComponent()
        {
            try
            {
                StateHasChanged();
            }
            catch (Exception ex)
            {
                _Logger?.LogError("Error occurred while refreshing NavMenu", ex);
            }
        }
        #endregion

        #region Lifecycle Methods
        /// <summary>
        /// Initializes the component.
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            try
            {
                _RequestRefresh!.RefreshRequested += RefreshComponent;
            }
            catch (Exception ex)
            {
                _Logger.LogError("Error occurred while initializing MenuBar", ex);
            }
        }

        /// <summary>
        /// Disposes the resources used by the MenuBar component.
        /// </summary>
        public void Dispose()
        {
            try
            {
                if (_RequestRefresh != null)
                {
                    _RequestRefresh.RefreshRequested -= RefreshComponent;
                }
            }
            catch (Exception ex)
            {
                _Logger.LogError("Error occurred while disposing MenuBar", ex);
            }
        }
        #endregion
    }
}