using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blazor.RCL.Automation.AutomationApp
{
    /// <summary>
    /// Represents a required action environment.
    /// </summary>
    [Table("RequiredActionEnvironment")]
    public class RequiredActionEnvironment
    {
        /// <summary>
        /// Gets or sets the primary key for the required action environment.
        /// </summary>
        [Key]
        public short RequiredActionEnvironmentPK { get; set; }

        /// <summary>
        /// Gets or sets the foreign key for the required action code.
        /// </summary>
        public short RequiredActionCodeFK { get; set; }

        /// <summary>
        /// Gets or sets the name of the environment.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string EnvironmentName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description of the environment.
        /// </summary>
        public string? EnvironmentDesc { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the required action environment is active.
        /// </summary>
        public bool ActiveInd { get; set; }

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