using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Blazor.RCL.Domain.Entities
{
    /// <summary>
    /// Represents a request status code in the database.
    /// </summary>
    public class RequestStatusCode
    {
        /// <summary>
        /// Gets or sets the primary key for the status code.
        /// </summary>
        [Key]
        public int RequestStatusCodePK { get; set; }
        
        /// <summary>
        /// Gets or sets the description of the status.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string RequestStatusDesc { get; set; }
        
        /// <summary>
        /// Gets or sets the display text for the status.
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string DisplayStatusText { get; set; }
        
        /// <summary>
        /// Gets or sets a value indicating whether the status code is active.
        /// </summary>
        [Required]
        public bool ActiveInd { get; set; }
        
        /// <summary>
        /// Navigation property for related tools requests.
        /// </summary>
        public virtual ICollection<ToolsRequest> ToolsRequest { get; set; }
    }
}