using Microsoft.AspNetCore.Components;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Blazor.RCL.UIComponents.Components.JSON
{
    public partial class JsonArray : ComponentBase
    {
        [Parameter] public JToken Value { get; set; } = new JArray();
        [Parameter] public EventCallback<JToken> OnValueChanged { get; set; }

        private List<JToken> ArrayItems => ((JArray)Value).ToList();

        private async Task HandleItemValueChanged(int index, JToken newValue)
        {
            var array = (JArray)Value;
            array[index] = newValue;
            await NotifyValueChanged();
        }

        private async Task AddItem(JTokenType type)
        {
            var array = (JArray)Value;
            JToken newValue = type switch
            {
                JTokenType.Object => new JObject(),
                JTokenType.Array => new JArray(),
                _ => new JValue(string.Empty)
            };
            array.Add(newValue);
            await NotifyValueChanged();
        }

        private async Task RemoveItem(int index)
        {
            var array = (JArray)Value;
            array.RemoveAt(index);
            await NotifyValueChanged();
        }

        private async Task NotifyValueChanged()
        {
            if (OnValueChanged.HasDelegate)
            {
                await OnValueChanged.InvokeAsync(Value);
            }
        }
    }
} 