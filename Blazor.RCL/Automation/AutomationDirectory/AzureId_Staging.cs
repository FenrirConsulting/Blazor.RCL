using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blazor.RCL.Automation.AutomationDirectory
{
    /// <summary>
    /// Represents an Azure ID staging record.
    /// </summary>
    [Table("AzureId_Staging")]
    public class AzureId_Staging
    {
        /// <summary>
        /// Gets or sets the Azure object identifier.
        /// </summary>
        public string? Id { get; set; }

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        public string? DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the email address.
        /// </summary>
        public string? Mail { get; set; }

        /// <summary>
        /// Gets or sets the user principal name.
        /// </summary>
        public string? UserPrincipalName { get; set; }

        /// <summary>
        /// Gets or sets the employee identifier.
        /// </summary>
        public string? EmployeeId { get; set; }

        /// <summary>
        /// Gets or sets the creation date and time.
        /// </summary>
        public DateTime? CreatedDateTime { get; set; }

        /// <summary>
        /// Gets or sets the last sign-in date and time.
        /// </summary>
        public DateTime? LastSignInDateTime { get; set; }

        /// <summary>
        /// Gets or sets the username of the user who last updated the record.
        /// </summary>
        public string? AuditUpdateUserName { get; set; }

        /// <summary>
        /// Gets or sets the date and time of the last update.
        /// </summary>
        public DateTime? AuditUpdateDate { get; set; }
    }
}