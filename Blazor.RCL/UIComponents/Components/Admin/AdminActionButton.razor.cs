using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Blazor.RCL.UIComponents.Components.Admin
{
    public partial class AdminActionButton : ComponentBase
    {
        [Parameter] public string? Text { get; set; }
        [Parameter] public string? Icon { get; set; }
        [Parameter] public Color ButtonColor { get; set; } = Color.Primary;
        [Parameter] public Variant ButtonVariant { get; set; } = Variant.Filled;
        [Parameter] public bool Disabled { get; set; }
        [Parameter] public EventCallback OnClick { get; set; }

        private async Task HandleClick()
        {
            if (OnClick.HasDelegate)
            {
                await OnClick.InvokeAsync();
            }
        }
    }
} 