using Blazor.RCL.Infrastructure.BackgroundServices.Interfaces;
using Blazor.RCL.NLog.LogService.Interface;
using Blazor.RCL.Automation.AutomationRequest.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Blazor.RCL.Infrastructure.BackgroundServices
{
    /// <summary>
    /// Service that polls for request status and updates the database
    /// </summary>
    public class RequestPollingService : IRequestPollingService
    {
        private readonly ILogHelper _logger;
        private readonly BackgroundRequestService _backgroundRequestService;
        private readonly IRequestLogRepository _requestLogRepository;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="RequestPollingService"/> class
        /// </summary>
        public RequestPollingService(
            ILogHelper logger,
            BackgroundRequestService backgroundRequestService,
            IRequestLogRepository requestLogRepository)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _backgroundRequestService = backgroundRequestService ?? throw new ArgumentNullException(nameof(backgroundRequestService));
            _requestLogRepository = requestLogRepository ?? throw new ArgumentNullException(nameof(requestLogRepository));
        }
        
        /// <inheritdoc/>
        public async Task PollAndProcessRequestsAsync(bool checkExternalDatabase, CancellationToken cancellationToken)
        {
            try
            {
                // Get all requests with status code 1 or 2 (Processing)
                var processingRequests1 = await _backgroundRequestService.GetRequestsByStatusAsync(1, cancellationToken);
                var processingRequests2 = await _backgroundRequestService.GetRequestsByStatusAsync(2, cancellationToken);
                
                // Combine the lists
                var processingRequests = new List<Domain.Entities.ToolsRequest>();
                processingRequests.AddRange(processingRequests1);
                processingRequests.AddRange(processingRequests2);
                
                if (!processingRequests.Any())
                {
                    return;
                }
                
                // Process each request
                foreach (var request in processingRequests)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }
                    
                    await ProcessRequestAsync(request, checkExternalDatabase, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error during request polling cycle", ex);
            }
        }
        
        private async Task ProcessRequestAsync(Domain.Entities.ToolsRequest request, bool checkExternalDatabase, CancellationToken cancellationToken)
        {
            try
            {
                // Only check the external database if instructed to do so
                if (checkExternalDatabase)
                {
                    // Check the external database for the request status
                    var (isFinished, statusCode, statusComments) = await CheckExternalDatabaseForStatusAsync(request);
                    
                    if (isFinished)
                    {
                        // Update the request status in the database with the status code from the external database
                        await _backgroundRequestService.UpdateStatusAsync(
                            request.ToolsRequestPK, 
                            statusCode, 
                            statusComments,
                            cancellationToken);
                            
                        bool isReinstateRequest = (statusCode == 3 || statusCode == 8) && 
                            request.AccessSubType == "Reinstate";
                        
                        if (isReinstateRequest)
                        {
                            _logger.LogMessage($"Processing successful reinstate request {request.Request} with status code {statusCode}");
                            await ProcessReinstateCompletionAsync(request, cancellationToken);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing request {request.Request}", ex);
            }
        }
        
        private async Task ProcessReinstateCompletionAsync(Domain.Entities.ToolsRequest request, CancellationToken cancellationToken)
        {
            try
            {
                // Extract the PreviousRequestItem from the Comments field
                string previousRequestId = string.Empty;
                
                if (!string.IsNullOrEmpty(request.Comments))
                {
                    // Check for the "Previous request:" format in the comments
                    int previousRequestIndex = request.Comments.IndexOf("Previous request:", StringComparison.OrdinalIgnoreCase);
                    if (previousRequestIndex >= 0)
                    {
                        // Extract everything after "Previous request:" and before the next ")" if it exists
                        string afterPrefix = request.Comments.Substring(previousRequestIndex + 17); // +17 to skip "Previous request:"
                        int closingParenIndex = afterPrefix.IndexOf(')');
                        if (closingParenIndex > 0)
                        {
                            previousRequestId = afterPrefix.Substring(0, closingParenIndex).Trim();
                        }
                        else
                        {
                            previousRequestId = afterPrefix.Trim();
                        }
                    }
                    else if (request.Comments.Contains("EXT-BATCH-"))
                    {
                        // Look for the EXT-BATCH pattern
                        int extBatchIndex = request.Comments.IndexOf("EXT-BATCH-");
                        if (extBatchIndex >= 0)
                        {
                            string afterPrefix = request.Comments.Substring(extBatchIndex);
                            // Extract up to the next space or end of string
                            int nextSpaceIndex = afterPrefix.IndexOf(' ');
                            if (nextSpaceIndex > 0)
                            {
                                previousRequestId = afterPrefix.Substring(0, nextSpaceIndex).Trim();
                            }
                            else
                            {
                                previousRequestId = afterPrefix.Trim();
                            }
                        }
                    }
                    
                    // Process structured JSON data if available
                    if (request.Comments.Contains("ADUSER:"))
                    {
                        // This would be more complex extraction of serialized ADUserDetails
                        // Advanced implementation would parse the JSON and extract the PreviousRequestItem property
                        // For simplicity, we'll rely on the patterns above for now
                    }
                }
                
                if (!string.IsNullOrEmpty(previousRequestId))
                {
                    _logger.LogMessage($"Marking previous dispose request {previousRequestId} as reinstated");
                    // Call our new method to mark the dispose records
                    bool result = await _backgroundRequestService.MarkDisposeRecordsAsReinstatedAsync(
                        previousRequestId,
                        request.SAMAccount ?? string.Empty,
                        cancellationToken);
                    
                    if (!result)
                    {
                        _logger.LogWarn($"Failed to mark previous dispose request {previousRequestId} as reinstated", null);
                    }
                }
                else
                {
                    _logger.LogWarn($"No previous request ID found in comments for reinstate request {request.Request}", null);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing reinstate completion for request {request.Request}", ex);
            }
        }
        
        private async Task<(bool isFinished, int statusCode, string statusComments)> CheckExternalDatabaseForStatusAsync(Domain.Entities.ToolsRequest request)
        {
            try
            {
                // Query the RequestLog table in the external database
                var requestLog = await _requestLogRepository.GetByRequestAsync(request.Request);
                
                if (requestLog == null)
                {
                    return (false, 0, string.Empty);
                }
                
                // Status codes 1 and 2 are considered "Processing"
                // All other status codes indicate the request is finished
                bool isFinished = requestLog.RequestStatusCodeFK != 1 && requestLog.RequestStatusCodeFK != 2;
                
                if (isFinished)
                {
                    return (true, Convert.ToInt32(requestLog.RequestStatusCodeFK), requestLog.RequestStatusComments ?? string.Empty);
                }
                else
                {
                    return (false, Convert.ToInt32(requestLog.RequestStatusCodeFK), string.Empty);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error checking external database for request {request.Request}", ex);
                return (false, 0, string.Empty);
            }
        }
    }
}
