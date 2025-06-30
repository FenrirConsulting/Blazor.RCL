using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Blazor.RCL.Application.Interfaces;
using Blazor.RCL.Domain.Entities;
using Blazor.RCL.NLog.LogService.Interface;
using Microsoft.EntityFrameworkCore;

namespace Blazor.RCL.Infrastructure.Data.Repositories
{
    /// <summary>
    /// Repository implementation for RequestStatusCode operations.
    /// </summary>
    public class RequestStatusCodeRepository : IRequestStatusCodeRepository
    {
        #region Fields

        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly ILogHelper _logger;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the RequestStatusCodeRepository class.
        /// </summary>
        /// <param name="contextFactory">The factory for creating database contexts.</param>
        /// <param name="logger">The logger for error logging.</param>
        public RequestStatusCodeRepository(IDbContextFactory<AppDbContext> contextFactory, ILogHelper logger)
        {
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #endregion

        #region Public Methods

        /// <inheritdoc/>
        public async Task<RequestStatusCode> GetByIdAsync(int statusCodePK, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                return await context.RequestStatusCodes
                    .FirstOrDefaultAsync(sc => sc.RequestStatusCodePK == statusCodePK, token);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while getting RequestStatusCode by ID: {statusCodePK}", ex);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<RequestStatusCode>> GetAllActiveAsync(CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                return await context.RequestStatusCodes
                    .Where(sc => sc.ActiveInd)
                    .ToListAsync(token);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurred while getting active RequestStatusCodes", ex);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<RequestStatusCode>> GetAllAsync(CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                return await context.RequestStatusCodes.ToListAsync(token);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurred while getting all RequestStatusCodes", ex);
                throw;
            }
        }

        #endregion
    }
}