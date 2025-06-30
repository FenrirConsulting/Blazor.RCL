using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blazor.RCL.Automation.AutomationApp
{
    /// <summary>
    /// Represents a line of business code.
    /// </summary>
    [Table("LineofBusinessCode")]
    public class LineofBusinessCode
    {
        /// <summary>
        /// Gets or sets the primary key for the line of business code.
        /// </summary>
        [Key]
        public short LineOfBusinessCodePK { get; set; }

        /// <summary>
        /// Gets or sets the description of the line of business.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string LineOfBusinessDesc { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the line of business is active.
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