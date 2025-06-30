using Blazor.RCL.Automation.AutomationRequest.Interfaces;
using System.Text.Json.Serialization;

namespace Blazor.RCL.Automation.AutomationRequest
{
    /// <summary>
    /// Represents the payload model for a disable never logon request.
    /// </summary>
    public class RequestDisableNeverLogonPayloadModel : IRequestPayloadModel
    {
        /// <summary>
        /// Gets or sets the employee identifier.
        /// </summary>
        [JsonPropertyName("EmployeeId")]
        public string? EmployeeId { get; set; }

        /// <summary>
        /// Gets or sets the list of accounts to disable.
        /// </summary>
        [JsonPropertyName("DisableAccounts")]
        public List<DisableAccount> DisableAccounts { get; set; } = new List<DisableAccount>();

        /// <summary>
        /// Gets or sets the list of attributes to set.
        /// </summary>
        [JsonPropertyName("SetAttributes")]
        public List<SetAttribute> SetAttributes { get; set; } = new List<SetAttribute>();

        /// <summary>
        /// Represents an account to be disabled.
        /// </summary>
        public class DisableAccount
        {
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

        /// <summary>
        /// Represents an attribute to be set.
        /// </summary>
        public class SetAttribute : DisableAccount
        {
            /// <summary>
            /// Gets or sets the attribute name.
            /// </summary>
            [JsonPropertyName("AttributeName")]
            public string? AttributeName { get; set; }

            /// <summary>
            /// Gets or sets the attribute value.
            /// </summary>
            [JsonPropertyName("AttributeValue")]
            public string? AttributeValue { get; set; }
        }
    }
}
