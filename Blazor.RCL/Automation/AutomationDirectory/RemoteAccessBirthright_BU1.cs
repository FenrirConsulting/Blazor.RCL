using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blazor.RCL.Automation.AutomationDirectory
{
    /// <summary>
    /// Represents remote access birthright backup record.
    /// </summary>
    [Table("RemoteAccessBirthright_BU1")]
    public class RemoteAccessBirthright_BU1
    {
        /// <summary>
        /// Gets or sets the primary key for remote access.
        /// </summary>
        public int RemoteAccessPK { get; set; }

        /// <summary>
        /// Gets or sets the run date and time.
        /// </summary>
        public string Rundatetime { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the source identifier.
        /// </summary>
        public string SourceId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the Automation GUID.
        /// </summary>
        public string? Automationguid { get; set; }

        /// <summary>
        /// Gets or sets the batch request status code primary key.
        /// </summary>
        public short BatchRequestStatusCodePK { get; set; }

        /// <summary>
        /// Gets or sets the status detail.
        /// </summary>
        public string? StatusDetail { get; set; }

        /// <summary>
        /// Gets or sets the distinguished name.
        /// </summary>
        public string DistinguishedName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the domain.
        /// </summary>
        public string Domain { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the employee number.
        /// </summary>
        public string Employeenumber { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the SAM account name.
        /// </summary>
        public string Samaccountname { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the business unit.
        /// </summary>
        public string? BusinessUnit { get; set; }

        /// <summary>
        /// Gets or sets the company code.
        /// </summary>
        public string? CompanyCode { get; set; }

        /// <summary>
        /// Gets or sets the reg temp value.
        /// </summary>
        public string? RegTemp { get; set; }

        /// <summary>
        /// Gets or sets the work location identifier.
        /// </summary>
        public string? WorkLocationId { get; set; }

        /// <summary>
        /// Gets or sets whether the account is enabled.
        /// </summary>
        public bool? IsEnabled { get; set; }

        /// <summary>
        /// Gets or sets whether the user is in group.
        /// </summary>
        public bool? IsUserInGroup { get; set; }

        /// <summary>
        /// Gets or sets whether to provision access.
        /// </summary>
        public bool? ProvisionAccess { get; set; }

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