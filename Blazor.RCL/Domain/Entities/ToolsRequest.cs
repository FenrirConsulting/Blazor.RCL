using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blazor.RCL.Domain.Entities
{
    /// <summary>
    /// Represents a tools request record in the database.
    /// </summary>
    public class ToolsRequest
    {
        /// <summary>
        /// Gets or sets the primary key for the tools request.
        /// </summary>
        [Key]
        public int ToolsRequestPK { get; set; }
        
        /// <summary>
        /// Gets or sets the source of the request.
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Source { get; set; }
        
        /// <summary>
        /// Gets or sets the unique source identifier for the request.
        /// </summary>
        [Required]
        public Guid SourceId { get; set; }
        
        /// <summary>
        /// Gets or sets the request identifier (TOOLS + incremented number).
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Request { get; set; }
        
        /// <summary>
        /// Gets or sets the request item identifier (same as Request for grouping).
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string RequestItem { get; set; }
        
        /// <summary>
        /// Gets or sets the catalog item for the request.
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string CatalogItem { get; set; }
        
        /// <summary>
        /// Gets or sets the access type for the request.
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string AccessType { get; set; }
        
        /// <summary>
        /// Gets or sets the access sub-type for the request.
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string AccessSubType { get; set; }
        
        /// <summary>
        /// Gets or sets the JSON item data for the request.
        /// </summary>
        public string? ItemData { get; set; }
        
        /// <summary>
        /// Gets or sets the batch identifier for the request.
        /// </summary>
        [MaxLength(200)]
        public string? BatchId { get; set; }
        
        /// <summary>
        /// Gets or sets the username of the requester.
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string RequestUsername { get; set; }
        
        /// <summary>
        /// Gets or sets the application that originated the request.
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string RequestApplication { get; set; }
        
        /// <summary>
        /// Gets or sets the JSON response from the initial API request.
        /// </summary>
        public string? ResponseJSON { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether this is a disable state request.
        /// </summary>
        public bool? RequestDisableState { get; set; }
        
        /// <summary>
        /// Gets or sets the comments for the request.
        /// </summary>
        [MaxLength(2000)]
        public string? Comments { get; set; }
        
        /// <summary>
        /// Gets or sets the employee ID associated with the request.
        /// </summary>
        [MaxLength(400)]
        public string? EmployeeID { get; set; }
        
        /// <summary>
        /// Gets or sets the SAM account associated with the request.
        /// </summary>
        [MaxLength(400)]
        public string? SAMAccount { get; set; }
        
        /// <summary>
        /// Gets or sets the domain associated with the account in the request.
        /// </summary>
        [MaxLength(400)]
        public string? Domain { get; set; }
        
        /// <summary>
        /// Gets or sets the status code foreign key.
        /// </summary>
        public int? RequestStatusCodeFK { get; set; }
        
        /// <summary>
        /// Gets or sets the date when the status was last updated.
        /// </summary>
        public DateTime? RequestStatusDate { get; set; }
        
        /// <summary>
        /// Gets or sets the status comments.
        /// </summary>
        [MaxLength(2000)]
        public string? RequestStatusComments { get; set; }
        
        /// <summary>
        /// Gets or sets the username of the user who added the record (managed by trigger).
        /// </summary>
        [MaxLength(200)]
        public string? AuditAddUserName { get; set; }
        
        /// <summary>
        /// Gets or sets the date when the record was added (managed by trigger).
        /// </summary>
        public DateTime? AuditAddDate { get; set; }
        
        /// <summary>
        /// Gets or sets the username of the user who last changed the record (managed by trigger).
        /// </summary>
        [MaxLength(200)]
        public string? AuditChangeUserName { get; set; }
        
        /// <summary>
        /// Gets or sets the date when the record was last changed (managed by trigger).
        /// </summary>
        public DateTime? AuditChangeDate { get; set; }
        
        /// <summary>
        /// Navigation property for the status code.
        /// </summary>
        [ForeignKey("RequestStatusCodeFK")]
        public virtual RequestStatusCode? StatusCode { get; set; }
    }
}