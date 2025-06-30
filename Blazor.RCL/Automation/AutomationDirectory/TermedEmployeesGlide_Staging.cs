using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blazor.RCL.Automation.AutomationDirectory
{
    /// <summary>
    /// Represents a termed employees Glide staging record.
    /// </summary>
    [Table("TermedEmployeesGlide_Staging")]
    public class TermedEmployeesGlide_Staging
    {
        /// <summary>
        /// Gets or sets the date when the record was entered.
        /// </summary>
        public DateTime DateEntered { get; set; }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        public string? FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        public string? LastName { get; set; }

        /// <summary>
        /// Gets or sets the Aetna employee identifier.
        /// </summary>
        public string? AetnaEmployeeId { get; set; }

        /// <summary>
        /// Gets or sets the Company employee identifier.
        /// </summary>
        public string? CompanyEmployeeId { get; set; }

        /// <summary>
        /// Gets or sets the Company resource identifier.
        /// </summary>
        public string? CompanyResourceId { get; set; }

        /// <summary>
        /// Gets or sets the Aetna resource identifier.
        /// </summary>
        public string? AetnaResourceId { get; set; }

        /// <summary>
        /// Gets or sets the L1 manager identifier.
        /// </summary>
        public string? L1managerid { get; set; }

        /// <summary>
        /// Gets or sets the HR status.
        /// </summary>
        public string? HRStatus { get; set; }

        /// <summary>
        /// Gets or sets the term process date.
        /// </summary>
        public DateTime? termprocessdate { get; set; }

        /// <summary>
        /// Gets or sets the username of the user who added the record.
        /// </summary>
        public string AuditAddUserName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the date when the record was added.
        /// </summary>
        public DateTime AuditAddDate { get; set; }

        /// <summary>
        /// Gets or sets the username of the user who last changed the record.
        /// </summary>
        public string AuditChangeUserName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the date when the record was last changed.
        /// </summary>
        public DateTime AuditChangeDate { get; set; }
    }
}