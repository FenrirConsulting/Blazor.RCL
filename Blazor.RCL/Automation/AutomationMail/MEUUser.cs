using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blazor.RCL.Automation.AutomationMail
{
    /// <summary>
    /// Represents a user in the MEU system.
    /// </summary>
    [Table("MEUUser")]
    public class MEUUser
    {
        /// <summary>
        /// Gets or sets the primary key for the MEU user.
        /// </summary>
        [Key]
        public int MEUUserPK { get; set; }

        /// <summary>
        /// Gets or sets the employee number.
        /// </summary>
        public string? EmployeeNumber { get; set; }

        /// <summary>
        /// Gets or sets the email address of the user.
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string EmailAddress { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the source of the user.
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Source { get; set; } = string.Empty;

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