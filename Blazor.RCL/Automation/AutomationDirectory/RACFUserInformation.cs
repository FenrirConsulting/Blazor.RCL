using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blazor.RCL.Automation.AutomationDirectory
{
    /// <summary>
    /// Represents RACF user information record.
    /// </summary>
    [Table("RACFUserInformation")]
    public class RACFUserInformation
    {
        /// <summary>
        /// Gets or sets the date when the record was entered.
        /// </summary>
        public DateTime DateEntered { get; set; }

        /// <summary>
        /// Gets or sets the user class.
        /// </summary>
        public string? Class { get; set; }

        /// <summary>
        /// Gets or sets the count information.
        /// </summary>
        public string? Count { get; set; }

        /// <summary>
        /// Gets or sets the last use information.
        /// </summary>
        public string? LastUse { get; set; }

        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        public string? UserId { get; set; }

        /// <summary>
        /// Gets or sets the user's name.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the default group.
        /// </summary>
        public string? DfltGrp { get; set; }

        /// <summary>
        /// Gets or sets the owner information.
        /// </summary>
        public string? Owner { get; set; }

        /// <summary>
        /// Gets or sets the creation date.
        /// </summary>
        public string? CreateDat { get; set; }

        /// <summary>
        /// Gets or sets the employee identifier.
        /// </summary>
        public string? EmpID { get; set; }

        /// <summary>
        /// Gets or sets the installation data.
        /// </summary>
        public string? InstData { get; set; }

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
