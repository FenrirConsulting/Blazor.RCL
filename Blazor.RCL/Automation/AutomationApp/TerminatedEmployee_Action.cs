using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blazor.RCL.Automation.AutomationApp
{
    /// <summary>
    /// Represents an action taken for a terminated employee.
    /// </summary>
    [Table("TerminatedEmployee_Action")]
    public class TerminatedEmployee_Action
    {
        /// <summary>
        /// Gets or sets the primary key for the terminated employee action.
        /// </summary>
        [Key]
        public int TerminatedEmployee_ActionPK { get; set; }

        /// <summary>
        /// Gets or sets the foreign key for the terminated employee.
        /// </summary>
        public int TerminatedEmployeeFK { get; set; }

        /// <summary>
        /// Gets or sets the foreign key for the required action code.
        /// </summary>
        public short RequiredActionCodeFK { get; set; }

        /// <summary>
        /// Gets or sets the foreign key for the required action environment.
        /// </summary>
        public short? RequiredActionEnvironmentFK { get; set; }

        /// <summary>
        /// Gets or sets the foreign key for the batch request.
        /// </summary>
        public int? BatchRequestFK { get; set; }

        /// <summary>
        /// Gets or sets the date when the termination process occurred.
        /// </summary>
        [Required]
        public DateTime TermProcessDate { get; set; }

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