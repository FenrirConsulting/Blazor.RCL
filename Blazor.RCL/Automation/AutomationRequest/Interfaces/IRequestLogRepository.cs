using System;
using System.Threading;
using System.Threading.Tasks;
using Blazor.RCL.Automation.AutomationRequest;

namespace Blazor.RCL.Automation.AutomationRequest.Interfaces
{
    /// <summary>
    /// Interface for operations on RequestLog entities in the external database.
    /// </summary>
    public interface IRequestLogRepository
    {
        /// <summary>
        /// Gets a RequestLog by its request identifier.
        /// </summary>
        /// <param name="request">The request identifier.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The RequestLog if found; otherwise, null.</returns>
        Task<RequestLog> GetByRequestAsync(string request, CancellationToken token = default);
        
        /// <summary>
        /// Gets a RequestLog by its source identifier.
        /// </summary>
        /// <param name="sourceId">The source identifier.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The RequestLog if found; otherwise, null.</returns>
        Task<RequestLog> GetBySourceIdAsync(string sourceId, CancellationToken token = default);
    }
}
