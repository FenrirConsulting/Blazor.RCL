using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blazor.RCL.Automation.AutomationRequest
{
    /// <summary>
    /// Represents valid access type codes.
    /// </summary>
    [Table("ValidAccessTypeCode")]
    public class ValidAccessTypeCode
    {
        /// <summary>
        /// Gets or sets the primary key for the valid access type code.
        /// </summary>
        [Key]
        public short ValidAccessTypeCodePK { get; set; }

        /// <summary>
        /// Gets or sets the foreign key for access type code.
        /// </summary>
        [Required]
        public short AccessTypeCodeFK { get; set; }

        /// <summary>
        /// Gets or sets the foreign key for access subtype code.
        /// </summary>
        [Required]
        public short AccessSubTypeCodeFK { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether request transformation is required.
        /// </summary>
        public bool? RequiresRequestTransformInd { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether task lineup is required.
        /// </summary>
        public bool? RequiresTaskLineupInd { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the access type code is active.
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