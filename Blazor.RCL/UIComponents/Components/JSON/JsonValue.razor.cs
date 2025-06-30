using Microsoft.AspNetCore.Components;
using Newtonsoft.Json.Linq;

namespace Blazor.RCL.UIComponents.Components.JSON
{
    public partial class JsonValue : ComponentBase
    {
        [Parameter] public JToken Value { get; set; } = new JValue("");
        [Parameter] public EventCallback<JToken> OnValueChanged { get; set; }

        private string? _stringValue;

        protected override void OnParametersSet()
        {
            _stringValue = Value.ToString();
        }

        private async Task HandleValueChanged()
        {
            if (OnValueChanged.HasDelegate)
            {
                // Try to parse as number or boolean first
                if (double.TryParse(_stringValue, out double number))
                {
                    await OnValueChanged.InvokeAsync(new JValue(number));
                }
                else if (bool.TryParse(_stringValue?.ToLower(), out bool boolean))
                {
                    await OnValueChanged.InvokeAsync(new JValue(boolean));
                }
                else
                {
                    await OnValueChanged.InvokeAsync(new JValue(_stringValue));
                }
            }
        }

        private string GetValueTypeClass()
        {
            if (string.IsNullOrEmpty(_stringValue)) return "";
            if (bool.TryParse(_stringValue?.ToLower(), out _)) return "value-boolean";
            if (double.TryParse(_stringValue, out _)) return "value-number";
            return "value-string";
        }
    }
} 