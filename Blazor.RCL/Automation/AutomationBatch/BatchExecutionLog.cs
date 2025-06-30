using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blazor.RCL.Domain.Entities.AutomationBatch
{
    /// <summary>
    /// Represents a batch execution log in the Automation batch processing system.
    /// </summary>
    [Table("BatchExecutionLog")]
    public class BatchExecutionLog
    {
        #region Properties

        /// <summary>
        /// Gets or sets the unique identifier for the batch execution log.
        /// </summary>
        [Key]
        public int BatchExecutionLogPK { get; set; }

        /// <summary>
        /// Gets or sets the batch request type code foreign key.
        /// </summary>
        [Required]
        public short BatchRequestTypeCodeFK { get; set; }

        /// <summary>
        /// Gets or sets the batch file path.
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string? BatchFilePath { get; set; }

        /// <summary>
        /// Gets or sets the batch file name.
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string? BatchFileName { get; set; }

        /// <summary>
        /// Gets or sets the source identifier.
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string? SourceId { get; set; }

        /// <summary>
        /// Gets or sets the execution status code foreign key.
        /// </summary>
        public short? ExecutionStatusCodeFK { get; set; }

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
