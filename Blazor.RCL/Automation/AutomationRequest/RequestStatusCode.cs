using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blazor.RCL.Automation.AutomationRequest
{
    /// <summary>
    /// Represents status codes for requests.
    /// </summary>
    [Table("RequestStatusCode")]
    public class RequestStatusCode
    {
        /// <summary>
        /// Gets or sets the primary key for the request status code.
        /// </summary>
        [Key]
        public short RequestStatusCodePK { get; set; }

        /// <summary>
        /// Gets or sets the description of the request status.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string RequestStatusDesc { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the display text for the status.
        /// </summary>
        public string? DisplayStatusText { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the status is active.
        /// </summary>
        public bool ActiveInd { get; set; }

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