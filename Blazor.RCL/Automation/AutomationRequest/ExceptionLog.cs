using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blazor.RCL.Automation.AutomationRequest
{
    /// <summary>
    /// Represents a log entry for exceptions.
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
        /// Gets or sets the error number associated with the exception.
        /// </summary>
        [Required]
        public int ErrorNumber { get; set; }

        /// <summary>
        /// Gets or sets the state of the error.
        /// </summary>
        [Required]
        public int ErrorState { get; set; }

        /// <summary>
        /// Gets or sets the severity of the error.
        /// </summary>
        [Required]
        public int ErrorSeverity { get; set; }

        /// <summary>
        /// Gets or sets the procedure where the error occurred.
        /// </summary>
        [Required]
        public string ErrorProcedure { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the line number where the error occurred.
        /// </summary>
        [Required]
        public int ErrorLine { get; set; }

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        [Required]
        public string ErrorMessage { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the foreign key for the request log.
        /// </summary>
        public int? 
            
            FK { get; set; }

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