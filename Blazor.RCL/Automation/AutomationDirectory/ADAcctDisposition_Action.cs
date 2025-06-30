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
    /// Represents an action entry for AD Account Disposition.
    /// </summary>
    [Table("ADAcctDisposition_Action")]
    public class ADAcctDisposition_Action
    {
        /// <summary>
        /// Gets or sets the action identifier.
        /// </summary>
        [Key]
        public int ActionID { get; set; }

        /// <summary>
        /// Gets or sets the action.
        /// </summary>
        [MaxLength(20)]
        public string? Action { get; set; }

        /// <summary>
        /// Gets or sets the final action.
        /// </summary>
        [MaxLength(20)]
        public string? FinalAction { get; set; }

        /// <summary>
        /// Gets or sets the action type.
        /// </summary>
        [MaxLength(50)]
        public string? ActionType { get; set; }

        /// <summary>
        /// Gets or sets the action value.
        /// </summary>
        [MaxLength(20)]
        public string? ActionValue { get; set; }

        /// <summary>
        /// Gets or sets the action comment.
        /// </summary>
        [MaxLength(200)]
        public string? ActionComment { get; set; }

        /// <summary>
        /// Gets or sets the action result.
        /// </summary>
        [MaxLength(20)]
        public string? ActionResult { get; set; }

        /// <summary>
        /// Gets or sets the action result comment.
        /// </summary>
        [MaxLength(200)]
        public string? ActionResultComment { get; set; }

        /// <summary>
        /// Gets or sets the user's last logon.
        /// </summary>
        [MaxLength(30)]
        public string? UserLastLogon { get; set; }

        /// <summary>
        /// Gets or sets the date of disable.
        /// </summary>
        [MaxLength(20)]
        public string? DateOfDisable { get; set; }

        /// <summary>
        /// Gets or sets the source domain.
        /// </summary>
        [MaxLength(200)]
        public string? SourceDomain { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the account is disabled.
        /// </summary>
        [Column("Disabled")]
        public bool Disabled { get; set; }

        /// <summary>
        /// Gets or sets the source SAM account name.
        /// </summary>
        [MaxLength(200)]
        public string? SourceSamaccountname { get; set; }

        /// <summary>
        /// Gets or sets the source original OU.
        /// </summary>
        [MaxLength(200)]
        public string? SourceOriginalOU { get; set; }

        /// <summary>
        /// Gets or sets the Extension Attribute 8.
        /// </summary>
        [MaxLength(200)]
        public string? ExtensionAttribute8 { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        [MaxLength(1000)]
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the source manager.
        /// </summary>
        [MaxLength(200)]
        public string? SourceManager { get; set; }

        /// <summary>
        /// Gets or sets the source member of.
        /// </summary>
        [MaxLength(200)]
        public string? SourceMemberOf { get; set; }

        /// <summary>
        /// Gets or sets the employee number.
        /// </summary>
        [MaxLength(50)]
        public string? EmployeeNumber { get; set; }

        /// <summary>
        /// Gets or sets the mDBUseDefaults value.
        /// </summary>
        [MaxLength(200)]
        public string? mDBUseDefaults { get; set; }

        /// <summary>
        /// Gets or sets the msExchHideFromAddressLists value.
        /// </summary>
        [MaxLength(1000)]
        public string? msExchHideFromAddressLists { get; set; }

        /// <summary>
        /// Gets or sets the last logon date.
        /// </summary>
        [MaxLength(30)]
        public string? LastLogonDate { get; set; }

        /// <summary>
        /// Gets or sets the creation date.
        /// </summary>
        [MaxLength(30)]
        public string? WhenCreated { get; set; }

        /// <summary>
        /// Gets or sets the password last set date.
        /// </summary>
        [MaxLength(200)]
        public string? PwdLastSet { get; set; }

        /// <summary>
        /// Gets or sets the batch ID.
        /// </summary>
        [MaxLength(100)]
        public string? BatchId { get; set; }

        /// <summary>
        /// Gets or sets the batch request FK.
        /// </summary>
        public int? BatchRequestFK { get; set; }

        /// <summary>
        /// Gets or sets the tools result.
        /// </summary>
        [MaxLength(20)]
        public string? ToolsResult { get; set; }
    }
}
