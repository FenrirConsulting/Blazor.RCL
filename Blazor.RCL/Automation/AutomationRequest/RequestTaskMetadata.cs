using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blazor.RCL.Automation.AutomationRequest
{
    /// <summary>
    /// Represents metadata for request tasks.
    /// </summary>
    [Table("RequestTaskMetaData")]
    public class RequestTaskMetaData
    {
        /// <summary>
        /// Gets or sets the primary key for the request task metadata.
        /// </summary>
        [Key]
        public int RequestTaskMetadataPK { get; set; }

        /// <summary>
        /// Gets or sets the foreign key for the request task.
        /// </summary>
        [Required]
        public int RequestTaskFK { get; set; }

        /// <summary>
        /// Gets or sets the key for the metadata.
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string MetaDataKey { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the value for the metadata.
        /// </summary>
        [Required]
        public string MetaDataValue { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the encrypted value for the metadata.
        /// </summary>
        public byte[]? MetaDataValueEncrypted { get; set; }

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