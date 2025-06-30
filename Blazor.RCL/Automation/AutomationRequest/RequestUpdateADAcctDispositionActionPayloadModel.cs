using Blazor.RCL.Automation.AutomationRequest.Interfaces;
using System.Text.Json.Serialization;

namespace Blazor.RCL.Automation.AutomationRequest
{
    /// <summary>
    /// Represents the payload model for updating AD Account Disposition actions.
    /// </summary>
    public class RequestUpdateADAcctDispositionActionPayloadModel : IRequestPayloadModel
    {
        /// <summary>
        /// Gets or sets the action identifier.
        /// </summary>
        [JsonPropertyName("ActionID")]
        public int ActionID { get; set; }

        /// <summary>
        /// Gets or sets the action.
        /// </summary>
        [JsonPropertyName("Action")]
        public string? Action { get; set; }

        /// <summary>
        /// Gets or sets the final action.
        /// </summary>
        [JsonPropertyName("FinalAction")]
        public string? FinalAction { get; set; }

        /// <summary>
        /// Gets or sets the action result.
        /// </summary>
        [JsonPropertyName("ActionResult")]
        public string? ActionResult { get; set; }

        /// <summary>
        /// Gets or sets the action result comment.
        /// </summary>
        [JsonPropertyName("ActionResultComment")]
        public string? ActionResultComment { get; set; }
    }
}
