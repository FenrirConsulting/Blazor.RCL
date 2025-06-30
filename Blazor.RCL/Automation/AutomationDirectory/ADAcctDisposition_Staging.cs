using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Blazor.RCL.Automation.AutomationDirectory
{
    /// <summary>
    /// Represents a staging entry for AD Account Disposition.
    /// </summary>
    [Table("ADAcctDisposition_Staging")]
    public class ADAcctDisposition_Staging
    {
        /// <summary>
        /// Gets or sets the unique identifier for the staging entry.
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the batch identifier.
        /// </summary>
        [MaxLength(100)]
        public string? BatchId { get; set; }

        /// <summary>
        /// Gets or sets the domain.
        /// </summary>
        [MaxLength(200)]
        public string? Domain { get; set; }

        /// <summary>
        /// Gets or sets the path.
        /// </summary>
        [MaxLength(500)]
        public string? Path { get; set; }

        /// <summary>
        /// Gets or sets the distinguished name.
        /// </summary>
        [MaxLength(500)]
        public string? DistinguishedName { get; set; }

        /// <summary>
        /// Gets or sets the canonical name.
        /// </summary>
        [MaxLength(200)]
        public string? CanonicalName { get; set; }

        /// <summary>
        /// Gets or sets the CN (Common Name).
        /// </summary>
        [MaxLength(200)]
        public string? CN { get; set; }

        /// <summary>
        /// Gets or sets the SAM account name.
        /// </summary>
        [MaxLength(200)]
        public string? SamAccountName { get; set; }

        /// <summary>
        /// Gets or sets the employee number.
        /// </summary>
        [MaxLength(200)]
        public string? EmployeeNumber { get; set; }

        /// <summary>
        /// Gets or sets the HCB employee number.
        /// </summary>
        [MaxLength(200)]
        public string? HcbEmployeeNumber { get; set; }

        /// <summary>
        /// Gets or sets the employee ID.
        /// </summary>
        [MaxLength(200)]
        public string? EmployeeId { get; set; }

        /// <summary>
        /// Gets or sets the Company employee ID.
        /// </summary>
        [MaxLength(200)]
        public string? CompanyEmployeeId { get; set; }

        /// <summary>
        /// Gets or sets the email address.
        /// </summary>
        [MaxLength(200)]
        public string? Mail { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        [MaxLength(500)]
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the manager.
        /// </summary>
        [MaxLength(200)]
        public string? Manager { get; set; }

        /// <summary>
        /// Gets or sets the last logon timestamp.
        /// </summary>
        [MaxLength(200)]
        public string? LastLogonTimeStamp { get; set; }

        /// <summary>
        /// Gets or sets the last logon date.
        /// </summary>
        [MaxLength(200)]
        public string? LastLogonDate { get; set; }

        /// <summary>
        /// Gets or sets the number of days since last logon.
        /// </summary>
        public int? LastLogonDays { get; set; }

        /// <summary>
        /// Gets or sets the last sign-in from Azure.
        /// </summary>
        [MaxLength(200)]
        public string? LastSignInFromAzure { get; set; }

        /// <summary>
        /// Gets or sets the creation date.
        /// </summary>
        [MaxLength(200)]
        public string? WhenCreated { get; set; }

        /// <summary>
        /// Gets or sets the last change date.
        /// </summary>
        [MaxLength(200)]
        public string? WhenChanged { get; set; }

        /// <summary>
        /// Gets or sets the number of days since creation.
        /// </summary>
        public int? WhenCreatedDays { get; set; }

        /// <summary>
        /// Gets or sets the last password set date.
        /// </summary>
        [MaxLength(200)]
        public string? PwdLastSet { get; set; }

        /// <summary>
        /// Gets or sets the last password set date as a formatted string.
        /// </summary>
        [MaxLength(200)]
        public string? PwdLastSetDate { get; set; }

        /// <summary>
        /// Gets or sets the number of days since the last password set.
        /// </summary>
        public int? PwdLastSetDays { get; set; }

        /// <summary>
        /// Gets or sets the Extension Attribute 8.
        /// </summary>
        [MaxLength(200)]
        public string? ExtensionAttribute8 { get; set; }

        /// <summary>
        /// Gets or sets the MS-DS-Source-Object-DN.
        /// </summary>
        [MaxLength(200)]
        public string? MsdsSourceObjectDn { get; set; }

        /// <summary>
        /// Gets or sets the MS-Exchange-Hide-From-Address-Lists attribute.
        /// </summary>
        [MaxLength(200)]
        public string? MsExchHideFromAddressLists { get; set; }

        /// <summary>
        /// Gets or sets the MDB-Use-Defaults attribute.
        /// </summary>
        [MaxLength(200)]
        public string? MdbuseDefaults { get; set; }

        /// <summary>
        /// Gets or sets the MS-DS-Consistency-Guid.
        /// </summary>
        [MaxLength(200)]
        public string? MsDSConsistencyGuid { get; set; }

        /// <summary>
        /// Gets or sets the Member Of attribute.
        /// </summary>
        [MaxLength(200)]
        public string? MemberOf { get; set; }

        /// <summary>
        /// Gets or sets the User Account Control attribute.
        /// </summary>
        [MaxLength(200)]
        public string? UserAccountControl { get; set; }

        /// <summary>
        /// Gets or sets the Never Logon attribute.
        /// </summary>
        [MaxLength(200)]
        public string? NeverLogon { get; set; }

        /// <summary>
        /// Gets or sets the UAC Disabled attribute.
        /// </summary>
        [MaxLength(200)]
        public string? UACDisabled { get; set; }

        /// <summary>
        /// Gets or sets the Account Inactive attribute.
        /// </summary>
        [MaxLength(200)]
        public string? AcctInactive { get; set; }

        /// <summary>
        /// Gets or sets the Account Reinstated attribute.
        /// </summary>
        [MaxLength(200)]
        public string? AcctReinstated { get; set; }

        /// <summary>
        /// Gets or sets the Account Reinstated Date.
        /// </summary>
        [MaxLength(200)]
        public string? AcctReinstatedDate { get; set; }

        /// <summary>
        /// Gets or sets the number of days since the account was reinstated.
        /// </summary>
        public int? AcctReinstatedDays { get; set; }

        /// <summary>
        /// Gets or sets the facsimile telephone number.
        /// </summary>
        [MaxLength(100)]
        public string? facsimileTelephoneNumber { get; set; }

        /// <summary>
        /// Gets or sets the LOA (Leave of Absence) attribute.
        /// </summary>
        [MaxLength(200)]
        public string? LOA { get; set; }

        /// <summary>
        /// Gets or sets the AETH attribute.
        /// </summary>
        [MaxLength(200)]
        public string? AETH { get; set; }

        /// <summary>
        /// Gets or sets the Store Employee attribute.
        /// </summary>
        [MaxLength(200)]
        public string? StoreEmployee { get; set; }

        /// <summary>
        /// Gets or sets the new description.
        /// </summary>
        [MaxLength(1000)]
        public string? NewDesc { get; set; }

        /// <summary>
        /// Gets or sets the action.
        /// </summary>
        [MaxLength(200)]
        public string? Action { get; set; }

        /// <summary>
        /// Gets or sets the action type.
        /// </summary>
        [MaxLength(200)]
        public string? ActionType { get; set; }

        /// <summary>
        /// Gets or sets the action value.
        /// </summary>
        [MaxLength(200)]
        public string? ActionValue { get; set; }

        /// <summary>
        /// Gets or sets the action comment.
        /// </summary>
        [MaxLength(200)]
        public string? ActionComment { get; set; }

        /// <summary>
        /// Gets or sets the action result.
        /// </summary>
        [MaxLength(200)]
        public string? ActionResult { get; set; }

        /// <summary>
        /// Gets or sets the action result comment.
        /// </summary>
        [MaxLength(200)]
        public string? ActionResultComment { get; set; }

        /// <summary>
        /// Gets or sets the audit update user name.
        /// </summary>
        [MaxLength(200)]
        public string? AuditUpdateUserName { get; set; }

        /// <summary>
        /// Gets or sets the audit update date.
        /// </summary>
        public DateTime? AuditUpdateDate { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        [MaxLength(30)]
        public string? Status { get; set; }

        /// <summary>
        /// Gets or sets the final status.
        /// </summary>
        [MaxLength(20)]
        public string? FinalStatus { get; set; }

        /// <summary>
        /// Gets or sets the status reason.
        /// </summary>
        [MaxLength(50)]
        public string? StatusReason { get; set; }
    }

}
