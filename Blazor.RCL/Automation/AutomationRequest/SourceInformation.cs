using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blazor.RCL.Automation.AutomationRequest
{
    /// <summary>
    /// Represents information about a source.
    /// </summary>
    [Table("SourceInformation")]
    public class SourceInformation
    {
        /// <summary>
        /// Gets or sets the primary key for the source information.
        /// </summary>
        [Key]
        public int SourceInformationPK { get; set; }

        /// <summary>
        /// Gets or sets the identity of the source.
        /// </summary>
        public string? SourceIdentity { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier for the source identity.
        /// </summary>
        public Guid? SourceIdentityValue { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the source is active.
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