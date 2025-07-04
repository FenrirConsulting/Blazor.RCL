﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Blazor.RCL.Domain.Entities;

namespace Blazor.RCL.Application.Interfaces
{
    /// <summary>
    /// Defines operations for managing ServiceAccount configurations.
    /// </summary>
    public interface IServiceAccountRepository
    {
        /// <summary>
        /// Retrieves all ServiceAccounts asynchronously.
        /// </summary>
        /// <param name="token">Optional. A cancellation token to cancel the operation.</param>
        /// <returns>An enumerable of ServiceAccount configurations.</returns>
        Task<IEnumerable<ServiceAccount>> GetAllAsync(CancellationToken token = default);

        /// <summary>
        /// Retrieves a specific ServiceAccount by its ID asynchronously.
        /// </summary>
        /// <param name="id">The ID of the ServiceAccount.</param>
        /// <param name="token">Optional. A cancellation token to cancel the operation.</param>
        /// <returns>The ServiceAccount configuration, or null if not found.</returns>
        /// <exception cref="System.ArgumentException">Thrown when the ID is invalid.</exception>
        Task<ServiceAccount?> GetByIdAsync(long id, CancellationToken token = default);

        /// <summary>
        /// Adds a new ServiceAccount configuration asynchronously.
        /// </summary>
        /// <param name="account">The ServiceAccount configuration to add.</param>
        /// <param name="token">Optional. A cancellation token to cancel the operation.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when the account is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown when required fields are missing, empty, or exceed maximum length.</exception>
        Task AddAsync(ServiceAccount account, CancellationToken token = default);

        /// <summary>
        /// Updates an existing ServiceAccount configuration asynchronously.
        /// </summary>
        /// <param name="account">The ServiceAccount configuration to update.</param>
        /// <param name="token">Optional. A cancellation token to cancel the operation.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when the account is null.</exception>
        /// <exception cref="System.ArgumentException">Thrown when required fields are missing, empty, exceed maximum length, or the ID is invalid.</exception>
        Task UpdateAsync(ServiceAccount account, CancellationToken token = default);

        /// <summary>
        /// Removes a ServiceAccount configuration asynchronously.
        /// </summary>
        /// <param name="id">The ID of the ServiceAccount configuration to remove.</param>
        /// <param name="token">Optional. A cancellation token to cancel the operation.</param>
        /// <exception cref="System.ArgumentException">Thrown when the ID is invalid.</exception>
        Task RemoveAsync(long id, CancellationToken token = default);
    }
}