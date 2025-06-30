using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blazor.RCL.Domain.Entities.AutomationBatch
{
    /// <summary>
    /// Represents an access subtype code (Do Not Use) in the Automation batch processing system.
    /// </summary>
    [Table("AccessSubTypeCode_DNU")]
    public class AccessSubTypeCode_DNU
    {
        #region Properties

        /// <summary>
        /// Gets or sets the unique identifier for the access subtype code.
        /// </summary>
        [Key]
        public short AccessSubTypeCodePK { get; set; }

        /// <summary>
        /// Gets or sets the description of the access subtype.
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string? AccessSubTypeDesc { get; set; }

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