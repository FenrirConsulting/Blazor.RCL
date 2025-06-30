using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blazor.RCL.Domain.Entities.AutomationBatch
{
    /// <summary>
    /// Represents an exception log in the Automation batch processing system.
    /// </summary>
    [Table("ExceptionLog")]
    public class ExceptionLog
    {
        #region Properties

        /// <summary>
        /// Gets or sets the unique identifier for the exception log.
        /// </summary>
        [Key]
        public int ExceptionLogPK { get; set; }

        /// <summary>
        /// Gets or sets the error number.
        /// </summary>
        [Required]
        public int ErrorNumber { get; set; }

        /// <summary>
        /// Gets or sets the error state.
        /// </summary>
        [Required]
        public int ErrorState { get; set; }

        /// <summary>
        /// Gets or sets the error severity.
        /// </summary>
        [Required]
        public int ErrorSeverity { get; set; }

        /// <summary>
        /// Gets or sets the error procedure.
        /// </summary>
        [Required]
        public string? ErrorProcedure { get; set; }

        /// <summary>
        /// Gets or sets the error line.
        /// </summary>
        [Required]
        public int ErrorLine { get; set; }

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        [Required]
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets the batch request foreign key.
        /// </summary>
        public int? BatchRequestFK { get; set; }

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
