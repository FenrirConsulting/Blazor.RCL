using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blazor.RCL.Automation.AutomationRequest
{
    /// <summary>
    /// Represents an access type code.
    /// </summary>
    [Table("AccessTypeCode")]
    public class AccessTypeCode
    {
        /// <summary>
        /// Gets or sets the primary key for the access type code.
        /// </summary>
        [Key]
        public short AccessTypeCodePK { get; set; }

        /// <summary>
        /// Gets or sets the description of the access type.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string AccessTypeDesc { get; set; } = string.Empty;

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