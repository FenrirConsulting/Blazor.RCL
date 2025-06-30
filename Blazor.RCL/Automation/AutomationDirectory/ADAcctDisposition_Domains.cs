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
    /// Represents a domain entry for AD Account Disposition.
    /// </summary>
    [Table("ADAcctDisposition_Domains")]
    public class ADAcctDisposition_Domains
    {
        /// <summary>
        /// Gets or sets the domain identifier.
        /// </summary>
        [Key]
        public int DomainId { get; set; }

        /// <summary>
        /// Gets or sets the domain name.
        /// </summary>
        [MaxLength(50)]
        public string? DomainName { get; set; }

        /// <summary>
        /// Gets or sets the Fully Qualified Domain Name.
        /// </summary>
        [MaxLength(200)]
        public string? FQDN { get; set; }

        /// <summary>
        /// Gets or sets the Primary Domain Controller.
        /// </summary>
        [MaxLength(200)]
        public string? PDC { get; set; }

        /// <summary>
        /// Gets or sets the Organizational Unit for terminated users.
        /// </summary>
        [MaxLength(1000)]
        public string? TermUsersOU { get; set; }

        /// <summary>
        /// Gets or sets the credential domain.
        /// </summary>
        [MaxLength(50)]
        public string? CredentialDomain { get; set; }
    }
}
