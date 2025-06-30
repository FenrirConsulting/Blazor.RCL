using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blazor.RCL.Automation.AutomationRequest
{
    /// <summary>
    /// Represents a configuration setting.
    /// </summary>
    [Table("Configuration")]
    public class Configuration
    {
        /// <summary>
        /// Gets or sets the primary key for the configuration.
        /// </summary>
        [Key]
        public int ConfigurationPK { get; set; }

        /// <summary>
        /// Gets or sets the name of the configuration.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string ConfigurationName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the value of the configuration.
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string ConfigurationValue { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets comments related to the configuration.
        /// </summary>
        public string? Comments { get; set; }

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