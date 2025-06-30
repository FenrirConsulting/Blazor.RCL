using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blazor.RCL.Automation.AutomationApp
{
    /// <summary>
    /// Represents the application database connection information.
    /// </summary>
    [Table("AppDbConnectionInfo")]
    public class AppDbConnectionInfo
    {
        /// <summary>
        /// Gets or sets the primary key for the application database connection info.
        /// </summary>
        [Key]
        public int AppDBConnectionInfoPK { get; set; }

        /// <summary>
        /// Gets or sets the foreign key for the line of business code.
        /// </summary>
        public short LineOfBusinessCodeFK { get; set; }

        /// <summary>
        /// Gets or sets the name of the application.
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string ApplicationName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the name of the database.
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string DatabaseName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the connection string for the database.
        /// </summary>
        [Required]
        public string ConnectionString { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the default tablespace.
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string DefaultTablespace { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the temporary tablespace.
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string TemporaryTablespace { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the profile associated with the connection.
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Profile { get; set; } = string.Empty;

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