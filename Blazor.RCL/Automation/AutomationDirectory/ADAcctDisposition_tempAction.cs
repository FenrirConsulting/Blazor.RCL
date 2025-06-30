using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blazor.RCL.Automation.AutomationDirectory
{
    /// <summary>
    /// Represents a temporary action record for AD Account Disposition.
    /// </summary>
    [Table("ADAcctDisposition_tempAction")]
    public class ADAcctDisposition_tempAction
    {
        /// <summary>
        /// Gets or sets the employee number.
        /// </summary>
        public string? EmployeeNumber { get; set; }

        /// <summary>
        /// Gets or sets the temporary action identifier.
        /// </summary>
        public int TempActionID { get; set; }

        /// <summary>
        /// Gets or sets the action to be performed.
        /// </summary>
        public string? Action { get; set; }

        /// <summary>
        /// Gets or sets the type of action.
        /// </summary>
        public string? ActionType { get; set; }

        /// <summary>
        /// Gets or sets the value associated with the action.
        /// </summary>
        public string? ActionValue { get; set; }

        /// <summary>
        /// Gets or sets comments related to the action.
        /// </summary>
        public string? ActionComment { get; set; }

        /// <summary>
        /// Gets or sets the result of the action.
        /// </summary>
        public string? ActionResult { get; set; }

        /// <summary>
        /// Gets or sets comments related to the action result.
        /// </summary>
        public string? ActionResultComment { get; set; }

        /// <summary>
        /// Gets or sets the batch identifier.
        /// </summary>
        public string? BatchId { get; set; }
    }
}