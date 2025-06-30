using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazor.RCL.Automation.AutomationDirectory
{
    /// <summary>
    /// Represents an AD request entry.
    /// </summary>
    [Table("ADADRequest")]
    public class ADADRequest
    {
        /// <summary>
        /// Gets or sets the request primary key identifier.
        /// </summary>
        [Key]
        public long ADADRequestPK { get; set; }

        /// <summary>
        /// Gets or sets the domain.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string Domain { get; set; } = null!;

        /// <summary>
        /// Gets or sets the Company resource ID.
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string CompanyResourceId { get; set; } = null!;

        /// <summary>
        /// Gets or sets the HR status.
        /// </summary>
        [MaxLength(50)]
        public string? HRStatus { get; set; }

        /// <summary>
        /// Gets or sets the SAM account name.
        /// </summary>
        [Required]
        [MaxLength(150)]
        public string SamAccountName { get; set; } = null!;

        /// <summary>
        /// Gets or sets the UPN (User Principal Name).
        /// </summary>
        [Required]
        [MaxLength(150)]
        public string UPN { get; set; } = null!;

        /// <summary>
        /// Gets or sets the user account control.
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string UserAccountControl { get; set; } = null!;

        /// <summary>
        /// Gets or sets the distinguished name.
        /// </summary>
        [Required]
        [MaxLength(1024)]
        public string DistinguishedName { get; set; } = null!;

        /// <summary>
        /// Gets or sets the SN (surname).
        /// </summary>
        [Required]
        [MaxLength(150)]
        public string SN { get; set; } = null!;

        /// <summary>
        /// Gets or sets the given name.
        /// </summary>
        [Required]
        [MaxLength(150)]
        public string GivenName { get; set; } = null!;

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        [MaxLength(200)]
        public string? DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the AD description.
        /// </summary>
        [MaxLength(1024)]
        public string? ADDescription { get; set; }

        /// <summary>
        /// Gets or sets the EA3 (Extension Attribute 3).
        /// </summary>
        [MaxLength(2048)]
        public string? EA3 { get; set; }

        /// <summary>
        /// Gets or sets the EA4 (Extension Attribute 4).
        /// </summary>
        [MaxLength(500)]
        public string? EA4 { get; set; }

        /// <summary>
        /// Gets or sets the EA8 (Extension Attribute 8).
        /// </summary>
        [MaxLength(50)]
        public string? EA8 { get; set; }

        /// <summary>
        /// Gets or sets the member of.
        /// </summary>
        [MaxLength(4096)]
        public string? MemberOf { get; set; }

        /// <summary>
        /// Gets or sets the mail.
        /// </summary>
        [MaxLength(320)]
        public string? Mail { get; set; }

        /// <summary>
        /// Gets or sets the manager.
        /// </summary>
        [MaxLength(320)]
        public string? Manager { get; set; }

        /// <summary>
        /// Gets or sets the manager mail.
        /// </summary>
        [MaxLength(320)]
        public string? ManagerMail { get; set; }

        /// <summary>
        /// Gets or sets the mDBUseDefaults value.
        /// </summary>
        [MaxLength(100)]
        public string? MdbuseDefaults { get; set; }

        /// <summary>
        /// Gets or sets the MS Exchange Remote Recipient Type.
        /// </summary>
        [MaxLength(50)]
        public string? MSExchangeRemoteRecipientType { get; set; }

        /// <summary>
        /// Gets or sets the MS Exchange Recipient Type Details.
        /// </summary>
        [MaxLength(50)]
        public string? MSExchRecipientTypeDetails { get; set; }

        /// <summary>
        /// Gets or sets the MS Exchange Hide From Address Lists.
        /// </summary>
        [MaxLength(200)]
        public string? MsExchHideFromAddressLists { get; set; }

        /// <summary>
        /// Gets or sets the facsimile telephone number.
        /// </summary>
        [MaxLength(200)]
        public string? FacsimileTelephoneNumber { get; set; }

        /// <summary>
        /// Gets or sets the user enabled status.
        /// </summary>
        [MaxLength(50)]
        public string? UserEnabled { get; set; }

        /// <summary>
        /// Gets or sets the password last set date.
        /// </summary>
        public DateTime? PwdLastSetDate { get; set; }

        /// <summary>
        /// Gets or sets the password last set days.
        /// </summary>
        public int? PwdLastSetDays { get; set; }

        /// <summary>
        /// Gets or sets the last logon date.
        /// </summary>
        public DateTime? LastLogonDate { get; set; }

        /// <summary>
        /// Gets or sets the last logon days.
        /// </summary>
        public int? LastLogonDays { get; set; }

        /// <summary>
        /// Gets or sets the AD when created date.
        /// </summary>
        public DateTime? ADWhenCreated { get; set; }

        /// <summary>
        /// Gets or sets the action.
        /// </summary>
        [MaxLength(20)]
        public string? Action { get; set; }

        /// <summary>
        /// Gets or sets the action comment.
        /// </summary>
        [MaxLength(200)]
        public string? ActionComment { get; set; }

        /// <summary>
        /// Gets or sets the action date.
        /// </summary>
        public DateTime? ActionDate { get; set; }

        /// <summary>
        /// Gets or sets the audit update user name.
        /// </summary>
        [MaxLength(100)]
        public string? AuditUpdateUserName { get; set; }

        /// <summary>
        /// Gets or sets the audit update date.
        /// </summary>
        public DateTime? AuditUpdateDate { get; set; }

        /// <summary>
        /// Gets or sets the tools result.
        /// </summary>
        [MaxLength(100)]
        public string? ToolsResult { get; set; }

        /// <summary>
        /// Gets or sets the source ID.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string SourceId { get; set; } = null!;

        /// <summary>
        /// Gets or sets the Automation GUID.
        /// </summary>
        [MaxLength(100)]
        public string? AutomationGuid { get; set; }

        /// <summary>
        /// Gets or sets the Automation status.
        /// </summary>
        [MaxLength(50)]
        public string? AutomationStatus { get; set; }

        /// <summary>
        /// Gets or sets the Automation status message.
        /// </summary>
        [Column(TypeName = "varchar(MAX)")]
        public string? AutomationStatusMessage { get; set; }
    }
}
