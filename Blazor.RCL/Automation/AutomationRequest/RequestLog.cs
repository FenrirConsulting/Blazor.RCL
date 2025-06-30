using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blazor.RCL.Automation.AutomationRequest
{
    /// <summary>
    /// Represents a log entry for requests.
    /// </summary>
    [Table("RequestLog", Schema = "dbo")]
    public class RequestLog
    {
        /// <summary>
        /// Gets or sets the primary key for the request log.
        /// </summary>
        [Key]
        public int RequestLogPK { get; set; }

        /// <summary>
        /// Gets or sets the source of the request.
        /// </summary>
        [StringLength(100)]
        public string? Source { get; set; }

        /// <summary>
        /// Gets or sets the source ID of the request.
        /// </summary>
        [StringLength(100)]
        public string? SourceId { get; set; }

        /// <summary>
        /// Gets or sets the request details.
        /// </summary>
        [StringLength(100)]
        public string? Request { get; set; }

        /// <summary>
        /// Gets or sets the request item.
        /// </summary>
        [StringLength(100)]
        public string? RequestItem { get; set; }

        /// <summary>
        /// Gets or sets the catalog item.
        /// </summary>
        [StringLength(100)]
        public string? CatalogItem { get; set; }

        /// <summary>
        /// Gets or sets the access type.
        /// </summary>
        [StringLength(100)]
        public string? AccessType { get; set; }

        /// <summary>
        /// Gets or sets the access subtype.
        /// </summary>
        [StringLength(100)]
        public string? AccessSubtype { get; set; }

        /// <summary>
        /// Gets or sets the foreign key for valid access type code.
        /// </summary>
        public short? ValidAccessTypeCodeFK { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the request.
        /// </summary>
        [Required]
        public Guid Guid { get; set; }

        /// <summary>
        /// Gets or sets the foreign key for request status code.
        /// </summary>
        [Required]
        public short RequestStatusCodeFK { get; set; }

        /// <summary>
        /// Gets or sets the date of the request status.
        /// </summary>
        [Required]
        public DateTime RequestStatusDate { get; set; }

        /// <summary>
        /// Gets or sets the comments for the request status.
        /// </summary>
        public string? RequestStatusComments { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the request is in test mode.
        /// </summary>
        [Required]
        public bool TestModeInd { get; set; }

        /// <summary>
        /// Gets or sets the pre-approval validation request indicator.
        /// </summary>
        public int? PreApprovalValidationRequestInd { get; set; }

        /// <summary>
        /// Gets or sets the username of the user who added the record.
        /// </summary>
        [Required]
        [StringLength(100)]
        public string AuditAddUserName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the date when the record was added.
        /// </summary>
        [Required]
        public DateTime AuditAddDate { get; set; }

        /// <summary>
        /// Gets or sets the username of the user who last changed the record.
        /// </summary>
        [Required]
        [StringLength(100)]
        public string AuditChangeUserName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the date when the record was last changed.
        /// </summary>
        [Required]
        public DateTime AuditChangeDate { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this is a sync request.
        /// </summary>
        public bool? SyncRequestInd { get; set; }

        /// <summary>
        /// Gets or sets the HTTP status code.
        /// </summary>
        public short? HTTPStatusCode { get; set; }
    }
}