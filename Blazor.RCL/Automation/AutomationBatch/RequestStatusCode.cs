using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blazor.RCL.Automation.AutomationBatch
{
    /// <summary>
    /// Represents a request status code in the Automation batch processing system.
    /// </summary>
    [Table("RequestStatusCode")]
    public class RequestStatusCode
    {
        #region Properties

        /// <summary>
        /// Gets or sets the unique identifier for the request status code.
        /// </summary>
        [Key]
        public short RequestStatusCodePK { get; set; }

        /// <summary>
        /// Gets or sets the description of the request status.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string? RequestStatusDesc { get; set; }

        /// <summary>
        /// Gets or sets the display text for the status.
        /// </summary>
        public string? DisplayStatusText { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the status code is active.
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
