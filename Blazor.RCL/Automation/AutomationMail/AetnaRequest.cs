using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blazor.RCL.Automation.AutomationMail
{
    /// <summary>
    /// Represents a request for Aetna.
    /// </summary>
    [Table("AetnaRequest")]
    public class AetnaRequest
    {
        /// <summary>
        /// Gets or sets the primary key for the Aetna request.
        /// </summary>
        [Key]
        public int AetnaRequestPK { get; set; }

        /// <summary>
        /// Gets or sets the foreign key for the batch execution log.
        /// </summary>
        public int BatchExecutionLogFK { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the Automation.
        /// </summary>
        public Guid? AutomationGuid { get; set; }

        /// <summary>
        /// Gets or sets the employee ID.
        /// </summary>
        public string? EmployeeID { get; set; }

        /// <summary>
        /// Gets or sets the SAM account name.
        /// </summary>
        public string? SamAccountName { get; set; }

        /// <summary>
        /// Gets or sets the email address.
        /// </summary>
        public string? Mail { get; set; }

        /// <summary>
        /// Gets or sets the item data.
        /// </summary>
        public string? ItemData { get; set; }

        /// <summary>
        /// Gets or sets the status of the request.
        /// </summary>
        public string? Status { get; set; }

        /// <summary>
        /// Gets or sets the status message.
        /// </summary>
        public string? StatusMessage { get; set; }

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