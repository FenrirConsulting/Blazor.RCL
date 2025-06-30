using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Blazor.RCL.Domain.Entities;

namespace Blazor.RCL.Application.Interfaces
{
    /// <summary>
    /// Interface for operations on RequestStatusCode entities.
    /// </summary>
    public interface IRequestStatusCodeRepository
    {
        /// <summary>
        /// Gets a RequestStatusCode by its primary key.
        /// </summary>
        /// <param name="statusCodePK">The primary key of the status code.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The RequestStatusCode if found; otherwise, null.</returns>
        Task<RequestStatusCode> GetByIdAsync(int statusCodePK, CancellationToken token = default);
        
        /// <summary>
        /// Gets all active RequestStatusCodes.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A list of active RequestStatusCodes.</returns>
        Task<IEnumerable<RequestStatusCode>> GetAllActiveAsync(CancellationToken token = default);
        
        /// <summary>
        /// Gets all RequestStatusCodes.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A list of all RequestStatusCodes.</returns>
        Task<IEnumerable<RequestStatusCode>> GetAllAsync(CancellationToken token = default);
    }
}