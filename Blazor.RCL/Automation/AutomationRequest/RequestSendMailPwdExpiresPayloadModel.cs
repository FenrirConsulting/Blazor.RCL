using Blazor.RCL.Automation.AutomationRequest.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Blazor.RCL.Automation.AutomationRequest
{
    /// <summary>
    /// Represents the payload model for sending a password expiration mail request.
    /// </summary>
    public class RequestSendMailPwdExpiresPayloadModel : IRequestPayloadModel
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

        /// <summary>
        /// Gets or sets the list of email addresses.
        /// </summary>
        [JsonPropertyName("EmailAddress")]
        public List<string> EmailAddress { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the email subject.
        /// </summary>
        [JsonPropertyName("Subject")]
        public string? Subject { get; set; }

        /// <summary>
        /// Gets or sets the email body in HTML format.
        /// </summary>
        [JsonPropertyName("BodyAsHTML")]
        public string? BodyAsHTML { get; set; }
    }
}
