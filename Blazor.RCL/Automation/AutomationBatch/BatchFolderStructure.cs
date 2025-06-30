using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blazor.RCL.Domain.Entities.AutomationBatch
{
    /// <summary>
    /// Represents a batch folder structure in the Automation batch processing system.
    /// </summary>
    [Table("BatchFolderStructure")]
    public class BatchFolderStructure
    {
        #region Properties

        /// <summary>
        /// Gets or sets the unique identifier for the batch folder structure.
        /// </summary>
        [Key]
        public short BatchFolderStructurePK { get; set; }

        /// <summary>
        /// Gets or sets the folder name.
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string? FolderName { get; set; }

        /// <summary>
        /// Gets or sets the child folder name.
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string? ChildFolderName { get; set; }

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