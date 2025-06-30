using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blazor.RCL.Automation.AutomationDirectory
{
    /// <summary>
    /// Represents a revalidated action record for AD Account Disposition.
    /// </summary>
    [Table("ADAcctDisposition_RevalidatedActions")]
    public class ADAcctDisposition_RevalidatedActions
    {
        /// <summary>
        /// Gets or sets the employee number.
        /// </summary>
        public string? EmployeeNumber { get; set; }

        /// <summary>
        /// Gets or sets the employee identifier.
        /// </summary>
        public string? EmployeeId { get; set; }

        /// <summary>
        /// Gets or sets the Company employee identifier.
        /// </summary>
        public string? CompanyEmployeeId { get; set; }

        /// <summary>
        /// Gets or sets the final action to be performed.
        /// </summary>
        public string? FinalAction { get; set; }

        /// <summary>
        /// Gets or sets the batch identifier.
        /// </summary>
        public string? BatchId { get; set; }

        /// <summary>
        /// Gets or sets the date when the action was performed.
        /// </summary>
        public DateTime? ActionDate { get; set; }
    }
}