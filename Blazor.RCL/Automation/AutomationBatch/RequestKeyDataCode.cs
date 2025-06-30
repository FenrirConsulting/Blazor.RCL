using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blazor.RCL.Domain.Entities.AutomationBatch
{
    /// <summary>
    /// Represents a request key data code in the Automation batch processing system.
    /// </summary>
    [Table("RequestKeyDataCode")]
    public class RequestKeyDataCode
    {
        #region Properties

        /// <summary>
        /// Gets or sets the unique identifier for the request key data code.
        /// </summary>
        [Key]
        public short RequestKeyDataCodePK { get; set; }

        /// <summary>
        /// Gets or sets the batch request type code foreign key.
        /// </summary>
        [Required]
        public short BatchRequestTypeCodeFK { get; set; }

        /// <summary>
        /// Gets or sets the request key data description.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string? RequestKeyDataDesc { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the key data code is active.
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