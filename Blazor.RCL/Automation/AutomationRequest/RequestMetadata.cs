using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blazor.RCL.Automation.AutomationRequest
{
    /// <summary>
    /// Represents metadata for requests.
    /// </summary>
    [Table("RequestMetaData")]
    public class RequestMetaData
    {
        /// <summary>
        /// Gets or sets the primary key for the request metadata.
        /// </summary>
        [Key]
        public int RequestMetadataPK { get; set; }

        /// <summary>
        /// Gets or sets the foreign key for the request log.
        /// </summary>
        [Required]
        public int RequestLogFK { get; set; }

        /// <summary>
        /// Gets or sets the HTTP protocol used for the request.
        /// </summary>
        [Required]
        [MaxLength(10)]
        public string HttpProtocol { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the request header.
        /// </summary>
        [Required]
        public string RequestHeader { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the request body.
        /// </summary>
        [Required]
        public string RequestBody { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the browser used for the request.
        /// </summary>
        public string? BrowserUsed { get; set; }

        /// <summary>
        /// Gets or sets the caller URL.
        /// </summary>
        public string? CallerURL { get; set; }

        /// <summary>
        /// Gets or sets the username of the user who added the record.
        /// </summary>
        [Required]
        [MaxLength(100)]
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
        [MaxLength(100)]
        public string AuditChangeUserName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the date when the record was last changed.
        /// </summary>
        [Required]
        public DateTime AuditChangeDate { get; set; }
    }
}