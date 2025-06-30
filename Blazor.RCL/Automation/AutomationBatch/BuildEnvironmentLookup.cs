using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blazor.RCL.Domain.Entities.AutomationBatch
{
    /// <summary>
    /// Represents a build environment lookup in the Automation batch processing system.
    /// </summary>
    [Table("BuildEnvironmentLookup")]
    public class BuildEnvironmentLookup
    {
        #region Properties

        /// <summary>
        /// Gets or sets the unique identifier for the build environment lookup.
        /// </summary>
        [Key]
        public short BuildEnvironmentLookupPK { get; set; }

        /// <summary>
        /// Gets or sets the batch request type code foreign key.
        /// </summary>
        [Required]
        public short BatchRequestTypeCodeFK { get; set; }

        /// <summary>
        /// Gets or sets the build environment.
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string? BuildEnvironment { get; set; }

        /// <summary>
        /// Gets or sets the lookup key.
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string? LookupKey { get; set; }

        /// <summary>
        /// Gets or sets the lookup value.
        /// </summary>
        [Required]
        public string? LookupValue { get; set; }

        /// <summary>
        /// Gets or sets the username of the user who created the record.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string? AuditAddUserName { get; set; }

        /// <summary>
        /// Gets or sets the date when the record was created.
        /// </summary>
        [Required]
        public DateTime AuditAddDate { get; set; }

        /// <summary>
        /// Gets or sets the username of the user who last modified the record.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string? AuditChangeUserName { get; set; }

        /// <summary>
        /// Gets or sets the date when the record was last modified.
        /// </summary>
        [Required]
        public DateTime AuditChangeDate { get; set; }

        #endregion
    }
}