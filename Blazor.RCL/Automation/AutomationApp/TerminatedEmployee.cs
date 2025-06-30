using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blazor.RCL.Automation.AutomationApp
{
    /// <summary>
    /// Represents a terminated employee.
    /// </summary>
    [Table("TerminatedEmployee")]
    public class TerminatedEmployee
    {
        /// <summary>
        /// Gets or sets the primary key for the terminated employee.
        /// </summary>
        [Key]
        public int TerminatedEmployeePK { get; set; }

        /// <summary>
        /// Gets or sets the employee ID.
        /// </summary>
        public string? EmployeeId { get; set; }

        /// <summary>
        /// Gets or sets the first name of the terminated employee.
        /// </summary>
        public string? FirstName { get; set; }

        /// <summary>
        /// Gets or sets the last name of the terminated employee.
        /// </summary>
        public string? LastName { get; set; }

        /// <summary>
        /// Gets or sets the level 1 manager ID.
        /// </summary>
        public string? L1ManagerId { get; set; }

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