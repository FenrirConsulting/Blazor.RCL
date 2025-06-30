using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Blazor.RCL.UIComponents.Components.Admin
{
    public partial class AdminDialog : ComponentBase
    {
        [Parameter] public bool IsVisible { get; set; }
        [Parameter] public EventCallback<bool> IsVisibleChanged { get; set; }
        [Parameter] public bool IsNew { get; set; }
        [Parameter] public string Title { get; set; } = "";
        [Parameter] public DialogOptions Options { get; set; } = new() { MaxWidth = MaxWidth.Medium, FullWidth = true };
        [Parameter] public RenderFragment? DialogContent { get; set; }
        [Parameter] public EventCallback OnSave { get; set; }
        [Parameter] public EventCallback OnCancel { get; set; }

        private async Task HandleSave()
        {
            if (OnSave.HasDelegate)
            {
                await OnSave.InvokeAsync();
            }
        }

        private async Task HandleCancel()
        {
            if (OnCancel.HasDelegate)
            {
                await OnCancel.InvokeAsync();
            }
        }
    }
} 