using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blazor.RCL.Automation.AutomationDirectory
{
    /// <summary>
    /// Represents an RRTELDAP action record.
    /// </summary>
    [Table("RRTELDAP_Action")]
    public class RRTELDAP_Action
    {
        /// <summary>
        /// Gets or sets the primary key for RRTELDAP actions.
        /// </summary>
        public int RRTELDAP_ActionsPK { get; set; }

        /// <summary>
        /// Gets or sets the run date and time.
        /// </summary>
        public DateTime RunDateTime { get; set; }

        /// <summary>
        /// Gets or sets the RRT status.
        /// </summary>
        public string RRTStatus { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the employee number.
        /// </summary>
        public string EmployeeNumber { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the action to be performed.
        /// </summary>
        public string Action { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the batch request foreign key.
        /// </summary>
        public int? BatchRequestFK { get; set; }

        /// <summary>
        /// Gets or sets whether test mode is enabled.
        /// </summary>
        public bool? TestMode { get; set; }

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

        /// <summary>
        /// Gets or sets the source identifier.
        /// </summary>
        public string SourceID { get; set; } = string.Empty;
    }
}