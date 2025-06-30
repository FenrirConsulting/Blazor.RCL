using Microsoft.AspNetCore.Components;
using MudBlazor;
using System;
using System.Threading.Tasks;
using Blazor.RCL.Application.Models.Notifications;

namespace Blazor.RCL.UIComponents.Components.Admin.Dialogs
{
    public partial class PreviewTemplateDialog : ComponentBase
    {
        [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = default!;
        
        [Parameter] public string TemplateName { get; set; } = string.Empty;
        [Parameter] public RenderedEmailContent? PreviewContent { get; set; }
        [Parameter] public Func<Task>? OnSendTestEmail { get; set; }
        
        private void Close() => MudDialog.Close();
        
        private async Task SendTestEmail()
        {
            if (OnSendTestEmail != null)
            {
                await OnSendTestEmail();
            }
        }
    }
}