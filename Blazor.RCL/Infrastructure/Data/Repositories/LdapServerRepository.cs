using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Blazor.RCL.Application.Interfaces;
using Blazor.RCL.Domain.Entities.Configuration;
using Blazor.RCL.NLog.LogService.Interface;
using Microsoft.EntityFrameworkCore;

namespace Blazor.RCL.Infrastructure.Data.Repositories
{
    /// <summary>
    /// Repository implementation for LDAPServer operations using Entity Framework Core.
    /// </summary>
    public class LDAPServerRepository : ILDAPServerRepository
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly ILogHelper _logger;

        /// <summary>
        /// Initializes a new instance of the LDAPServerRepository class.
        /// </summary>
        /// <param name="contextFactory">The factory for creating database contexts.</param>
        /// <param name="logger">The logger for error logging.</param>
        public LDAPServerRepository(IDbContextFactory<AppDbContext> contextFactory, ILogHelper logger)
        {
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc />
        public async Task<IEnumerable<LDAPServer>> GetAllAsync(CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                return await context.LDAPServers.ToListAsync(token);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurred while getting all LDAP servers", ex);
                throw;
            }
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentException">Thrown when the ID is invalid.</exception>
        public async Task<LDAPServer?> GetByIdAsync(long id, CancellationToken token = default)
        {
            try
            {
                // Validate id parameter
                if (id <= 0)
                {
                    throw new ArgumentException("ID must be a positive number.", nameof(id));
                }

                using var context = await _contextFactory.CreateDbContextAsync(token);
                return await context.LDAPServers.FindAsync(new object[] { id }, token);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while getting LDAP server with ID: {id}", ex);
                throw;
            }
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">Thrown when the server is null.</exception>
        /// <exception cref="ArgumentException">Thrown when required fields are missing or empty.</exception>
        public async Task AddAsync(LDAPServer server, CancellationToken token = default)
        {
            try
            {
                // Validate server parameter
                ValidateLDAPServer(server);

                using var context = await _contextFactory.CreateDbContextAsync(token);
                await context.LDAPServers.AddAsync(server, token);
                await context.SaveChangesAsync(token);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurred while adding LDAP server", ex);
                throw;
            }
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">Thrown when the server is null.</exception>
        /// <exception cref="ArgumentException">Thrown when required fields are missing or empty.</exception>
        public async Task UpdateAsync(LDAPServer server, CancellationToken token = default)
        {
            try
            {
                // Validate server parameter
                ValidateLDAPServer(server);

                // Ensure the ID is valid for an update operation
                if (server.Id <= 0)
                {
                    throw new ArgumentException("Server ID must be a positive number for update operations.", nameof(server));
                }

                using var context = await _contextFactory.CreateDbContextAsync(token);
                context.LDAPServers.Update(server);
                await context.SaveChangesAsync(token);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while updating LDAP server with ID: {server?.Id}", ex);
                throw;
            }
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentException">Thrown when the ID is invalid.</exception>
        public async Task RemoveAsync(long id, CancellationToken token = default)
        {
            try
            {
                // Validate id parameter
                if (id <= 0)
                {
                    throw new ArgumentException("ID must be a positive number.", nameof(id));
                }

                using var context = await _contextFactory.CreateDbContextAsync(token);
                var server = await context.LDAPServers.FindAsync(new object[] { id }, token);
                if (server != null)
                {
                    context.LDAPServers.Remove(server);
                    await context.SaveChangesAsync(token);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while removing LDAP server with ID: {id}", ex);
                throw;
            }
        }

        /// <summary>
        /// Validates that an LDAP server meets all requirements.
        /// </summary>
        /// <param name="server">The LDAP server to validate.</param>
        /// <exception cref="ArgumentNullException">Thrown when the server is null.</exception>
        /// <exception cref="ArgumentException">Thrown when required fields are missing or empty or exceed max length.</exception>
        private void ValidateLDAPServer(LDAPServer server)
        {
            if (server == null)
            {
                throw new ArgumentNullException(nameof(server), "LDAP server cannot be null.");
            }

            // Validate Name field
            if (string.IsNullOrWhiteSpace(server.Name))
            {
                throw new ArgumentException("Name is required and cannot be empty.", nameof(server));
            }
            if (server.Name.Length > 50)
            {
                throw new ArgumentException("Name cannot exceed 50 characters.", nameof(server));
            }

            // Validate Server field
            if (string.IsNullOrWhiteSpace(server.Server))
            {
                throw new ArgumentException("Server address is required and cannot be empty.", nameof(server));
            }
            if (server.Server.Length > 100)
            {
                throw new ArgumentException("Server address cannot exceed 100 characters.", nameof(server));
            }

            // Validate Port field
            if (string.IsNullOrWhiteSpace(server.Port))
            {
                throw new ArgumentException("Port is required and cannot be empty.", nameof(server));
            }
            if (server.Port.Length > 50)
            {
                throw new ArgumentException("Port cannot exceed 50 characters.", nameof(server));
            }

            // Validate SearchBase field
            if (string.IsNullOrWhiteSpace(server.SearchBase))
            {
                throw new ArgumentException("Search base is required and cannot be empty.", nameof(server));
            }
            if (server.SearchBase.Length > 200)
            {
                throw new ArgumentException("Search base cannot exceed 200 characters.", nameof(server));
            }

            // Validate AdminSearchBase field
            if (string.IsNullOrWhiteSpace(server.AdminSearchBase))
            {
                throw new ArgumentException("Admin search base is required and cannot be empty.", nameof(server));
            }
            if (server.AdminSearchBase.Length > 200)
            {
                throw new ArgumentException("Admin search base cannot exceed 200 characters.", nameof(server));
            }
        }
    }
}