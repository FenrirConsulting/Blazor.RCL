using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Blazor.RCL.Domain.Entities;

namespace Blazor.RCL.Application.Interfaces
{
    /// <summary>
    /// Defines operations for managing server host configurations.
    /// </summary>
    public interface IServerHostRepository
    {
        /// <summary>
        /// Gets all server hosts asynchronously.
        /// </summary>
        /// <param name="token">Optional. A cancellation token to cancel the operation.</param>
        /// <returns>Collection of server hosts.</returns>
        Task<IEnumerable<ServerHost>> GetAllAsync(CancellationToken token = default);

        /// <summary>
        /// Gets a server host by its identifier asynchronously.
        /// </summary>
        /// <param name="id">The server host identifier.</param>
        /// <param name="token">Optional. A cancellation token to cancel the operation.</param>
        /// <returns>The server host if found; otherwise, null.</returns>
        /// <exception cref="System.ArgumentException">Thrown when the ID is invalid.</exception>
        Task<ServerHost?> GetByIdAsync(int id, CancellationToken token = default);

        /// <summary>
        /// Adds a new server host asynchronously.
        /// </summary>
        /// <param name="serverHost">The server host to add.</param>
        /// <param name="token">Optional. A cancellation token to cancel the operation.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when the server host is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown when required fields are missing, empty, or exceed maximum length.</exception>
        Task AddAsync(ServerHost serverHost, CancellationToken token = default);

        /// <summary>
        /// Updates an existing server host asynchronously.
        /// </summary>
        /// <param name="serverHost">The server host to update.</param>
        /// <param name="token">Optional. A cancellation token to cancel the operation.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when the server host is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown when required fields are missing, empty, exceed maximum length, or the ID is invalid.</exception>
        Task UpdateAsync(ServerHost serverHost, CancellationToken token = default);

        /// <summary>
        /// Removes a server host asynchronously.
        /// </summary>
        /// <param name="id">The identifier of the server host to remove.</param>
        /// <param name="token">Optional. A cancellation token to cancel the operation.</param>
        /// <exception cref="System.ArgumentException">Thrown when the ID is invalid.</exception>
        Task RemoveAsync(int id, CancellationToken token = default);
    }
}
