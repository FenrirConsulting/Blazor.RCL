using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blazor.RCL.Automation.AutomationRequest
{
    /// <summary>
    /// Represents schema data for requests.
    /// </summary>
    [Table("RequestSchemaData")]
    public class RequestSchemaData
    {
        /// <summary>
        /// Gets or sets the primary key for the request schema data.
        /// </summary>
        [Key]
        public int RequestSchemaDataPK { get; set; }

        /// <summary>
        /// Gets or sets the foreign key for valid access type code.
        /// </summary>
        [Required]
        public short ValidAccessTypeCodeFK { get; set; }

        /// <summary>
        /// Gets or sets the path of the schema data.
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string Path { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the key of the schema data.
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
        /// Gets or sets the minimum required count for the schema data.
        /// </summary>
        public short? MinRequiredCount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the schema data is required.
        /// </summary>
        [Required]
        public bool RequiredInd { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the schema data is level 1.
        /// </summary>
        [Required]
        public bool Level1Ind { get; set; }

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