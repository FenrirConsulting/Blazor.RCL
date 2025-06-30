using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Blazor.RCL.Automation.Data;
using Blazor.RCL.Automation.AutomationRequest.Interfaces;
using Blazor.RCL.NLog.LogService.Interface;
using Microsoft.EntityFrameworkCore;

namespace Blazor.RCL.Automation.AutomationRequest.Repositories
{
    /// <summary>
    /// Repository implementation for RequestLog operations in the external database.
    /// </summary>
    public class RequestLogRepository : IRequestLogRepository
    {
        private readonly IDbContextFactory<AutomationRequestDbContext> _contextFactory;
        private readonly ILogHelper _logger;

        /// <summary>
        /// Initializes a new instance of the RequestLogRepository class.
        /// </summary>
        /// <param name="contextFactory">The factory for creating database contexts.</param>
        /// <param name="logger">The logger for error logging.</param>
        public RequestLogRepository(IDbContextFactory<AutomationRequestDbContext> contextFactory, ILogHelper logger)
        {
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc/>
        public async Task<RequestLog> GetByRequestAsync(string request, CancellationToken token = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request))
                {
                    throw new ArgumentException("Request identifier is required.", nameof(request));
                }

                using var context = await _contextFactory.CreateDbContextAsync(token);
                return await context.RequestLogs
                    .AsNoTracking()
                    .Where(rl => rl.Request == request)
                    .FirstOrDefaultAsync(token);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while getting RequestLog by Request: {request}", ex);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<RequestLog> GetBySourceIdAsync(string sourceId, CancellationToken token = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(sourceId))
                {
                    throw new ArgumentException("Source identifier is required.", nameof(sourceId));
                }

                using var context = await _contextFactory.CreateDbContextAsync(token);
                return await context.RequestLogs
                    .AsNoTracking()
                    .Where(rl => rl.SourceId == sourceId)
                    .FirstOrDefaultAsync(token);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while getting RequestLog by SourceId: {sourceId}", ex);
                throw;
            }
        }
    }
}
