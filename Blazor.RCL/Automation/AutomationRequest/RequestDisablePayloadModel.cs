using Blazor.RCL.Automation.AutomationRequest.Interfaces;
using System.Text.Json.Serialization;

namespace Blazor.RCL.Automation.AutomationRequest
{
    /// <summary>
    /// Represents the item data for a request.
    /// </summary>
    public class RequestDisablePayloadModel : IRequestPayloadModel
    {
        /// <summary>
        /// Gets or sets the employee identifier.
        /// </summary>
        [JsonPropertyName("EmployeeId")]
        public string? EmployeeId { get; set; }

        /// <summary>
        /// Gets or sets the account name.
        /// </summary>
        [JsonPropertyName("AccountName")]
        public string? AccountName { get; set; }

        /// <summary>
        /// Gets or sets the account domain.
        /// </summary>
        [JsonPropertyName("AccountDomain")]
        public string? AccountDomain { get; set; }
    }
}
