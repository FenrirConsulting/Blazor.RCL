using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blazor.RCL.Automation.AutomationMail
{
    /// <summary>
    /// Represents the details of an exchange request.
    /// </summary>
    [Table("ExchangeRequestDetail_DNU")]
    public class ExchangeRequestDetail_DNU
    {
        /// <summary>
        /// Gets or sets the primary key for the exchange request detail.
        /// </summary>
        [Key]
        public int ExchangeRequestDetailPK { get; set; }

        /// <summary>
        /// Gets or sets the foreign key for the batch execution log.
        /// </summary>
        public int BatchExecutionLogFK { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the Automation.
        /// </summary>
        public Guid? AutomationGuid { get; set; }

        /// <summary>
        /// Gets or sets the user type.
        /// </summary>
        public string? UserType { get; set; }

        /// <summary>
        /// Gets or sets the custom SMTP suffix.
        /// </summary>
        public string? CustomSMTPSuffix { get; set; }

        /// <summary>
        /// Gets or sets the SAM account name.
        /// </summary>
        public string? SamAccountName { get; set; }

        /// <summary>
        /// Gets or sets the distinguished name.
        /// </summary>
        public string? DistinguishedName { get; set; }

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        public string? DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the surname (SN).
        /// </summary>
        public string? SN { get; set; }

        /// <summary>
        /// Gets or sets the given name.
        /// </summary>
        public string? GivenName { get; set; }

        /// <summary>
        /// Gets or sets the full name.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the initials.
        /// </summary>
        public string? Initials { get; set; }

        /// <summary>
        /// Gets or sets the employee number.
        /// </summary>
        public string? EmployeeNumber { get; set; }

        /// <summary>
        /// Gets or sets the company name.
        /// </summary>
        public string? Company { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// Gets or sets the date when the account was created in Active Directory.
        /// </summary>
        public string? ADWhenCreated { get; set; }

        /// <summary>
        /// Gets or sets the home MDB.
        /// </summary>
        public string? HomeMdb { get; set; }

        /// <summary>
        /// Gets or sets the extension attribute 8.
        /// </summary>
        public string? ExtensionAttribute8 { get; set; }

        /// <summary>
        /// Gets or sets the MSDSSourceObjectDN.
        /// </summary>
        public string? MSDSSourceObjectDN { get; set; }

        /// <summary>
        /// Gets or sets the MSExchRemoteRecipientType.
        /// </summary>
        public string? MSExchRemoteRecipientType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the user is enabled.
        /// </summary>
        public string? Enabled { get; set; }

        /// <summary>
        /// Gets or sets the user account control.
        /// </summary>
        public string? UserAccountControl { get; set; }

        /// <summary>
        /// Gets or sets the source distinguished name.
        /// </summary>
        public string? SourceDistinguishedname { get; set; }

        /// <summary>
        /// Gets or sets the source domain.
        /// </summary>
        public string? SourceDomain { get; set; }

        /// <summary>
        /// Gets or sets the source account details.
        /// </summary>
        public string? SourceAccountDetails { get; set; }

        /// <summary>
        /// Gets or sets the source SAM account name.
        /// </summary>
        public string? SourceSamAccountName { get; set; }

        /// <summary>
        /// Gets or sets the source account SID.
        /// </summary>
        public string? SourceAccountSid { get; set; }

        /// <summary>
        /// Gets or sets the default SMTP address.
        /// </summary>
        public string? DefaultSMTPAddress { get; set; }

        /// <summary>
        /// Gets or sets the custom SMTP address.
        /// </summary>
        public string? CustomSMTPAddress { get; set; }

        /// <summary>
        /// Gets or sets the remote routing address.
        /// </summary>
        public string? RemoteRoutingAddress { get; set; }

        /// <summary>
        /// Gets or sets the MSDSSConsistencyGuid.
        /// </summary>
        public string? MSDSConsistencyGuid { get; set; }

        /// <summary>
        /// Gets or sets the MSExchMasterAccountSid.
        /// </summary>
        public string? MSExchMasterAccountSid { get; set; }

        /// <summary>
        /// Gets or sets the remote mailbox.
        /// </summary>
        public string? RemoteMailbox { get; set; }

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