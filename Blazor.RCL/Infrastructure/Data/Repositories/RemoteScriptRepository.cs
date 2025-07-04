﻿using System;
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
    /// Repository implementation for RemoteScript operations using Entity Framework Core.
    /// </summary>
    public class RemoteScriptRepository : IRemoteScriptRepository
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly ILogHelper _logger;

        /// <summary>
        /// Initializes a new instance of the RemoteScriptRepository class.
        /// </summary>
        /// <param name="contextFactory">The factory for creating database contexts.</param>
        /// <param name="logger">The logger for error logging.</param>
        public RemoteScriptRepository(IDbContextFactory<AppDbContext> contextFactory, ILogHelper logger)
        {
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc />
        public async Task<IEnumerable<RemoteScript>> GetAllAsync(CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                return await context.RemoteScripts.ToListAsync(token);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurred while getting all RemoteScripts", ex);
                throw;
            }
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentException">Thrown when the ID is invalid.</exception>
        public async Task<RemoteScript?> GetByIdAsync(long id, CancellationToken token = default)
        {
            try
            {
                // Validate ID parameter
                if (id <= 0)
                {
                    throw new ArgumentException("ID must be a positive number.", nameof(id));
                }
                
                using var context = await _contextFactory.CreateDbContextAsync(token);
                return await context.RemoteScripts.FindAsync(new object[] { id }, token);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while getting RemoteScript with ID: {id}", ex);
                throw;
            }
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">Thrown when the script is null.</exception>
        /// <exception cref="ArgumentException">Thrown when required fields are missing or empty.</exception>
        public async Task AddAsync(RemoteScript script, CancellationToken token = default)
        {
            try
            {
                // Validate script parameter
                ValidateRemoteScript(script);
                
                using var context = await _contextFactory.CreateDbContextAsync(token);
                await context.RemoteScripts.AddAsync(script, token);
                await context.SaveChangesAsync(token);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurred while adding RemoteScript", ex);
                throw;
            }
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">Thrown when the script is null.</exception>
        /// <exception cref="ArgumentException">Thrown when required fields are missing or empty.</exception>
        public async Task UpdateAsync(RemoteScript script, CancellationToken token = default)
        {
            try
            {
                // Validate script parameter
                ValidateRemoteScript(script);
                
                // Ensure the ID is valid for an update operation
                if (script.Id <= 0)
                {
                    throw new ArgumentException("Script ID must be a positive number for update operations.", nameof(script));
                }
                
                using var context = await _contextFactory.CreateDbContextAsync(token);
                context.RemoteScripts.Update(script);
                await context.SaveChangesAsync(token);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while updating RemoteScript with ID: {script?.Id}", ex);
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
                var script = await context.RemoteScripts.FindAsync(new object[] { id }, token);
                if (script != null)
                {
                    context.RemoteScripts.Remove(script);
                    await context.SaveChangesAsync(token);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while removing RemoteScript with ID: {id}", ex);
                throw;
            }
        }
        
        /// <summary>
        /// Validates that a RemoteScript meets all requirements.
        /// </summary>
        /// <param name="script">The RemoteScript to validate.</param>
        /// <exception cref="ArgumentNullException">Thrown when the script is null.</exception>
        /// <exception cref="ArgumentException">Thrown when required fields are missing or empty or exceed max length.</exception>
        private void ValidateRemoteScript(RemoteScript script)
        {
            if (script == null)
            {
                throw new ArgumentNullException(nameof(script), "RemoteScript cannot be null.");
            }

            // Validate Location field
            if (string.IsNullOrWhiteSpace(script.Location))
            {
                throw new ArgumentException("Location is required and cannot be empty.", nameof(script));
            }
            if (script.Location.Length > 500)
            {
                throw new ArgumentException("Location cannot exceed 500 characters.", nameof(script));
            }

            // Validate Name field
            if (string.IsNullOrWhiteSpace(script.Name))
            {
                throw new ArgumentException("Name is required and cannot be empty.", nameof(script));
            }
            if (script.Name.Length > 100)
            {
                throw new ArgumentException("Name cannot exceed 100 characters.", nameof(script));
            }
        }
    }
}