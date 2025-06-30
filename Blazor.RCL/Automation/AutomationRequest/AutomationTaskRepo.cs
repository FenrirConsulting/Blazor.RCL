using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blazor.RCL.Automation.AutomationRequest
{
    /// <summary>
    /// Represents a repository for automation tasks.
    /// </summary>
    [Table("AutomationTaskRepo")]
    public class AutomationTaskRepo
    {
        /// <summary>
        /// Gets or sets the primary key for the automation task repository.
        /// </summary>
        [Key]
        public int AutomationTaskRepoPK { get; set; }

        /// <summary>
        /// Gets or sets the foreign key for the valid access type code.
        /// </summary>
        public short? ValidAccessTypeCodeFK { get; set; }

        /// <summary>
        /// Gets or sets the code for the task.
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string TaskCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the sequence number for the task.
        /// </summary>
        [Required]
        public short TaskSequenceNumber { get; set; }

        /// <summary>
        /// Gets or sets the description of the generic task.
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string GenericTaskDesc { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether to wait for the previous task.
        /// </summary>
        public bool WaitForPreviousTaskInd { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the task is restrictive.
        /// </summary>
        public bool RestrictiveTaskInd { get; set; }

        /// <summary>
        /// Gets or sets the sleep duration for the task.
        /// </summary>
        public short? SleepDuration { get; set; }

        /// <summary>
        /// Gets or sets the path to the PowerShell script.
        /// </summary>
        [MaxLength(200)]
        public string? PowerShellscriptPath { get; set; }

        /// <summary>
        /// Gets or sets the endpoint URL for the task.
        /// </summary>
        [MaxLength(500)]
        public string? EndPointURL { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether itemized tasks are needed.
        /// </summary>
        public bool NeedItemizedTasksInd { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether metrics count is enabled.
        /// </summary>
        public bool MetricsCountInd { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to reprocess failed tasks.
        /// </summary>
        public bool FailedTaskReprocessInd { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the task is a post-processing task.
        /// </summary>
        public bool? PostProcessingTaskInd { get; set; }

        /// <summary>
        /// Gets or sets the delay for post-processing.
        /// </summary>
        public int? PostProcessingDelay { get; set; }

        /// <summary>
        /// Gets or sets the indicator for pre-approval validation task.
        /// </summary>
        public int? PreApprovalValidationTaskInd { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the task is active.
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