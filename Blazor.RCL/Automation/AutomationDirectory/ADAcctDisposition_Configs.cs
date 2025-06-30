using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazor.RCL.Automation.AutomationDirectory
{
    /// <summary>
    /// Represents a configuration entry for AD Account Disposition.
    /// </summary>
    [Table("ADAcctDisposition_Configs")]
    public class ADAcctDisposition_Configs
    {
        /// <summary>
        /// Gets or sets the configuration type.
        /// </summary>
        [MaxLength(100)]
        public string? ConfigType { get; set; }

        /// <summary>
        /// Gets or sets the configuration value.
        /// </summary>
        [MaxLength(1000)]
        public string? Value { get; set; }

        /// <summary>
        /// Gets or sets the configuration name.
        /// </summary>
        [MaxLength(30)]
        public string? Name { get; set; }
    }
}
