using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blazor.RCL.Domain.Entities.AutomationBatch
{
    /// <summary>
    /// Represents a batch request detail in the Automation batch processing system.
    /// </summary>
    [Table("BatchRequestDetail")]
    public class BatchRequestDetail
    {
        #region Properties

        /// <summary>
        /// Gets or sets the unique identifier for the batch request detail.
        /// </summary>
        [Key]
        public int BatchRequestDetailPK { get; set; }

        /// <summary>
        /// Gets or sets the batch request foreign key.
        /// </summary>
        [Required]
        public int BatchRequesFK { get; set; }

        /// <summary>
        /// Gets or sets the detail data type code foreign key.
        /// </summary>
        [Required]
        public short DetailDataTypeCodeFK { get; set; }

        /// <summary>
        /// Gets or sets the key data field name.
        /// </summary>
        [MaxLength(100)]
        public string? KeyDataFieldName { get; set; }

        /// <summary>
        /// Gets or sets the data value.
        /// </summary>
        [Required]
        public string? DataValue { get; set; }

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