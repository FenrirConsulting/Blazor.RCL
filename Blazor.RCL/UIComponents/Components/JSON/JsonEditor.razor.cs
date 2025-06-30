using Microsoft.AspNetCore.Components;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using MudBlazor;

namespace Blazor.RCL.UIComponents.Components.JSON
{
    public partial class JsonEditor : ComponentBase
    {
        [Parameter] public string? Value { get; set; }
        [Parameter] public EventCallback<string> ValueChanged { get; set; }

        private JToken? _parsedJson;
        private bool _showRawJson;
        private string? _rawJsonText;

        protected override void OnParametersSet()
        {
            try
            {
                _parsedJson = string.IsNullOrEmpty(Value) 
                    ? JToken.Parse("{}") 
                    : JToken.Parse(Value);
                if (_showRawJson)
                {
                    _rawJsonText = _parsedJson.ToString(Formatting.Indented);
                }
            }
            catch
            {
                _parsedJson = null;
            }
        }

        private async Task HandleValueChanged(JToken newValue)
        {
            if (ValueChanged.HasDelegate)
            {
                _parsedJson = newValue;
                await ValueChanged.InvokeAsync(newValue.ToString());
            }
        }

        private void ToggleView()
        {
            _showRawJson = !_showRawJson;
            if (_showRawJson && _parsedJson != null)
            {
                _rawJsonText = _parsedJson.ToString(Formatting.Indented);
            }
        }

        private async Task HandleRawJsonChanged()
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(_rawJsonText))
                {
                    var newJson = JToken.Parse(_rawJsonText);
                    await HandleValueChanged(newJson);
                }
            }
            catch
            {
                // Keep existing value if parse fails
                if (_parsedJson != null)
                {
                    _rawJsonText = _parsedJson.ToString(Formatting.Indented);
                }
            }
        }
    }
} 