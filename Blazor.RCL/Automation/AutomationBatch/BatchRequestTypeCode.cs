﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blazor.RCL.Domain.Entities.AutomationBatch
{
    /// <summary>
    /// Represents a batch request type code in the Automation batch processing system.
    /// </summary>
    [Table("BatchRequestTypeCode")]
    public class BatchRequestTypeCode
    {
        #region Properties

        /// <summary>
        /// Gets or sets the unique identifier for the batch request type code.
        /// </summary>
        [Key]
        public short BatchRequestTypeCodePK { get; set; }

        /// <summary>
        /// Gets or sets the batch type code.
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string? BatchTypeCode { get; set; }

        /// <summary>
        /// Gets or sets the batch type description.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string? BatchTypeDesc { get; set; }

        /// <summary>
        /// Gets or sets the valid access type code foreign key.
        /// </summary>
        [Required]
        public short ValidAccessTypeCodeFK { get; set; }

        /// <summary>
        /// Gets or sets the catalog item.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string? CatalogItem { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether snapshot is required.
        /// </summary>
        [Required]
        public bool RequiresSnapshotInd { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the type code is active.
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