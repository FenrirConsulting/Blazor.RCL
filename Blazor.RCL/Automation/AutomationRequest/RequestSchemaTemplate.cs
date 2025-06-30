using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blazor.RCL.Automation.AutomationRequest
{
    /// <summary>
    /// Represents a template for request schemas.
    /// </summary>
    [Table("RequestSchemaTemplate")]
    public class RequestSchemaTemplate
    {
        /// <summary>
        /// Gets or sets the primary key for the request schema template.
        /// </summary>
        [Key]
        public int RequestSchemaTemplatePK { get; set; }

        /// <summary>
        /// Gets or sets the foreign key for valid access type code.
        /// </summary>
        [Required]
        public short ValidAccessTypeCodeFK { get; set; }

        /// <summary>
        /// Gets or sets the schema definition.
        /// </summary>
        [Required]
        public string SchemaDefinition { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the sample data for the schema.
        /// </summary>
        [Required]
        public string SampleData { get; set; } = string.Empty;

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