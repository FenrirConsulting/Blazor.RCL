using Blazor.RCL.Application.Interfaces;
using Blazor.RCL.Domain.Entities;
using Blazor.RCL.NLog.LogService.Interface;
using Microsoft.EntityFrameworkCore;

namespace Blazor.RCL.Infrastructure.Data.Repositories
{
    /// <summary>
    /// Repository implementation for server host configurations.
    /// </summary>
    public class ServerHostRepository : IServerHostRepository
    {
        #region Private Fields

        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly ILogHelper _logger;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the ServerHostRepository class.
        /// </summary>
        public ServerHostRepository(IDbContextFactory<AppDbContext> contextFactory, ILogHelper logger)
        {
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #endregion

        #region Public Methods

        /// <inheritdoc/>
        public async Task<IEnumerable<ServerHost>> GetAllAsync(CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                return await context.Set<ServerHost>().ToListAsync(token);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error loading server hosts", ex);
                throw;
            }
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentException">Thrown when the ID is invalid.</exception>
        public async Task<ServerHost?> GetByIdAsync(int id, CancellationToken token = default)
        {
            try
            {
                // Validate ID parameter
                if (id <= 0)
                {
                    throw new ArgumentException("ID must be a positive number.", nameof(id));
                }
                
                using var context = await _contextFactory.CreateDbContextAsync(token);
                return await context.Set<ServerHost>().FindAsync(new object[] { id }, token);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting server host with ID {id}", ex);
                throw;
            }
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">Thrown when the server host is null.</exception>
        /// <exception cref="ArgumentException">Thrown when required fields are missing or empty.</exception>
        public async Task AddAsync(ServerHost serverHost, CancellationToken token = default)
        {
            try
            {
                // Validate server host parameter
                ValidateServerHost(serverHost);
                
                using var context = await _contextFactory.CreateDbContextAsync(token);
                await context.Set<ServerHost>().AddAsync(serverHost, token);
                await context.SaveChangesAsync(token);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error adding server host", ex);
                throw;
            }
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentNullException">Thrown when the server host is null.</exception>
        /// <exception cref="ArgumentException">Thrown when required fields are missing or empty.</exception>
        public async Task UpdateAsync(ServerHost serverHost, CancellationToken token = default)
        {
            try
            {
                // Validate server host parameter
                ValidateServerHost(serverHost);
                
                // Ensure the ID is valid for an update operation
                if (serverHost.Id <= 0)
                {
                    throw new ArgumentException("Server host ID must be a positive number for update operations.", nameof(serverHost));
                }
                
                using var context = await _contextFactory.CreateDbContextAsync(token);
                context.Set<ServerHost>().Update(serverHost);
                await context.SaveChangesAsync(token);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating server host with ID {serverHost?.Id}", ex);
                throw;
            }
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentException">Thrown when the ID is invalid.</exception>
        public async Task RemoveAsync(int id, CancellationToken token = default)
        {
            try
            {
                // Validate ID parameter
                if (id <= 0)
                {
                    throw new ArgumentException("ID must be a positive number.", nameof(id));
                }
                
                using var context = await _contextFactory.CreateDbContextAsync(token);
                var serverHost = await context.Set<ServerHost>().FindAsync(new object[] { id }, token);
                if (serverHost != null)
                {
                    context.Set<ServerHost>().Remove(serverHost);
                    await context.SaveChangesAsync(token);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error removing server host with ID {id}", ex);
                throw;
            }
        }

        #endregion
        
        #region Private Methods
        
        /// <summary>
        /// Validates that a ServerHost meets all requirements.
        /// </summary>
        /// <param name="serverHost">The server host to validate.</param>
        /// <exception cref="ArgumentNullException">Thrown when the server host is null.</exception>
        /// <exception cref="ArgumentException">Thrown when required fields are missing or empty or exceed max length.</exception>
        private void ValidateServerHost(ServerHost serverHost)
        {
            if (serverHost == null)
            {
                throw new ArgumentNullException(nameof(serverHost), "Server host cannot be null.");
            }

            // Validate Hostname field
            if (string.IsNullOrWhiteSpace(serverHost.Hostname))
            {
                throw new ArgumentException("Hostname is required and cannot be empty.", nameof(serverHost));
            }
            if (serverHost.Hostname.Length > 100)
            {
                throw new ArgumentException("Hostname cannot exceed 100 characters.", nameof(serverHost));
            }

            // Validate Environment field
            if (string.IsNullOrWhiteSpace(serverHost.Environment))
            {
                throw new ArgumentException("Environment is required and cannot be empty.", nameof(serverHost));
            }
            if (serverHost.Environment.Length > 50)
            {
                throw new ArgumentException("Environment cannot exceed 50 characters.", nameof(serverHost));
            }

            // Validate Name field
            if (string.IsNullOrWhiteSpace(serverHost.Name))
            {
                throw new ArgumentException("Name is required and cannot be empty.", nameof(serverHost));
            }
            if (serverHost.Name.Length > 100)
            {
                throw new ArgumentException("Name cannot exceed 100 characters.", nameof(serverHost));
            }

            // Validate Role field
            if (string.IsNullOrWhiteSpace(serverHost.Role))
            {
                throw new ArgumentException("Role is required and cannot be empty.", nameof(serverHost));
            }
            if (serverHost.Role.Length > 100)
            {
                throw new ArgumentException("Role cannot exceed 100 characters.", nameof(serverHost));
            }
        }
        
        #endregion
    }
}
