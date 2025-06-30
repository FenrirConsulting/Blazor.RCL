using Microsoft.AspNetCore.Components;

namespace Blazor.RCL.UIComponents.Components.Admin
{
    public partial class AdminExpandableSection<TItem> : ComponentBase
    {
        [Parameter] public string Title { get; set; } = "";
        [Parameter] public string AddButtonText { get; set; } = "Add New";
        [Parameter] public bool FirstSection { get; set; }
        [Parameter] public bool IsExpanded { get; set; }
        [Parameter] public EventCallback<bool> IsExpandedChanged { get; set; }
        [Parameter] public IEnumerable<TItem>? Items { get; set; }
        [Parameter] public EventCallback OnAdd { get; set; }
        [Parameter] public RenderFragment? ChildContent { get; set; }

        private async Task HandleExpand()
        {
            IsExpanded = !IsExpanded;
            if (IsExpandedChanged.HasDelegate)
            {
                await IsExpandedChanged.InvokeAsync(IsExpanded);
            }
        }
    }
} 