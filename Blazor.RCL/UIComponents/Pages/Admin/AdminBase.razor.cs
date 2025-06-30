using Microsoft.AspNetCore.Components;
using MudBlazor;
using Blazor.RCL.UIComponents.Pages.Admin;
using Blazor.RCL.NLog.LogService.Interface;
using Microsoft.AspNetCore.Components.Web;

namespace Blazor.RCL.UIComponents.Pages.Admin
{
    public partial class AdminBase : BaseAdminComponent
    {
        [Parameter]
        public string? Title { get; set; }

        [Parameter]
        public RenderFragment? HeaderContent { get; set; }

        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        [Parameter]
        public RenderFragment? FooterContent { get; set; }

        [Parameter]
        public bool Dense { get; set; }

        [Parameter]
        public bool DisableElevation { get; set; }

        [Parameter]
        public string? PageTitle { get; set; }

        private ErrorBoundary? errorBoundary;

        protected override void OnInitialized()
        {
            Title ??= PageTitle;
            base.OnInitialized();
        }

        protected override async Task OnInitializeAsync()
        {
            try 
            {
                // Add any specific initialization logic here
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                Logger.LogError("Error during admin base initialization", ex);
                throw;
            }
        }

        protected override void OnParametersSet()
        {
            errorBoundary?.Recover();
        }
    }
} 