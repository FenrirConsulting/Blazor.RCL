using Microsoft.AspNetCore.Components;

namespace Blazor.RCL.UIComponents.Components.Admin
{
    public partial class AdminFilterBar : ComponentBase
    {
        [Parameter] public bool ShowSearch { get; set; } = true;
        [Parameter] public string? SearchText { get; set; }
        [Parameter] public EventCallback<string> SearchTextChanged { get; set; }
        [Parameter] public RenderFragment? ChildContent { get; set; }
    }
} 