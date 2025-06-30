using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using Blazor.RCL.Application.Common.Configuration.Interfaces;
using Blazor.RCL.Infrastructure.Services.Interfaces;
using Blazor.RCL.NLog.LogService.Interface;
using Blazor.RCL.Application.Interfaces;
using MudBlazor;

namespace Blazor.RCL.UIComponents.Components
{
    /// <summary>
    /// Represents the options panel component that provides user account management and preferences.
    /// </summary>
    public partial class OptionsPanel : ComponentBase, IDisposable
    {
        #region Injected Services
        [Inject] protected IThemeService _ThemeService { get; set; } = default!;
        [Inject] protected ILogHelper _Logger { get; set; } = default!;
        [Inject] protected IAppConfiguration _AppConfiguration { get; set; } = default!;
        [Inject] protected AuthenticationStateProvider _AuthenticationStateProvider { get; set; } = default!;
        [Inject] protected NavigationManager _NavigationManager { get; set; } = default!;
        [Inject] protected IApplicationNotificationProfileService _ApplicationProfileService { get; set; } = default!;
        #endregion

        #region Protected Properties
        protected List<string> UserRoles = new List<string>();
        protected string UserDisplayName = string.Empty;
        protected string UserEmail = string.Empty;
        protected bool IsAuthenticated = false;
        protected bool IsNotificationSettingsAvailable = false; // Will be set to true when NotificationSettingsPanel is implemented
        protected bool RolesExpanded = false;
        protected Dictionary<string, string> RoleIconLookup = new Dictionary<string, string>();
        protected string DefaultRoleIcon = Icons.Material.Filled.Key; // Default RBAC/AD related icon
        #endregion

        #region Lifecycle Methods
        /// <summary>
        /// Initializes the component and loads user information.
        /// </summary>
        protected override async Task OnInitializedAsync()
        {
            try
            {
                await LoadUserInformation();
                await LoadApplicationProfiles();
                _ThemeService.OnChange += HandleThemeChange;
            }
            catch (Exception ex)
            {
                _Logger.LogError("Error occurred while initializing OptionsPanel", ex);
            }
        }

        /// <summary>
        /// Disposes the resources used by the OptionsPanel component.
        /// </summary>
        public void Dispose()
        {
            try
            {
                _ThemeService.OnChange -= HandleThemeChange;
            }
            catch (Exception ex)
            {
                _Logger.LogError("Error occurred while disposing OptionsPanel", ex);
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Loads the current user's information from the authentication state.
        /// </summary>
        private async Task LoadUserInformation()
        {
            try
            {
                var authState = await _AuthenticationStateProvider.GetAuthenticationStateAsync();
                var user = authState.User;

                IsAuthenticated = user.Identity?.IsAuthenticated ?? false;

                if (IsAuthenticated)
                {
                    UserDisplayName = user.FindFirst(ClaimTypes.Name)?.Value ?? 
                                     user.FindFirst("name")?.Value ?? 
                                     user.FindFirst("preferred_username")?.Value ??
                                     user.FindFirst("given_name")?.Value ??
                                     user.FindFirst("displayName")?.Value ??
                                     user.Identity?.Name ??
                                     "User";
                    
                    UserEmail = user.FindFirst(ClaimTypes.Email)?.Value ?? 
                               user.FindFirst("email")?.Value ?? 
                               string.Empty;
                    
                    UserRoles = user.Claims
                        .Where(c => c.Type == ClaimTypes.Role || c.Type == "role")
                        .Select(c => c.Value)
                        .Distinct()
                        .OrderBy(r => r)
                        .ToList();
                }
            }
            catch (Exception ex)
            {
                _Logger.LogError("Error loading user information", ex);
            }
        }

        /// <summary>
        /// Gets the user's initials for the avatar display.
        /// </summary>
        /// <returns>The user's initials or a default icon.</returns>
        private string GetUserInitials()
        {
            if (string.IsNullOrWhiteSpace(UserDisplayName))
                return "U";

            var parts = UserDisplayName.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 2)
                return $"{parts[0][0]}{parts[^1][0]}".ToUpper();
            
            return UserDisplayName.Length > 0 ? UserDisplayName[0].ToString().ToUpper() : "U";
        }

        /// <summary>
        /// Handles theme change events.
        /// </summary>
        private void HandleThemeChange()
        {
            InvokeAsync(() =>
            {
                StateHasChanged();
            });
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Toggles the application theme between light and dark mode.
        /// </summary>
        protected async Task ToggleTheme()
        {
            try
            {
                await _ThemeService.ToggleThemeAsync();
            }
            catch (Exception ex)
            {
                _Logger.LogError("Error occurred while toggling theme", ex);
            }
        }


        /// <summary>
        /// Opens the notification settings panel.
        /// </summary>
        protected void OpenNotificationSettings()
        {
            try
            {
                // TODO: Implement when NotificationSettingsPanel is available
                // Will open a dialog or navigate to notification settings
            }
            catch (Exception ex)
            {
                _Logger.LogError("Error occurred while opening notification settings", ex);
            }
        }

        /// <summary>
        /// Logs out the current user and navigates to the logout page.
        /// </summary>
        protected void Logout()
        {
            try
            {
                _NavigationManager.NavigateTo($"{_AppConfiguration.BaseUrl}account/logout", forceLoad: true);
            }
            catch (Exception ex)
            {
                _Logger.LogError("Error occurred during logout", ex);
            }
        }

        /// <summary>
        /// Toggles the expanded state of the roles section.
        /// </summary>
        protected void ToggleRolesExpanded()
        {
            RolesExpanded = !RolesExpanded;
        }

        /// <summary>
        /// Loads application profiles and builds the role icon lookup.
        /// </summary>
        private async Task LoadApplicationProfiles()
        {
            try
            {
                var profiles = await _ApplicationProfileService.GetActiveApplicationProfilesAsync();
                
                // Build lookup dictionary where role name matches application name
                RoleIconLookup.Clear();
                foreach (var profile in profiles)
                {
                    if (!string.IsNullOrEmpty(profile.ApplicationName) && !string.IsNullOrEmpty(profile.IconUrl))
                    {
                        RoleIconLookup[profile.ApplicationName] = profile.IconUrl;
                    }
                }
            }
            catch (Exception ex)
            {
                _Logger.LogError("Error loading application profiles for role icons", ex);
                // Continue without icons rather than breaking the component
            }
        }

        /// <summary>
        /// Gets the appropriate icon for a role based on ApplicationNotificationProfile lookup.
        /// </summary>
        /// <param name="role">The role name</param>
        /// <returns>The Material icon string</returns>
        protected string GetRoleIcon(string role)
        {
            // Check if role matches any application name in the lookup
            if (RoleIconLookup.TryGetValue(role, out var iconName))
            {
                // Get the icon value from Icons.Material.Filled using reflection
                var iconField = typeof(Icons.Material.Filled).GetField(iconName);
                if (iconField != null)
                {
                    return iconField.GetValue(null)?.ToString() ?? DefaultRoleIcon;
                }
            }
            
            // Return default RBAC/AD related icon
            return DefaultRoleIcon;
        }
        #endregion
    }
}