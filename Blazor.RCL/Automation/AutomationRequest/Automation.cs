using System.Text.Json.Serialization;

namespace Blazor.RCL.Automation.AutomationRequest
{
    /// <summary>
    /// Represents a response from the Automation (Identity and Access Protection Framework) API.
    /// </summary>
    public class AutomationResponse
    {
        #region Properties

        /// <summary>
        /// Gets or sets the source identifier of the request.
        /// </summary>
        [JsonPropertyName("sourceId")]
        public string? SourceId { get; set; }

        /// <summary>
        /// Gets or sets the request object.
        /// </summary>
        [JsonPropertyName("request")]
        public object? Request { get; set; }

        /// <summary>
        /// Gets or sets the request item identifier.
        /// </summary>
        [JsonPropertyName("requestItem")]
        public string? RequestItem { get; set; }

        /// <summary>
        /// Gets or sets the access type.
        /// </summary>
        [JsonPropertyName("accessType")]
        public string? AccessType { get; set; }

        /// <summary>
        /// Gets or sets the access subtype.
        /// </summary>
        [JsonPropertyName("accessSubtype")]
        public string? AccessSubtype { get; set; }

        /// <summary>
        /// Gets or sets the request status code.
        /// </summary>
        [JsonPropertyName("requestStatusCode")]
        public int RequestStatusCode { get; set; }

        /// <summary>
        /// Gets or sets the request status description.
        /// </summary>
        [JsonPropertyName("requestStatusDesc")]
        public string? RequestStatusDesc { get; set; }

        /// <summary>
        /// Gets or sets the status comments.
        /// </summary>
        [JsonPropertyName("statusComments")]
        public string? StatusComments { get; set; }

        /// <summary>
        /// Gets or sets the task details.
        /// </summary>
        [JsonPropertyName("taskDetails")]
        public object? TaskDetails { get; set; }

        /// <summary>
        /// Gets or sets the error details.
        /// </summary>
        [JsonPropertyName("errorDetails")]
        public object? ErrorDetails { get; set; }

        /// <summary>
        /// Gets or sets the Automation identifier.
        /// </summary>
        [JsonPropertyName("AutomationId")]
        public string? AutomationId { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AutomationResponse"/> class.
        /// </summary>
        public AutomationResponse()
        {
            // Default constructor
        }

        #endregion
    }
}