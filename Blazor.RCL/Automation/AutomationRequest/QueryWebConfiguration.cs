using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blazor.RCL.Automation.AutomationRequest
{
    /// <summary>
    /// Represents the configuration for web queries.
    /// </summary>
    [Table("QueryWebConfiguration")]
    public class QueryWebConfiguration
    {
        /// <summary>
        /// Gets or sets the primary key for the query web configuration.
        /// </summary>
        [Key]
        public int QueryWebConfigurationPK { get; set; }

        /// <summary>
        /// Gets or sets the key for the query web.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string QueryWebKey { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the first URI attribute name.
        /// </summary>
        public string? UriAttributeName1 { get; set; }

        /// <summary>
        /// Gets or sets the second URI attribute name.
        /// </summary>
        public string? UriAttributeName2 { get; set; }

        /// <summary>
        /// Gets or sets the third URI attribute name.
        /// </summary>
        public string? UriAttributeName3 { get; set; }

        /// <summary>
        /// Gets or sets the API endpoint URL for the task.
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string TaskAPIEndPointURL { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether pagination is needed.
        /// </summary>
        public bool NeedsPaginationInd { get; set; }

        /// <summary>
        /// Gets or sets the implementation URL.
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string ImplementationURL { get; set; } = string.Empty;

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