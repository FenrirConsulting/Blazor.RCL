using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Blazor.RCL.Application.Interfaces;
using Blazor.RCL.Domain.Entities;
using Blazor.RCL.NLog.LogService.Interface;
using Microsoft.EntityFrameworkCore;

namespace Blazor.RCL.Infrastructure.Data.Repositories
{
    /// <summary>
    /// Repository implementation for ServiceAccount operations using Entity Framework Core.
    /// </summary>
    public class ServiceAccountRepository : IServiceAccountRepository
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly ILogHelper _logger;

        /// <summary>
        /// Initializes a new instance of the ServiceAccountRepository class.
        /// </summary>
        /// <param name="contextFactory">The factory for creating database contexts.</param>
        /// <param name="logger">The logger for error logging.</param>
        public ServiceAccountRepository(IDbContextFactory<AppDbContext> contextFactory, ILogHelper logger)
        {
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc />
        public async Task<IEnumerable<ServiceAccount>> GetAllAsync(CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                return await context.ServiceAccounts.ToListAsync(token);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurred while getting all ServiceAccounts", ex);
                throw;
            }
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentException">Thrown when the ID is invalid.</exception>
        public async Task<ServiceAccount?> GetByIdAsync(long id, CancellationToken token = default)
        {
            try
            {
                // Validate ID parameter
                if (id <= 0)
                {
                    throw new ArgumentException("ID must be a positive number.", nameof(id));
                }
                
                using var context = await _contextFactory.CreateDbContextAsync(token);
                return await context.ServiceAccounts.FindAsync(new object[] { id }, token);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while getting ServiceAccount with ID: {id}", ex);
                throw;
            }
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">Thrown when the account is null.</exception>
        /// <exception cref="ArgumentException">Thrown when required fields are missing or empty.</exception>
        public async Task AddAsync(ServiceAccount account, CancellationToken token = default)
        {
            try
            {
                // Validate account parameter
                ValidateServiceAccount(account);
                
                using var context = await _contextFactory.CreateDbContextAsync(token);
                await context.ServiceAccounts.AddAsync(account, token);
                await context.SaveChangesAsync(token);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurred while adding ServiceAccount", ex);
                throw;
            }
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">Thrown when the account is null.</exception>
        /// <exception cref="ArgumentException">Thrown when required fields are missing or empty.</exception>
        public async Task UpdateAsync(ServiceAccount account, CancellationToken token = default)
        {
            try
            {
                // Validate account parameter
                ValidateServiceAccount(account);
                
                // Ensure the ID is valid for an update operation
                if (account.Id <= 0)
                {
                    throw new ArgumentException("Service account ID must be a positive number for update operations.", nameof(account));
                }
                
                using var context = await _contextFactory.CreateDbContextAsync(token);
                context.ServiceAccounts.Update(account);
                await context.SaveChangesAsync(token);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while updating ServiceAccount with ID: {account?.Id}", ex);
                throw;
            }
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentException">Thrown when the ID is invalid.</exception>
        public async Task RemoveAsync(long id, CancellationToken token = default)
        {
            try
            {
                // Validate ID parameter
                if (id <= 0)
                {
                    throw new ArgumentException("ID must be a positive number.", nameof(id));
                }
                
                using var context = await _contextFactory.CreateDbContextAsync(token);
                var account = await context.ServiceAccounts.FindAsync(new object[] { id }, token);
                if (account != null)
                {
                    context.ServiceAccounts.Remove(account);
                    await context.SaveChangesAsync(token);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while removing ServiceAccount with ID: {id}", ex);
                throw;
            }
        }
        
        /// <summary>
        /// Validates that a ServiceAccount meets all requirements.
        /// </summary>
        /// <param name="account">The service account to validate.</param>
        /// <exception cref="ArgumentNullException">Thrown when the account is null.</exception>
        /// <exception cref="ArgumentException">Thrown when required fields are missing or empty or exceed max length.</exception>
        private void ValidateServiceAccount(ServiceAccount account)
        {
            if (account == null)
            {
                throw new ArgumentNullException(nameof(account), "Service account cannot be null.");
            }

            // Validate SAM field
            if (string.IsNullOrWhiteSpace(account.SAM))
            {
                throw new ArgumentException("SAM is required and cannot be empty.", nameof(account));
            }
            if (account.SAM.Length > 50)
            {
                throw new ArgumentException("SAM cannot exceed 50 characters.", nameof(account));
            }

            // Validate Domain field
            if (string.IsNullOrWhiteSpace(account.Domain))
            {
                throw new ArgumentException("Domain is required and cannot be empty.", nameof(account));
            }
            if (account.Domain.Length > 50)
            {
                throw new ArgumentException("Domain cannot exceed 50 characters.", nameof(account));
            }

            // Validate KeyType field
            if (string.IsNullOrWhiteSpace(account.KeyType))
            {
                throw new ArgumentException("KeyType is required and cannot be empty.", nameof(account));
            }
            if (account.KeyType.Length > 50)
            {
                throw new ArgumentException("KeyType cannot exceed 50 characters.", nameof(account));
            }

            // Validate KeyPath field
            if (string.IsNullOrWhiteSpace(account.KeyPath))
            {
                throw new ArgumentException("KeyPath is required and cannot be empty.", nameof(account));
            }
            if (account.KeyPath.Length > 100)
            {
                throw new ArgumentException("KeyPath cannot exceed 100 characters.", nameof(account));
            }
        }
    }
}