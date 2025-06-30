using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Blazor.RCL.UIComponents.Components.Admin
{
    public partial class AdminStatusBadge : ComponentBase
    {
        [Parameter] public string Text { get; set; } = string.Empty;
        [Parameter] public string Status { get; set; } = string.Empty;

        private Color GetStatusColor() => (Status?.ToLowerInvariant() ?? string.Empty) switch
        {
            "active" or "enabled" => Color.Success,
            "inactive" or "disabled" => Color.Error,
            "pending" or "inprogress" => Color.Warning,
            _ => Color.Default
        };
    }
} 