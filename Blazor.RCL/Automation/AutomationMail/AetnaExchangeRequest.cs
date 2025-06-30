using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blazor.RCL.Automation.AutomationMail
{
    /// <summary>
    /// Represents a request for Aetna Exchange.
    /// </summary>
    [Table("AetnaExchangeRequest")]
    public class AetnaExchangeRequest
    {
        /// <summary>
        /// Gets or sets the primary key for the Aetna exchange request.
        /// </summary>
        [Key]
        public int AetnaExchangeRequestPK { get; set; }

        /// <summary>
        /// Gets or sets the foreign key for the batch execution log.
        /// </summary>
        public int BatchExecutionLogFK { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the Automation.
        /// </summary>
        public Guid? AutomationGuid { get; set; }

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
        /// Gets or sets the employee ID.
        /// </summary>
        public string? EmployeeID { get; set; }

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
        /// Gets or sets the user account control.
        /// </summary>
        public string? UserAccountControl { get; set; }

        /// <summary>
        /// Gets or sets the default SMTP address.
        /// </summary>
        public string? DefaultSMTPAddress { get; set; }

        /// <summary>
        /// Gets or sets the custom SMTP address.
        /// </summary>
        public string? CustomSMTPAddress { get; set; }

        /// <summary>
        /// Gets or sets the Company Health SMTP address.
        /// </summary>
        public string? CompanySMTPAddress { get; set; }

        /// <summary>
        /// Gets or sets the remote routing address.
        /// </summary>
        public string? RemoteRoutingAddress { get; set; }

        /// <summary>
        /// Gets or sets the remote mailbox.
        /// </summary>
        public string? RemoteMailbox { get; set; }

        /// <summary>
        /// Gets or sets the admin description.
        /// </summary>
        public string? AdminDescription { get; set; }

        /// <summary>
        /// Gets or sets the status of the request.
        /// </summary>
        public string? Status { get; set; }

        /// <summary>
        /// Gets or sets the status message.
        /// </summary>
        public string? StatusMessage { get; set; }

        /// <summary>
        /// Gets or sets the validation status.
        /// </summary>
        public string? ValidationStatus { get; set; }

        /// <summary>
        /// Gets or sets the validation message.
        /// </summary>
        public string? ValidationMessage { get; set; }

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