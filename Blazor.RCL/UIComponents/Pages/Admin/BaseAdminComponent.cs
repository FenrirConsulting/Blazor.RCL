using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;
using System.Threading;
using Blazor.RCL.NLog.LogService.Interface;

namespace Blazor.RCL.UIComponents.Pages.Admin
{
    public abstract class BaseAdminComponent : ComponentBase
    {
        [Inject]
        protected NavigationManager NavigationManager { get; set; } = default!;

        [Inject]
        protected ILogHelper Logger { get; set; } = default!;

        // Initialize properties to prevent null references
        protected bool IsLoading { get; set; }
        protected string ErrorMessage { get; set; } = string.Empty;
        protected bool HasError => !string.IsNullOrEmpty(ErrorMessage);
        protected bool IsAuthorized { get; set; }
        protected string PageTitle { get; set; } = string.Empty;
        protected bool ShowMetrics { get; set; }
        protected bool ShowHealthStatus { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await InitializeAsync();
        }

        protected virtual async Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            IsLoading = true;
            try
            {
                await CheckAuthorization();
                if (IsAuthorized)
                {
                    await OnInitializeAsync();
                }
                else
                {
                    ErrorMessage = "Unauthorized access";
                    NavigationManager.NavigateTo("/unauthorized", forceLoad: true);
                }
            }
            catch (OperationCanceledException)
            {
                Logger.LogMessage("Operation was canceled");
                throw;
            }
            catch (Exception ex)
            {
                ErrorMessage = "An error occurred during initialization.";
                Logger.LogError(ErrorMessage, ex);
                throw;
            }
            finally
            {
                IsLoading = false;
                StateHasChanged();
            }
        }

        protected virtual async Task CheckAuthorization()
        {
            // Default implementation - override in specific admin pages
            IsAuthorized = true;
            await Task.CompletedTask;
        }

        protected abstract Task OnInitializeAsync();
    }
}