using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Blazor.RCL.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Blazor.RCL.NLog.LogService.Interface;
using Blazor.RCL.Domain.Entities.Configuration;

namespace Blazor.RCL.Infrastructure.Data.Repositories
{
    /// <summary>
    /// Repository implementation for ToolsConfiguration operations using Entity Framework Core.
    /// </summary>
    public class ToolsConfigurationRepository : IToolsConfigurationRepository
    {
        #region Fields

        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly ILogHelper _logger;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the ToolsConfigurationRepository class.
        /// </summary>
        /// <param name="contextFactory">The factory for creating database contexts.</param>
        /// <param name="logger">The logger for error logging.</param>
        public ToolsConfigurationRepository(IDbContextFactory<AppDbContext> contextFactory, ILogHelper logger)
        {
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Retrieves a ToolsConfiguration by application and setting.
        /// </summary>
        /// <param name="application">The application name.</param>
        /// <param name="setting">The setting name.</param>
        /// <param name="token">Cancellation token.</param>
        /// <returns>The ToolsConfiguration if found; otherwise, null.</returns>
        /// <exception cref="ArgumentException">Thrown when required parameters are missing or empty.</exception>
        public async Task<ToolsConfiguration?> GetByApplicationAndSettingAsync(string application, string setting, CancellationToken token = default)
        {
            try
            {
                // Validate parameters
                if (string.IsNullOrWhiteSpace(application))
                {
                    throw new ArgumentException("Application name is required.", nameof(application));
                }

                if (string.IsNullOrWhiteSpace(setting))
                {
                    throw new ArgumentException("Setting name is required.", nameof(setting));
                }

                using var context = await _contextFactory.CreateDbContextAsync(token);
                return await context.ToolsConfiguration
                    .FirstOrDefaultAsync(tc => tc.Application == application && tc.Setting == setting, token);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while getting ToolsConfiguration for application: {application}, setting: {setting}", ex);
                throw;
            }
        }

        /// <summary>
        /// Retrieves the value of a ToolsConfiguration by application and setting.
        /// </summary>
        /// <param name="application">The application name.</param>
        /// <param name="setting">The setting name.</param>
        /// <param name="token">Cancellation token.</param>
        /// <returns>The value if found; otherwise, null.</returns>
        /// <exception cref="ArgumentException">Thrown when required parameters are missing or empty.</exception>
        public async Task<string?> GetValueAsync(string application, string setting, CancellationToken token = default)
        {
            try
            {
                // Parameter validation is handled by GetByApplicationAndSettingAsync
                var toolsConfiguration = await GetByApplicationAndSettingAsync(application, setting, token);
                return toolsConfiguration?.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while getting value for application: {application}, setting: {setting}", ex);
                throw;
            }
        }

        /// <summary>
        /// Retrieves all ToolsConfigurations for a specific application.
        /// </summary>
        /// <param name="application">The application name.</param>
        /// <param name="token">Cancellation token.</param>
        /// <returns>A collection of ToolsConfigurations.</returns>
        /// <exception cref="ArgumentException">Thrown when required parameters are missing or empty.</exception>
        public async Task<IEnumerable<ToolsConfiguration>> GetAllByApplicationAsync(string application, CancellationToken token = default)
        {
            try
            {
                // Validate parameters
                if (string.IsNullOrWhiteSpace(application))
                {
                    throw new ArgumentException("Application name is required.", nameof(application));
                }

                using var context = await _contextFactory.CreateDbContextAsync(token);
                return await context.ToolsConfiguration
                    .Where(tc => tc.Application == application)
                    .ToListAsync(token);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while getting all ToolsConfigurations for application: {application}", ex);
                throw;
            }
        }

        /// <summary>
        /// Sets a ToolsConfiguration, updating if it exists or adding if it doesn't.
        /// </summary>
        /// <param name="toolsConfiguration">The ToolsConfiguration to set.</param>
        /// <param name="token">Cancellation token.</param>
        /// <exception cref="ArgumentException">Thrown when required fields are missing or empty.</exception>
        public async Task SetAsync(ToolsConfiguration toolsConfiguration, CancellationToken token = default)
        {
            try
            {
                // Validate required fields
                if (toolsConfiguration == null)
                {
                    throw new ArgumentNullException(nameof(toolsConfiguration), "ToolsConfiguration cannot be null.");
                }

                // Validate Setting field
                if (string.IsNullOrWhiteSpace(toolsConfiguration.Setting))
                {
                    throw new ArgumentException("Setting is required and cannot be empty.", nameof(toolsConfiguration));
                }

                // Validate Application field
                if (string.IsNullOrWhiteSpace(toolsConfiguration.Application))
                {
                    throw new ArgumentException("Application is required and cannot be empty.", nameof(toolsConfiguration));
                }

                // Validate Value field
                if (string.IsNullOrWhiteSpace(toolsConfiguration.Value))
                {
                    throw new ArgumentException("Value is required and cannot be empty.", nameof(toolsConfiguration));
                }

                using var context = await _contextFactory.CreateDbContextAsync(token);
                var existingConfig = await context.ToolsConfiguration
                    .FirstOrDefaultAsync(tc => tc.Id == toolsConfiguration.Id, token);
                if (existingConfig == null)
                {
                    await context.ToolsConfiguration.AddAsync(toolsConfiguration, token);
                }
                else
                {
                    context.Entry(existingConfig).CurrentValues.SetValues(toolsConfiguration);
                }
                await context.SaveChangesAsync(token);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while setting ToolsConfiguration with ID: {toolsConfiguration?.Id}", ex);
                throw;
            }
        }

        /// <summary>
        /// Removes a ToolsConfiguration.
        /// </summary>
        /// <param name="config">The ToolsConfiguration to remove.</param>
        /// <param name="token">Cancellation token.</param>
        /// <exception cref="ArgumentNullException">Thrown when configuration is null.</exception>
        /// <exception cref="ArgumentException">Thrown when the ID is invalid.</exception>
        public async Task RemoveAsync(ToolsConfiguration config, CancellationToken token = default)
        {
            try
            {
                // Validate parameters
                if (config == null)
                {
                    throw new ArgumentNullException(nameof(config), "Configuration cannot be null.");
                }

                if (config.Id <= 0)
                {
                    throw new ArgumentException("Configuration ID must be a positive number.", nameof(config));
                }

                using var context = await _contextFactory.CreateDbContextAsync(token);
                var toolsConfiguration = await context.ToolsConfiguration.FindAsync(new object[] { config.Id }, token);
                if (toolsConfiguration != null)
                {
                    context.ToolsConfiguration.Remove(toolsConfiguration);
                    await context.SaveChangesAsync(token);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while removing ToolsConfiguration with ID: {config?.Id}", ex);
                throw;
            }
        }

        #endregion
    }
}