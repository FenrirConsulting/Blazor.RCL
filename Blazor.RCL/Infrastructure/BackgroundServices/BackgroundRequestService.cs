using Blazor.RCL.Application.Interfaces;
using Blazor.RCL.Domain.Entities;
using Blazor.RCL.NLog.LogService.Interface;
using Blazor.RCL.Automation.AutomationDirectory.AutomationDirectoryRepositories.Interfaces;
using Blazor.RCL.Automation.Services;
using Blazor.RCL.Infrastructure.Services.Interfaces;
using Company.Identity.PAM.Akeyless.CredentialManager;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Blazor.RCL.Domain.Entities.AzureAD;
using Blazor.RCL.Infrastructure.Services;

namespace Blazor.RCL.Infrastructure.BackgroundServices
{
    /// <summary>
    /// Service for background operations on ToolsRequest entities.
    /// This is a singleton service designed specifically for the background worker.
    /// </summary>
    public class BackgroundRequestService
    {
        private readonly IDbContextFactory<Data.AppDbContext> _contextFactory;
        private readonly ILogHelper _logger;
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initializes a new instance of the BackgroundRequestService class.
        /// </summary>
        /// <param name="contextFactory">The factory for creating database contexts.</param>
        /// <param name="logger">The logger for error logging.</param>
        /// <param name="serviceProvider">The service provider for resolving scoped services.</param>
        public BackgroundRequestService(
            IDbContextFactory<Data.AppDbContext> contextFactory,
            ILogHelper logger,
            IServiceProvider serviceProvider)
        {
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        /// <summary>
        /// Gets all ToolsRequest with a specific status code.
        /// </summary>
        /// <param name="statusCodeFK">The status code to filter by.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A list of ToolsRequest with the specified status.</returns>
        public async Task<List<ToolsRequest>> GetRequestsByStatusAsync(int statusCodeFK, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                
                return await context.ToolsRequest
                    .AsNoTracking()
                    .Include(tr => tr.StatusCode)
                    .Where(tr => tr.RequestStatusCodeFK == statusCodeFK)
                    .ToListAsync(token);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while getting ToolsRequest by status code: {statusCodeFK}", ex);
                throw;
            }
        }

        /// <summary>
        /// Updates the status of a ToolsRequest and also updates the ResponseJSON with the latest API response.
        /// </summary>
        /// <param name="toolsRequestPK">The primary key of the request.</param>
        /// <param name="statusCodeFK">The new status code.</param>
        /// <param name="statusComments">Comments about the status change.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>True if successful; otherwise, false.</returns>
        public async Task<bool> UpdateStatusAsync(int toolsRequestPK, int statusCodeFK, string statusComments, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                
                var toolsRequest = await context.ToolsRequest
                    .Where(tr => tr.ToolsRequestPK == toolsRequestPK)
                    .FirstOrDefaultAsync(token);
                
                if (toolsRequest == null)
                {
                    _logger.LogError($"ToolsRequest with ID {toolsRequestPK} not found for status update", null);
                    return false;
                }

                toolsRequest.RequestStatusCodeFK = statusCodeFK;
                toolsRequest.RequestStatusComments = statusComments ?? string.Empty;
                toolsRequest.RequestStatusDate = DateTime.UtcNow;
                
                // Save the status update first
                await context.SaveChangesAsync(token);
                
                // Then try to update the ResponseJSON with the latest API response
                // This is done separately so that even if the API call fails, the status update will still be saved
                try
                {
                    // Get the Automation ID from the record
                    string AutomationId = await GetAutomationIDFromRequestAsync(toolsRequest.Request, token);
                    
                    if (!string.IsNullOrEmpty(AutomationId))
                    {
                        // Update the ResponseJSON with the API response
                        await UpdateResponseJsonFromApiAsync(toolsRequestPK, AutomationId, token);
                    }
                }
                catch (Exception innerEx)
                {
                    // Log the error but don't fail the status update
                    _logger.LogError($"Error updating ResponseJSON for ToolsRequest {toolsRequestPK}: {innerEx.Message}", innerEx);
                }
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while updating status for ToolsRequest: {toolsRequestPK}", ex);
                throw;
            }
        }

