using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blazor.RCL.Automation.AutomationRequest
{
    /// <summary>
    /// Represents a log entry for request errors.
    /// </summary>
    [Table("RequestErrorLog")]
    public class RequestErrorLog
    {
        /// <summary>
        /// Gets or sets the primary key for the request error log.
        /// </summary>
        [Key]
        public int RequestErrorLogPK { get; set; }

        /// <summary>
        /// Gets or sets the foreign key for the request log.
        /// </summary>
        [Required]
        public int RequestLogFK { get; set; }

        /// <summary>
        /// Gets or sets the foreign key for the request task.
        /// </summary>
        public int? RequestTaskFK { get; set; }

        /// <summary>
        /// Gets or sets the foreign key for the error code.
        /// </summary>
        [Required]
        public int ErrorCodeFK { get; set; }

        /// <summary>
        /// Gets or sets the description of the error.
        /// </summary>
        [Required]
        public string ErrorDesc { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the error trace.
        /// </summary>
        [Required]
        public string ErrorTrace { get; set; } = string.Empty;

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