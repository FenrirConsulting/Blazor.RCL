using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blazor.RCL.Automation.AutomationBatch
{
    /// <summary>
    /// Represents an access type code (Do Not Use) in the Automation batch processing system.
    /// </summary>
    [Table("AccessTypeCode_DBU")]
    public class AccessTypeCode_DBU
    {
        #region Properties

        /// <summary>
        /// Gets or sets the unique identifier for the access subtype code.
        /// </summary>
        [Key]
        public short AccessTypeCodePK { get; set; }

        /// <summary>
        /// Gets or sets the description of the access subtype.
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string? AccessTypeDesc { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the access subtype code is active.
        /// </summary>
        [Required]
        public bool ActiveInd { get; set; }

        /// <summary>
        /// Gets or sets the username of the user who created the record.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string? AuditAddUserName { get; set; }

        /// <summary>
        /// Gets or sets the date when the record was created.
        /// </summary>
        [Required]
        public DateTime AuditAddDate { get; set; }

        /// <summary>
        /// Gets or sets the username of the user who last modified the record.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string? AuditChangeUserName { get; set; }

        /// <summary>
        /// Gets or sets the date when the record was last modified.
        /// </summary>
        [Required]
        public DateTime AuditChangeDate { get; set; }

        #endregion
    }
}