        /// <summary>
        /// Gets the Automation ID from a request record.
        /// </summary>
        /// <param name="requestNumber">The request number.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The Automation ID if found; otherwise, null.</returns>
        private async Task<string> GetAutomationIDFromRequestAsync(string requestNumber, CancellationToken token = default)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var toolsRequestRepository = scope.ServiceProvider.GetRequiredService<IToolsRequestRepository>();
                
                // Use the existing GetAutomationIDByRequest method to extract the Automation ID
                string AutomationId = await toolsRequestRepository.GetAutomationIDByRequest(requestNumber, token);
                
                if (string.IsNullOrEmpty(AutomationId))
                {
                    _logger.LogWarn($"No Automation ID found for request {requestNumber}", null);
                }
                else
                {
                    _logger.LogMessage($"Found Automation ID {AutomationId} for request {requestNumber}");
                }
                
                return AutomationId;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting Automation ID for request {requestNumber}", ex);
                return null;
            }
        }
        
        /// <summary>
        /// Updates the ResponseJSON field of a ToolsRequest with the response from the API.
        /// </summary>
        /// <param name="toolsRequestPK">The primary key of the ToolsRequest to update.</param>
        /// <param name="AutomationId">The Automation ID to use for the API call.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>True if successful; otherwise, false.</returns>
        private async Task<bool> UpdateResponseJsonFromApiAsync(int toolsRequestPK, string AutomationId, CancellationToken token = default)
        {
            try
            {
                // Create a scope to resolve scoped services
                using var scope = _serviceProvider.CreateScope();
                
                // Get the required services
                var apiServices = scope.ServiceProvider.GetRequiredService<APIServices>();
                var azureAdOptions = scope.ServiceProvider.GetRequiredService<IAzureAdOptions>();
                var _akeylessManager = scope.ServiceProvider.GetRequiredService<IAKeylessManager>();
                var toolsRequestRepository = scope.ServiceProvider.GetRequiredService<IToolsRequestRepository>();
                
                // Step 1: Get the access token
                try
                {
                    // Use the new GetAzureCredential method signature
                    var accessKey = await _akeylessManager.GetAzureCredentialAsync();

                    // Use the same API scope value as in the EnableDisableOrchestrationService
                    string apiScope = "AR-Identity-ART_AutomationRequestAPI";
                    
                    // Get the access token
                    var authResult = await apiServices.GetAccessToken(
                        azureAdOptions.ClientId,
                        accessKey,
                        azureAdOptions.TenantId,
                        apiScope);
                    string accessToken = authResult.AccessToken;
                    
                    if (string.IsNullOrEmpty(accessToken))
                    {
                        _logger.LogError($"Failed to obtain access token for Automation ID {AutomationId}", null);
                        return false;
                    }
                    
                    // Step 2: Call the GetRequestByGuid method to get the response from the API
                    var response = await apiServices.GetRequestByGuid(accessToken, AutomationId);
                    
                    if (response != null)
                    {
                        // Step 3: Serialize the response to JSON
                        string responseJson = JsonSerializer.Serialize(response);
                        _logger.LogMessage($"Retrieved API response for Automation ID {AutomationId}");
                        
                        // Step 4: Update the ToolsRequest record with the API response
                        bool updateResult = await toolsRequestRepository.UpdateResponseJsonAsync(
                            toolsRequestPK,
                            responseJson,
                            token);
                            
                        if (updateResult)
                        {
                            _logger.LogMessage($"Successfully updated ResponseJSON for ToolsRequest {toolsRequestPK}");
                            return true;
                        }
                        else
                        {
                            _logger.LogWarn($"Failed to update ResponseJSON for ToolsRequest {toolsRequestPK}", null);
                            return false;
                        }
                    }
                    else
                    {
                        _logger.LogWarn($"No response received from API for Automation ID {AutomationId}", null);
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error calling API for Automation ID {AutomationId}", ex);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating ResponseJSON for ToolsRequest {toolsRequestPK}", ex);
                return false;
            }
        }
        
        /// <summary>
        /// Marks previous dispose records as no longer eligible for reinstatement.
        /// </summary>
        /// <param name="previousRequestId">The ID of the previous request.</param>
        /// <param name="samAccountName">The SAM account name for the user.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>True if successful; otherwise, false.</returns>
        public async Task<bool> MarkDisposeRecordsAsReinstatedAsync(string previousRequestId, string samAccountName, CancellationToken token = default)
        {
            if (string.IsNullOrEmpty(previousRequestId) || string.IsNullOrEmpty(samAccountName))
            {
                _logger.LogError("Cannot mark dispose records as reinstated: previousRequestId or samAccountName is null or empty", null);
                return false;
            }

            try
            {
                if (previousRequestId.StartsWith("EXT-BATCH-"))
                {
                    // External database case - extract the batch ID without the prefix
                    string batchId = previousRequestId.Substring(10); // Remove "EXT-BATCH-"
                    
                    try
                    {
                        // Create a scope to resolve the scoped repository
                        using (var scope = _serviceProvider.CreateScope())
                        {
                            var adADRequestRepository = scope.ServiceProvider.GetRequiredService<IADADRequestRepository>();
                            
                            // Call the external database repository to mark records as reinstated
                            int updatedCount = await adADRequestRepository.MarkRecordsAsReinstatedAsync(batchId);
                            
                            if (updatedCount > 0)
                            {
                                _logger.LogMessage($"Successfully marked {updatedCount} external database records as reinstated for batch {batchId}");
                                return true;
                            }
                            else
                            {
                                _logger.LogWarn($"No external database records were updated for batch {batchId}", null);
                                return false;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Error updating external database records for batch {batchId}", ex);
                        return false;
                    }
                }
                else
                {
                    // Try to parse the previousRequestId - it might be a GUID or other format
                    if (Guid.TryParse(previousRequestId, out Guid requestId))
                    {
                        // Look up the original dispose record by SourceId
                        using var context = await _contextFactory.CreateDbContextAsync(token);
                        
                        var originalRequest = await context.ToolsRequest
                            .FirstOrDefaultAsync(tr => tr.SourceId == requestId, token);

                        if (originalRequest != null)
                        {
                            _logger.LogMessage($"Found original request ID {requestId} for reinstate operation");
                            
                            // Get the SAM account from the original request
                            string originalSamAccount = originalRequest.SAMAccount ?? string.Empty;

                            if (!string.IsNullOrEmpty(originalSamAccount))
                            {
                                // Update all dispose records with the same SAM account to have RequestDisableState = false
                                var disposeRecords = await context.ToolsRequest
                                    .Where(tr => tr.SAMAccount == originalSamAccount && tr.AccessSubType == "Dispose")
                                    .ToListAsync(token);

                                if (disposeRecords.Any())
                                {
                                    foreach (var record in disposeRecords)
                                    {
                                        record.RequestDisableState = false;
                                    }

                                    try
                                    {
                                        int rowsAffected = await context.SaveChangesAsync(token);
                                        _logger.LogMessage($"Successfully marked {rowsAffected} dispose records as reinstated for account {originalSamAccount}");
                                        return true;
                                    }
                                    catch (Exception ex)
                                    {
                                        _logger.LogError($"Error saving changes to database: {ex.Message}", ex);
                                        return false;
                                    }
                                }
                                else
                                {
                                    _logger.LogWarn($"No dispose records found for SAM account {originalSamAccount}", null);
                                }
                            }
                            else
                            {
                                _logger.LogWarn($"Original request with ID {previousRequestId} has no SAM account", null);
                            }
                        }
                        else
                        {
                            _logger.LogWarn($"No original dispose request found with SourceId {previousRequestId}", null);
                        }
                    }
                    else
                    {
                        _logger.LogWarn($"Cannot parse previousRequestId {previousRequestId} as a valid ID", null);
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error marking dispose records as reinstated for previousRequestId: {previousRequestId}", ex);
                return false;
            }
        }
    }
}
