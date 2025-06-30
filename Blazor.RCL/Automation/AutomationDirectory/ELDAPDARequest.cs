
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blazor.RCL.Automation.AutomationDirectory
{
    /// <summary>
    /// Represents an ELDAP DA (Directory Access) request record.
    /// </summary>
    [Table("ELDAPDARequest")]
    public class ELDAPDARequest
    {
        /// <summary>
        /// Gets or sets the primary key for the ELDAP DA request.
        /// </summary>
        public int ELDAPDARequestPK { get; set; }

        /// <summary>
        /// Gets or sets the employee identifier.
        /// </summary>
        public string? EmployeeID { get; set; }

        /// <summary>
        /// Gets or sets the action to be performed.
        /// </summary>
        public string? Action { get; set; }

        /// <summary>
        /// Gets or sets the source identifier.
        /// </summary>
        public string SourceId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the Automation GUID.
        /// </summary>
        public Guid? AutomationGuid { get; set; }

        /// <summary>
        /// Gets or sets the status of the request.
        /// </summary>
        public string? Status { get; set; }

        /// <summary>
        /// Gets or sets the detailed status message.
        /// </summary>
        public string? StatusMessage { get; set; }

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