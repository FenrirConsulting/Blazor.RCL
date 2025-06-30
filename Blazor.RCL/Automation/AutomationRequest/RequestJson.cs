using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blazor.RCL.Automation.AutomationRequest
{
    /// <summary>
    /// Represents a JSON request.
    /// </summary>
    [Table("RequestJson")]
    public class RequestJson
    {
        /// <summary>
        /// Gets or sets the primary key for the request JSON.
        /// </summary>
        [Key]
        public int RequestJsonPK { get; set; }

        /// <summary>
        /// Gets or sets the foreign key for the request log.
        /// </summary>
        [Required]
        public int RequestLogFK { get; set; }

        /// <summary>
        /// Gets or sets the JSON content of the request.
        /// </summary>
        [Required]
        [Column("RequestJson")] // Specify the exact column name in the database
        public string JsonContent { get; set; } = string.Empty; // Renamed to avoid conflict

        /// <summary>
        /// Gets or sets the JSON content before transformation.
        /// </summary>
        public string? RequestJsonBeforeTransform { get; set; }

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