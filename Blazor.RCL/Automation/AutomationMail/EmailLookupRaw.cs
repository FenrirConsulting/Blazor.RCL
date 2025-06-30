using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blazor.RCL.Automation.AutomationMail
{
    /// <summary>
    /// Represents a raw email lookup entry.
    /// </summary>
    [Table("EmailLookupRaw")]
    public class EmailLookupRaw
    {
        /// <summary>
        /// Gets or sets the email address.
        /// </summary>
        [Required]
        public string EmailAddress { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the employee number.
        /// </summary>
        public string? EmployeeNumber { get; set; }

        /// <summary>
        /// Gets or sets the source of the email lookup.
        /// </summary>
        [Required]
        public string Source { get; set; } = string.Empty;
    }
}