using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blazor.RCL.Automation.AutomationRequest
{
    /// <summary>
    /// Represents a mapping of source catalog items.
    /// </summary>
    [Table("SourceCatalogItemMap")]
    public class SourceCatalogItemMap
    {
        /// <summary>
        /// Gets or sets the primary key for the source catalog item map.
        /// </summary>
        [Key]
        public int SourceCatalogItemMapPK { get; set; }

        /// <summary>
        /// Gets or sets the foreign key for source information.
        /// </summary>
        [Required]
        public int SourceInformationFK { get; set; }

        /// <summary>
        /// Gets or sets the foreign key for catalog item code.
        /// </summary>
        [Required]
        public short CatalogItemCodeFK { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the mapping is active.
        /// </summary>
        public bool ActiveInd { get; set; }

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