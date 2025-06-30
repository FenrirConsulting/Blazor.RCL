using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blazor.RCL.Domain.Entities.AutomationBatch
{
    /// <summary>
    /// Represents a batch configuration in the Automation batch processing system.
    /// </summary>
    [Table("BatchConfiguration")]
    public class BatchConfiguration
    {
        #region Properties

        /// <summary>
        /// Gets or sets the unique identifier for the batch configuration.
        /// </summary>
        [Key]
        public short BatchConfigurationPK { get; set; }

        /// <summary>
        /// Gets or sets the batch request type code foreign key.
        /// </summary>
        [Required]
        public short BatchRequestTypeCodeFK { get; set; }

        /// <summary>
        /// Gets or sets the workflow name.
        /// </summary>
        [Required]
        [MaxLength(20)]
        public string? WorkflowName { get; set; }

        /// <summary>
        /// Gets or sets the headers information.
        /// </summary>
        [Required]
        public string? HeadersInfo { get; set; }

        /// <summary>
        /// Gets or sets the item data object.
        /// </summary>
        [Required]
        public string? ItemDataObject { get; set; }

        /// <summary>
        /// Gets or sets the rules object.
        /// </summary>
        [Required]
        public string? RulesObject { get; set; }

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