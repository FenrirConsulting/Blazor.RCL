using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blazor.RCL.Automation.AutomationMail
{
    /// <summary>
    /// Represents an entry in the exception log.
    /// </summary>
    [Table("ExceptionLog")]
    public class ExceptionLog
    {
        /// <summary>
        /// Gets or sets the primary key for the exception log.
        /// </summary>
        [Key]
        public int ExceptionLogPK { get; set; }

        /// <summary>
        /// Gets or sets the error number.
        /// </summary>
        public int ErrorNumber { get; set; }

        /// <summary>
        /// Gets or sets the error state.
        /// </summary>
        public int ErrorState { get; set; }

        /// <summary>
        /// Gets or sets the error severity.
        /// </summary>
        public int ErrorSeverity { get; set; }

        /// <summary>
        /// Gets or sets the error procedure.
        /// </summary>
        [Required]
        public string ErrorProcedure { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the error line.
        /// </summary>
        public int ErrorLine { get; set; }

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        [Required]
        public string ErrorMessage { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the foreign key for the batch execution log.
        /// </summary>
        public int? BatchExecutionLogFK { get; set; }

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