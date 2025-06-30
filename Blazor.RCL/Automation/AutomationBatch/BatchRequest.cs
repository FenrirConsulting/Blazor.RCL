using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blazor.RCL.Domain.Entities.AutomationBatch
{
    /// <summary>
    /// Represents a batch request in the Automation batch processing system.
    /// </summary>
    [Table("BatchRequest")]
    public class BatchRequest
    {
        #region Properties

        /// <summary>
        /// Gets or sets the unique identifier for the batch request.
        /// </summary>
        [Key]
        public int BatchRequestPK { get; set; }

        /// <summary>
        /// Gets or sets the type code for the batch request.
        /// </summary>
        [Required]
        public short BatchRequestTypeCodeFK { get; set; }

        /// <summary>
        /// Gets or sets the source of the batch request.
        /// </summary>
        [MaxLength(100)]
        public string? Source { get; set; }

        /// <summary>
        /// Gets or sets the source identifier.
        /// </summary>
        [MaxLength(100)]
        public string? SourceId { get; set; }

        /// <summary>
        /// Gets or sets the request identifier.
        /// </summary>
        [MaxLength(100)]
        public string? Request { get; set; }

        /// <summary>
        /// Gets or sets the request item identifier.
        /// </summary>
        [MaxLength(100)]
        public string? RequestItem { get; set; }

        /// <summary>
        /// Gets or sets the item data.
        /// </summary>
        [Required]
        public string? ItemData { get; set; }

        /// <summary>
        /// Gets or sets the Automation GUID.
        /// </summary>
        public Guid? AutomationGuid { get; set; }

        /// <summary>
        /// Gets or sets the batch identifier.
        /// </summary>
        public int? BatchID { get; set; }

        /// <summary>
        /// Gets or sets the request status code.
        /// </summary>
        [Required]
        public short RequestStatusCodeFK { get; set; }

        /// <summary>
        /// Gets or sets the request status date.
        /// </summary>
        [Required]
        public DateTime RequestStatusDate { get; set; }

        /// <summary>
        /// Gets or sets the request status comments.
        /// </summary>
        public string? RequestStatusComments { get; set; }

        /// <summary>
        /// Gets or sets the task status comments.
        /// </summary>
        public string? TaskStatusComments { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether post-check is complete.
        /// </summary>
        [Required]
        public bool PostCheckCompleteInd { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether test mode is enabled.
        /// </summary>
        [Required]
        public bool TestModeInd { get; set; }

        /// <summary>
        /// Gets or sets the source record identifier.
        /// </summary>
        public int? SourceRecordId { get; set; }

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