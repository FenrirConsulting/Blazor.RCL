using Blazor.RCL.Automation.AutomationRequest.Interfaces;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Blazor.RCL.Automation.AutomationRequest
{
    /// <summary>
    /// Represents a request model for enable/disable operations.
    /// </summary>
    public class RequestModel<T> where T : IRequestPayloadModel
    {
        #region Properties

        /// <summary>
        /// Gets or sets the source of the request.
        /// </summary>
        [JsonPropertyName("source")]
        public string Source { get; set; } = "AutomationTools";

        /// <summary>
        /// Gets or sets the source identifier.
        /// </summary>
        [JsonPropertyName("sourceId")]
        public string? SourceId { get; set; }

        /// <summary>
        /// Gets or sets the request identifier.
        /// </summary>
        [JsonPropertyName("request")]
        public string? Request { get; set; }

        /// <summary>
        /// Gets or sets the request item identifier.
        /// </summary>
        [JsonPropertyName("requestItem")]
        public string? RequestItem { get; set; }

        /// <summary>
        /// Gets or sets the batch ID Identifier (not sent in API Request).
        /// </summary>
        [JsonIgnore]
        public string BatchId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the catalog item.
        /// </summary>
        [JsonPropertyName("catalogItem")]
        public string? CatalogItem { get; set; }

        /// <summary>
        /// Gets or sets the access type.
        /// </summary>
        [JsonPropertyName("accessType")]
        public string? AccessType { get; set; }

        /// <summary>
        /// Gets or sets the access subtype.
        /// </summary>
        [JsonPropertyName("accessSubType")]
        public string? AccessSubType { get; set; }

        /// <summary>
        /// Gets or sets the item data. This is dynamic to the Request Payload used for ItemData.
        /// </summary>
        [JsonPropertyName("itemData")]
        public T ItemData { get; set; }

        /// <summary>
        /// Gets or sets the employee ID associated with the request.
        /// This property is not sent in the API request but is stored in the database.
        /// </summary>
        [JsonIgnore]
        public string? EmployeeID { get; set; }

        /// <summary>
        /// Gets or sets the SAM account associated with the request.
        /// This property is not sent in the API request but is stored in the database.
        /// </summary>
        [JsonIgnore]
        public string? SAMAccount { get; set; }

        /// <summary>
        /// Gets or sets the domain associated with the account in the request.
        /// This property is not sent in the API request but is stored in the database.
        /// </summary>
        [JsonIgnore]
        public string? Domain { get; set; }

        /// <summary>
        /// Gets or sets the comments for the request.
        /// This property is not sent in the API request but is stored in the database.
        /// </summary>
        [JsonIgnore]
        public string? Comments { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestModel"/> class.
        /// </summary>
        public RequestModel()
        {
            ItemData = Activator.CreateInstance<T>();
        }
        #endregion
    }
}