using Microsoft.AspNetCore.Components;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace Blazor.RCL.UIComponents.Components.JSON
{
    public partial class JsonObject : ComponentBase
    {
        [Parameter] public JToken Value { get; set; } = new JObject();
        [Parameter] public EventCallback<JToken> OnValueChanged { get; set; }

        private List<(string Name, JToken Value)> Properties => 
            ((JObject)Value).Properties()
                .Select(p => (p.Name, p.Value))
                .ToList();

        private string GenerateUniquePropertyName(string baseName = "newProperty")
        {
            var obj = (JObject)Value;
            if (!obj.ContainsKey(baseName))
                return baseName;

            int counter = 1;
            string newName;
            do
            {
                newName = $"{baseName}{counter}";
                counter++;
            } while (obj.ContainsKey(newName));

            return newName;
        }

        private async Task HandlePropertyNameChanged((string Name, JToken Value) property)
        {
            var obj = (JObject)Value;
            obj.Remove(property.Name);
            obj.Add(property.Name, property.Value);
            await NotifyValueChanged();
        }

        private async Task HandlePropertyValueChanged(string propertyName, JToken newValue)
        {
            var obj = (JObject)Value;
            obj[propertyName] = newValue;
            await NotifyValueChanged();
        }

        private async Task AddProperty(JTokenType type)
        {
            var obj = (JObject)Value;
            JToken newValue = type switch
            {
                JTokenType.Object => new JObject(),
                JTokenType.Array => new JArray(),
                _ => new JValue(string.Empty)
            };
            obj.Add(GenerateUniquePropertyName(), newValue);
            await NotifyValueChanged();
        }

        private async Task RemoveProperty(string propertyName)
        {
            var obj = (JObject)Value;
            obj.Remove(propertyName);
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