using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blazor.RCL.Domain.Entities.AutomationBatch
{
    /// <summary>
    /// Represents a detail data type code in the Automation batch processing system.
    /// </summary>
    [Table("DetailDataTypeCode")]
    public class DetailDataTypeCode
    {
        #region Properties

        /// <summary>
        /// Gets or sets the unique identifier for the detail data type code.
        /// </summary>
        [Key]
        public short DetailDataTypeCodePK { get; set; }

        /// <summary>
        /// Gets or sets the detail data type description.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string? DetailDataTypeDesc { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the data type code is active.
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
