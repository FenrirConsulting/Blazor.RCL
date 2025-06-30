using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;

namespace Blazor.RCL.UIComponents.Components.Admin
{
    public partial class AdminDataGrid<TItem> : ComponentBase
    {
        [Parameter]
        public IEnumerable<TItem> Items { get; set; } = Enumerable.Empty<TItem>();

        [Parameter]
        public bool Dense { get; set; } = true;

        [Parameter]
        public bool Loading { get; set; }

        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        protected override void OnParametersSet()
        {
            // Ensure Items is never null
            Items ??= Enumerable.Empty<TItem>();
        }
    }
} 