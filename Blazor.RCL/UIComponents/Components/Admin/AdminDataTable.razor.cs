using Microsoft.AspNetCore.Components;

namespace Blazor.RCL.UIComponents.Components.Admin
{
    public partial class AdminDataTable<TItem> : ComponentBase
    {
        [Parameter] public IEnumerable<TItem> Items { get; set; } = Array.Empty<TItem>();
        [Parameter] public RenderFragment? HeaderContent { get; set; }
        [Parameter] public RenderFragment<TItem>? RowTemplate { get; set; }
        [Parameter] public bool Dense { get; set; } = true;
        [Parameter] public bool Loading { get; set; }
        
        protected override void OnParametersSet()
        {
            Items ??= Array.Empty<TItem>();
        }
    }
} 