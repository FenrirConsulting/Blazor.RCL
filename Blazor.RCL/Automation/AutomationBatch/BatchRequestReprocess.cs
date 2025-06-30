using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blazor.RCL.Domain.Entities.AutomationBatch
{
    /// <summary>
    /// Represents a batch request reprocess record in the Automation batch processing system.
    /// </summary>
    [Table("BatchRequestReprocess")]
    public class BatchRequestReprocess
    {
        #region Properties

        /// <summary>
        /// Gets or sets the unique identifier for the batch request reprocess.
        /// </summary>
        [Key]
        public int? BatchRequestReprocessPK { get; set; }

        /// <summary>
        /// Gets or sets the batch request foreign key.
        /// </summary>
        public int? BatchRequestFK { get; set; }

        /// <summary>
        /// Gets or sets the batch request status foreign key.
        /// </summary>
        public int? BatchRequestStatusFK { get; set; }

        /// <summary>
        /// Gets or sets the batch request reprocess counter.
        /// </summary>
        public int? BatchRequestReprocessCounter { get; set; }

        #endregion
    }
}