using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazor.RCL.Automation.AutomationDirectory
{
    /// <summary>
    /// Represents a user entry for AD Account Disposition.
    /// </summary>
    [Table("ADAcctDisposition_Users")]
    public class ADAcctDisposition_Users
    {
        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        [Key]
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        [MaxLength(500)]
        public string? DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the email address.
        /// </summary>
        [MaxLength(500)]
        public string? Mail { get; set; }

        /// <summary>
        /// Gets or sets the user principal name.
        /// </summary>
        [MaxLength(100)]
        public string? UserPrincipalName { get; set; }

        /// <summary>
        /// Gets or sets the user updated date.
        /// </summary>
        [MaxLength(22)]
        public string? UserUpdatedDate { get; set; }

        /// <summary>
        /// Gets or sets the user last sign-in date.
        /// </summary>
        [MaxLength(22)]
        public string? UserLastSignInDate { get; set; }

        /// <summary>
        /// Gets or sets the sign-in captured date.
        /// </summary>
        [MaxLength(22)]
        public string? SignInCapturedDate { get; set; }
    }
}
