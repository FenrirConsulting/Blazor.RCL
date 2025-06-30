using Microsoft.AspNetCore.Components;
using Newtonsoft.Json.Linq;

namespace Blazor.RCL.UIComponents.Components.JSON
{
    public partial class JsonNode : ComponentBase
    {
        [Parameter] public JToken Value { get; set; } = new JValue("");
        [Parameter] public EventCallback<JToken> OnValueChanged { get; set; }

        private async Task HandleValueChanged(JToken newValue)
        {
            if (OnValueChanged.HasDelegate)
            {
                await OnValueChanged.InvokeAsync(newValue);
            }
        }
    }
} 