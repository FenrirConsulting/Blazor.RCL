using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blazor.RCL.Automation.AutomationApp
{
    /// <summary>
    /// Represents a required action code.
    /// </summary>
    [Table("RequiredActionCode")]
    public class RequiredActionCode
    {
        /// <summary>
        /// Gets or sets the primary key for the required action code.
        /// </summary>
        [Key]
        public short RequiredActionCodePK { get; set; }

        /// <summary>
        /// Gets or sets the description of the required action.
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string RequiredActionDesc { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the associated batch type code.
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string AssociatedBatchTypeCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the required action code is active.
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