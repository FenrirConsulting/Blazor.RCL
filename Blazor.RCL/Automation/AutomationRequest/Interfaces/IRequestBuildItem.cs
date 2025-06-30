using Blazor.RCL.Automation.AutomationTasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazor.RCL.Automation.AutomationRequest.Interfaces
{
    /// <summary>
    /// Defines the base interface for items used in building requests.
    /// This contains only the essential properties that all request build items should have.
    /// Specific implementations should extend this interface with additional properties.
    /// </summary>
    public interface IRequestBuildItem
    {
        /// <summary>
        /// Gets the action to be performed on the item.
        /// </summary>
        string Action { get; }
        
        /// <summary>
        /// Gets the user details associated with the item.
        /// This is optional and may be null for some request types.
        /// </summary>
        ADUserDetails? UserDetails { get; }
    }
}