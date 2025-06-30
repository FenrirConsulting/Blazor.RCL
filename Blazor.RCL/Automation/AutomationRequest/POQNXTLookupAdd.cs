using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blazor.RCL.Automation.AutomationRequest
{
    /// <summary>
    /// Represents an entry for adding POQNXT lookup.
    /// </summary>
    [Table("POQNXTLookupAdd")]
    public class POQNXTLookupAdd
    {
        /// <summary>
        /// Gets or sets the primary key for the POQNXT lookup add.
        /// </summary>
        [Key]
        public int POQNXTLookupAddPK { get; set; }

        /// <summary>
        /// Gets or sets the role code.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string RoleCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the request type.
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string RequestType { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the environment.
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string Environment { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the XML role code.
        /// </summary>
        public string? XMLRoleCode { get; set; }

        /// <summary>
        /// Gets or sets the XML access group code.
        /// </summary>
        public string? XMLAccessGroupCode { get; set; }

        /// <summary>
        /// Gets or sets the web service environment.
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string WebServiceEnvironment { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the Active Directory group.
        /// </summary>
        public string? ADGroup { get; set; }

        /// <summary>
        /// Gets or sets the Active Directory domain.
        /// </summary>
        public string? ADDomain { get; set; }

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