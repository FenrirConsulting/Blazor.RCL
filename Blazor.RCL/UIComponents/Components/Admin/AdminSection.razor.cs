using Microsoft.AspNetCore.Components;

namespace Blazor.RCL.UIComponents.Components.Admin
{
    public partial class AdminSection : ComponentBase
    {
        [Parameter] public string? Title { get; set; }
        [Parameter] public RenderFragment? HeaderContent { get; set; }
        [Parameter] public RenderFragment? ChildContent { get; set; }
        [Parameter] public int? Elevation { get; set; }
    }
} 