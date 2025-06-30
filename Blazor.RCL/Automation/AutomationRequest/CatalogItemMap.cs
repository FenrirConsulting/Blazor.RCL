using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blazor.RCL.Automation.AutomationRequest
{
    /// <summary>
    /// Represents a mapping of catalog items.
    /// </summary>
    [Table("CatalogItemMap")]
    public class CatalogItemMap
    {
        /// <summary>
        /// Gets or sets the primary key for the catalog item map.
        /// </summary>
        [Key]
        public short CatalogItemMapPK { get; set; }

        /// <summary>
        /// Gets or sets the foreign key for the catalog item code.
        /// </summary>
        [Required]
        public short CatalogItemCodeFK { get; set; }

        /// <summary>
        /// Gets or sets the foreign key for the valid access type code.
        /// </summary>
        [Required]
        public short ValidAccessTypeCodeFK { get; set; }

        /// <summary>
        /// Gets or sets the foreign key for the metrics access type code.
        /// </summary>
        [Required]
        public short MetricsAccessTypeCodeFK { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the catalog item map is active.
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