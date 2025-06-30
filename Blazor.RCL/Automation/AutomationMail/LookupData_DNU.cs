using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blazor.RCL.Automation.AutomationMail
{
    /// <summary>
    /// Represents lookup data.
    /// </summary>
    [Table("LookupData_DNU")]
    public class LookupData_DNU
    {
        /// <summary>
        /// Gets or sets the primary key for the lookup data.
        /// </summary>
        [Key]
        public int LookupDataPK { get; set; }

        /// <summary>
        /// Gets or sets the foreign key for the lookup key code.
        /// </summary>
        public short LookupKeyCodeFK { get; set; }

        /// <summary>
        /// Gets or sets the description of the lookup data.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string LookupDataDesc { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the lookup data is active.
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