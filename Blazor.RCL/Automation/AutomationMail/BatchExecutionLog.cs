using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blazor.RCL.Automation.AutomationMail
{
    /// <summary>
    /// Represents a log of batch executions.
    /// </summary>
    [Table("BatchExecutionLog")]
    public class BatchExecutionLog
    {
        /// <summary>
        /// Gets or sets the primary key for the batch execution log.
        /// </summary>
        [Key]
        public int BatchExecutionLogPK { get; set; }

        /// <summary>
        /// Gets or sets the foreign key for the batch type code.
        /// </summary>
        public short BatchTypeCodeFK { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the batch is in test mode.
        /// </summary>
        public bool TestModeInd { get; set; }

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
        [MaxLength(100)]
        public string? AuditChangeUserName { get; set; }

        /// <summary>
        /// Gets or sets the date when the record was last changed.
        /// </summary>
        public DateTime? AuditChangeDate { get; set; }
    }
}