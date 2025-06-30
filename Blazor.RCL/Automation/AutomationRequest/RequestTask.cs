using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blazor.RCL.Automation.AutomationRequest
{
    /// <summary>
    /// Represents a task associated with a request.
    /// </summary>
    [Table("RequestTask")]
    public class RequestTask
    {
        /// <summary>
        /// Gets or sets the primary key for the request task.
        /// </summary>
        [Key]
        public int RequestTaskPK { get; set; }

        /// <summary>
        /// Gets or sets the foreign key for the request log.
        /// </summary>
        public int? RequestLogFK { get; set; }

        /// <summary>
        /// Gets or sets the code for the task.
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string TaskCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the sequence number of the task.
        /// </summary>
        [Required]
        public short TaskSequenceNumber { get; set; }

        /// <summary>
        /// Gets or sets the description of the task.
        /// </summary>
        public string? TaskDesc { get; set; }

        /// <summary>
        /// Gets or sets the data associated with the task.
        /// </summary>
        [Required]
        public string RequestTaskData { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether to wait for the previous task.
        /// </summary>
        public bool? WaitForPreviousTaskInd { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the task is restrictive.
        /// </summary>
        [Required]
        public bool RestrictiveTaskInd { get; set; }

        /// <summary>
        /// Gets or sets the sleep duration for the task.
        /// </summary>
        public short? SleepDuration { get; set; }

        /// <summary>
        /// Gets or sets the scheduled start time for the task.
        /// </summary>
        public DateTime? ScheduledStart { get; set; }

        /// <summary>
        /// Gets or sets the actual start time for the task.
        /// </summary>
        public DateTime? ActualStart { get; set; }

        /// <summary>
        /// Gets or sets the actual end time for the task.
        /// </summary>
        public DateTime? ActualEnd { get; set; }

        /// <summary>
        /// Gets or sets the average duration of the task.
        /// </summary>
        public short? AverageDuration { get; set; }

        /// <summary>
        /// Gets or sets the path to the PowerShell script for the task.
        /// </summary>
        public string? PowerShellscriptPath { get; set; }

        /// <summary>
        /// Gets or sets the endpoint URL for the task.
        /// </summary>
        public string? EndPointURL { get; set; }

        /// <summary>
        /// Gets or sets the foreign key for the task status code.
        /// </summary>
        [Required]
        public short TaskStatusCodeFK { get; set; }

        /// <summary>
        /// Gets or sets the date of the task status.
        /// </summary>
        [Required]
        public DateTime TaskStatusDate { get; set; }

        /// <summary>
        /// Gets or sets the comments for the task status.
        /// </summary>
        public string? TaskStatusComments { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the task is ready.
        /// </summary>
        [Required]
        public bool ReadinessInd { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether metrics count is enabled.
        /// </summary>
        [Required]
        public bool MetricsCountInd { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the task should be reprocessed if it fails.
        /// </summary>
        [Required]
        public bool FailedTaskReprocessInd { get; set; }

        /// <summary>
        /// Gets or sets the batch ID for the task.
        /// </summary>
        public int? BatchId { get; set; }

        /// <summary>
        /// Gets or sets the reprocess counter for the task.
        /// </summary>
        public int? ReprocessCounter { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the task is a post-processing task.
        /// </summary>
        public bool? PostProcessingTaskInd { get; set; }

        /// <summary>
        /// Gets or sets the delay for post-processing.
        /// </summary>
        public int? PostProcessingDelay { get; set; }

        /// <summary>
        /// Gets or sets the value for the task lineup item.
        /// </summary>
        public string? TaskLineupItemValue { get; set; }

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

        /// <summary>
        /// Gets or sets the name of the request agent server.
        /// </summary>
        public string? RequestAgentServerName { get; set; }
    }
}