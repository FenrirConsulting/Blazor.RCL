using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blazor.RCL.Automation.AutomationRequest
{
    /// <summary>
    /// Represents a lookup key code entry.
    /// </summary>
    [Table("LookupKeyCode")]
    public class LookupKeyCode
    {
        /// <summary>
        /// Gets or sets the primary key for the lookup key code.
        /// </summary>
        [Key]
        public short LookupKeyCodePK { get; set; }

        /// <summary>
        /// Gets or sets the description of the lookup path.
        /// </summary>
        public string? LookupPathDesc { get; set; }

        /// <summary>
        /// Gets or sets the description of the lookup key.
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string LookupKeyDesc { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the lookup key code is active.
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