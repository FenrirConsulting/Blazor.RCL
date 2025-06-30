using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blazor.RCL.Automation.AutomationRequest
{
    /// <summary>
    /// Represents a parsed JSON request.
    /// </summary>
    [Table("RequestJsonParsed")]
    public class RequestJsonParsed
    {
        /// <summary>
        /// Gets or sets the primary key for the parsed JSON request.
        /// </summary>
        [Key]
        public int RequestJsonParsedPK { get; set; }

        /// <summary>
        /// Gets or sets the foreign key for the request log.
        /// </summary>
        [Required]
        public int RequestLogFK { get; set; }

        /// <summary>
        /// Gets or sets the row ID of the parsed JSON.
        /// </summary>
        [Required]
        public short RowId { get; set; }

        /// <summary>
        /// Gets or sets the parent row ID if applicable.
        /// </summary>
        public short? ParentRowId { get; set; }

        /// <summary>
        /// Gets or sets the path of the parsed JSON.
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string Path { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the key of the parsed JSON.
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Key { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the foreign key for the data type code.
        /// </summary>
        [Required]
        public short DataTypeCodeFK { get; set; }

        /// <summary>
        /// Gets or sets the value of the parsed JSON.
        /// </summary>
        public string? Value { get; set; }

        /// <summary>
        /// Gets or sets the element count if applicable.
        /// </summary>
        public int? ElementCount { get; set; }

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