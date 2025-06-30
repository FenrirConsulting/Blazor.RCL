using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blazor.RCL.Automation.AutomationRequest
{
    /// <summary>
    /// Represents requests that are to be archived.
    /// </summary>
    [Table("RequestsToArchive")]
    public class RequestsToArchive
    {
        /// <summary>
        /// Gets or sets the primary key for the request log.
        /// </summary>
        [Key]
        public int RequestLogPK { get; set; }
    }
}