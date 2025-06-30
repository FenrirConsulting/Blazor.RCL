using Blazor.RCL.Automation.AutomationRequest.Interfaces;
using System.Text.Json.Serialization;

namespace Blazor.RCL.Automation.AutomationRequest
{
    /// <summary>
    /// Represents the payload model for retrieving AD Account Disposition configurations.
    /// </summary>
    public class RequestGetADAcctDispositionConfigPayloadModel : IRequestPayloadModel
    {
        /// <summary>
        /// Gets or sets the configuration type.
        /// </summary>
        [JsonPropertyName("ConfigType")]
        public string? ConfigType { get; set; }

        /// <summary>
        /// Gets or sets the configuration name.
        /// </summary>
        [JsonPropertyName("Name")]
        public string? Name { get; set; }
    }
}
