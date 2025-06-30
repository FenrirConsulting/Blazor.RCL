using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blazor.RCL.Automation.AutomationRequest
{
    /// <summary>
    /// Represents the request counts outside of Automation.
    /// </summary>
    [Table("RequestCountsOutsideAutomation")]
    public class RequestCountsOutsideAutomation
    {
        /// <summary>
        /// Gets or sets the primary key for the request counts outside Automation.
        /// </summary>
        [Key]
        public short RequestCountsOutsideAutomationPK { get; set; }

        /// <summary>
        /// Gets or sets the foreign key for valid access type code.
        /// </summary>
        [Required]
        public short ValidAccessTypeCodeFK { get; set; }

        /// <summary>
        /// Gets or sets the month of the request.
        /// </summary>
        [Required]
        public short Month { get; set; }

        /// <summary>
        /// Gets or sets the year of the request.
        /// </summary>
        [Required]
        public short Year { get; set; }

        /// <summary>
        /// Gets or sets the foreign key for request status code.
        /// </summary>
        [Required]
        public short RequestStatusCodeFK { get; set; }

        /// <summary>
        /// Gets or sets the count of requests.
        /// </summary>
        [Required]
        public int Count { get; set; }

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