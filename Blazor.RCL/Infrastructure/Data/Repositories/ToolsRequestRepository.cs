using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using Blazor.RCL.Application.Interfaces;
using Blazor.RCL.Domain.Entities;
using Blazor.RCL.NLog.LogService.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging; // Added the missing using statement

namespace Blazor.RCL.Infrastructure.Data.Repositories
{
    /// <summary>
    /// Repository implementation for ToolsRequest operations.
    /// </summary>
    public class ToolsRequestRepository : IToolsRequestRepository
    {
        #region Fields

        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly ILogHelper _logger;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the ToolsRequestRepository class.
        /// </summary>
        /// <param name="contextFactory">The factory for creating database contexts.</param>
        /// <param name="logger">The logger for error logging.</param>
        public ToolsRequestRepository(IDbContextFactory<AppDbContext> contextFactory, ILogHelper logger)
        {
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #endregion

        #region Public Methods

        #region Query/Retrieval Methods

        /// <inheritdoc/>
        public async Task<ToolsRequest> GetByIdAsync(int toolsRequestPK, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                
                // Use a more direct query approach without the Include to avoid null reference issues
                var toolsRequest = await context.ToolsRequest
                    .AsNoTracking() // Use AsNoTracking for read-only operations
                    .Where(tr => tr.ToolsRequestPK == toolsRequestPK)
                    .Select(tr => new ToolsRequest
                    {
                        ToolsRequestPK = tr.ToolsRequestPK,
                        Source = tr.Source ?? string.Empty,
                        SourceId = tr.SourceId,
                        Request = tr.Request ?? string.Empty,
                        RequestItem = tr.RequestItem ?? string.Empty,
                        CatalogItem = tr.CatalogItem ?? string.Empty,
                        AccessType = tr.AccessType ?? string.Empty,
                        AccessSubType = tr.AccessSubType ?? string.Empty,
                        ItemData = tr.ItemData ?? string.Empty,
                        BatchId = tr.BatchId ?? string.Empty,
                        RequestUsername = tr.RequestUsername ?? string.Empty,
                        RequestApplication = tr.RequestApplication ?? string.Empty,
                        ResponseJSON = tr.ResponseJSON ?? string.Empty,
                        RequestDisableState = tr.RequestDisableState,
                        Comments = tr.Comments ?? string.Empty,
                        RequestStatusCodeFK = tr.RequestStatusCodeFK,
                        RequestStatusDate = tr.RequestStatusDate,
                        RequestStatusComments = tr.RequestStatusComments ?? string.Empty
                    })
                    .FirstOrDefaultAsync(token);
                
                return toolsRequest;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while getting ToolsRequest by ID: {toolsRequestPK}", ex);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<ToolsRequest>> GetByRequestAsync(string request, CancellationToken token = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request))
                {
                    throw new ArgumentException("Request identifier is required.", nameof(request));
                }

                using var context = await _contextFactory.CreateDbContextAsync(token);
                return await context.ToolsRequest
                    .Include(tr => tr.StatusCode)
                    .Where(tr => tr.Request == request)
                    .ToListAsync(token);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while getting ToolsRequest by Request: {request}", ex);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<ToolsRequest>> GetBySourceIdAsync(Guid sourceId, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                return await context.ToolsRequest
                    .Include(tr => tr.StatusCode)
                    .Where(tr => tr.SourceId == sourceId)
                    .ToListAsync(token);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while getting ToolsRequest by SourceId: {sourceId}", ex);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<ToolsRequest>> GetByBatchIdAsync(string batchId, CancellationToken token = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(batchId))
                {
                    throw new ArgumentException("BatchId is required.", nameof(batchId));
                }

                using var context = await _contextFactory.CreateDbContextAsync(token);
                return await context.ToolsRequest
                    .Include(tr => tr.StatusCode)
                    .Where(tr => tr.BatchId == batchId)
                    .ToListAsync(token);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while getting ToolsRequest by BatchId: {batchId}", ex);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<ToolsRequest>> GetByUsernameAsync(string username, CancellationToken token = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(username))
                {
                    throw new ArgumentException("Username is required.", nameof(username));
                }

                using var context = await _contextFactory.CreateDbContextAsync(token);
                return await context.ToolsRequest
                    .Include(tr => tr.StatusCode)
                    .Where(tr => tr.RequestUsername == username)
                    .ToListAsync(token);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while getting ToolsRequest by Username: {username}", ex);
                throw;
            }
        }
        
        /// <inheritdoc/>
        public async Task<string> GetAutomationIDByRequest(string request, CancellationToken token = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request))
                {
                    throw new ArgumentException("Request identifier is required.", nameof(request));
                }

                _logger.LogMessage($"Searching for request: {request}");

                using var context = await _contextFactory.CreateDbContextAsync(token);
                var toolsRequest = await context.ToolsRequest
                    .Where(tr => tr.Request == request)
                    .OrderByDescending(tr => tr.RequestStatusDate) // Get the most recent one if multiple exist
                    .FirstOrDefaultAsync(token);

                if (toolsRequest == null)
                {
                    _logger.LogWarn($"No record found for request {request}", null);
                    return null;
                }

                if (string.IsNullOrWhiteSpace(toolsRequest.ResponseJSON))
                {
                    _logger.LogWarn($"ResponseJSON is empty for request {request}", null);
                    return null;
                }

                _logger.LogMessage($"Found record for request {request} with ResponseJSON length: {toolsRequest.ResponseJSON.Length}");

                // Parse the ResponseJSON to extract the Automationid
                try
                {
                    // First, parse the outer JSON object
                    using JsonDocument outerDoc = JsonDocument.Parse(toolsRequest.ResponseJSON);
                    
                    // Log the properties available in the outer JSON
                    var outerProperties = string.Join(", ", outerDoc.RootElement.EnumerateObject().Select(p => p.Name));
                    _logger.LogMessage($"Outer JSON properties: {outerProperties}");
                    
                    // Try to find Automationid directly in the outer JSON first
                    string[] possiblePropertyNames = { "Automationid", "Automationid", "Automationid", "AutomationID", "AutomationID" };
                    
                    foreach (var propName in possiblePropertyNames)
                    {
                        if (outerDoc.RootElement.TryGetProperty(propName, out JsonElement idElement) && 
                            idElement.ValueKind == JsonValueKind.String)
                        {
                            string AutomationId = idElement.GetString();
                            _logger.LogMessage($"Found {propName} directly in outer JSON: {AutomationId} for request {request}");
                            return AutomationId;
                        }
                    }
                    
                    // Check if there's a property that contains "Automation" in its name in the outer JSON
                    foreach (var prop in outerDoc.RootElement.EnumerateObject())
                    {
                        if (prop.Name.IndexOf("Automation", StringComparison.OrdinalIgnoreCase) >= 0 && 
                            prop.Value.ValueKind == JsonValueKind.String)
                        {
                            string AutomationId = prop.Value.GetString();
                            _logger.LogMessage($"Found property containing 'Automation' in outer JSON: {prop.Name} with value: {AutomationId}");
                            return AutomationId;
                        }
                    }
                    
                    // If not found directly, check if there's a Message property with nested JSON
                    if (outerDoc.RootElement.TryGetProperty("Message", out JsonElement messageElement))
                    {
                        // The Message property contains another JSON string that needs to be parsed
                        string innerJson = messageElement.GetString();
                        if (string.IsNullOrWhiteSpace(innerJson))
                        {
                            _logger.LogWarn($"Inner JSON is empty for request {request}", null);
                            // Continue to check for GUID in outer JSON instead of returning null
                        }
                        else
                        {
                            _logger.LogMessage($"Inner JSON length: {innerJson.Length}");
                            
                            try
                            {
                                // Parse the inner JSON to extract the Automationid
                                using JsonDocument innerDoc = JsonDocument.Parse(innerJson);
                                
                                // Log the properties available in the inner JSON
                                var innerProperties = string.Join(", ", innerDoc.RootElement.EnumerateObject().Select(p => p.Name));
                                _logger.LogMessage($"Inner JSON properties: {innerProperties}");
                                
                                // Try different property name variations for Automationid in inner JSON
                                foreach (var propName in possiblePropertyNames)
                                {
                                    if (innerDoc.RootElement.TryGetProperty(propName, out JsonElement idElement) && 
                                        idElement.ValueKind == JsonValueKind.String)
                                    {
                                        string AutomationId = idElement.GetString();
                                        _logger.LogMessage($"Found {propName} in inner JSON: {AutomationId} for request {request}");
                                        return AutomationId;
                                    }
                                }
                                
                                // Check if there's a property that contains "Automation" in its name in inner JSON
                                foreach (var prop in innerDoc.RootElement.EnumerateObject())
                                {
                                    if (prop.Name.IndexOf("Automation", StringComparison.OrdinalIgnoreCase) >= 0 && 
                                        prop.Value.ValueKind == JsonValueKind.String)
                                    {
                                        string AutomationId = prop.Value.GetString();
                                        _logger.LogMessage($"Found property containing 'Automation' in inner JSON: {prop.Name} with value: {AutomationId}");
                                        return AutomationId;
                                    }
                                }
                                
                                // Check for a GUID-like property in inner JSON if none of the above worked
                                foreach (var prop in innerDoc.RootElement.EnumerateObject())
                                {
                                    if (prop.Value.ValueKind == JsonValueKind.String)
                                    {
                                        string value = prop.Value.GetString();
                                        if (!string.IsNullOrEmpty(value) && 
                                            value.Length > 30 && 
                                            value.Contains("-") && 
                                            Guid.TryParse(value, out _))
                                        {
                                            _logger.LogMessage($"Found GUID-like property in inner JSON: {prop.Name} with value: {value}");
                                            return value;
                                        }
                                    }
                                }
                            }
                            catch (JsonException innerEx)
                            {
                                _logger.LogError($"Error parsing inner JSON for request {request}: {innerEx.Message}", innerEx);
                                // Continue to check for GUID in outer JSON instead of returning null
                            }
                        }
                    }
                    
                    // As a last resort, check for any GUID-like property in the outer JSON
                    foreach (var prop in outerDoc.RootElement.EnumerateObject())
                    {
                        if (prop.Value.ValueKind == JsonValueKind.String)
                        {
                            string value = prop.Value.GetString();
                            if (!string.IsNullOrEmpty(value) && 
                                value.Length > 30 && 
                                value.Contains("-") && 
                                Guid.TryParse(value, out _))
                            {
                                _logger.LogMessage($"Found GUID-like property in outer JSON: {prop.Name} with value: {value}");
                                return value;
                            }
                        }
                    }
                    
                    _logger.LogWarn($"No suitable Automation ID property found in any JSON structure for request {request}", null);
                    return null;
                }
                catch (JsonException ex)
                {
                    _logger.LogError($"Error parsing ResponseJSON for request {request}: {ex.Message}", ex);
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while getting Automationid for request: {request}", ex);
                throw;
            }
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public async Task<(int total, int completed, bool isComplete)> GetRequestItemCompletionStatusAsync(string requestItem, CancellationToken token = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(requestItem))
                {
                    throw new ArgumentException("RequestItem is required.", nameof(requestItem));
                }

                using var context = await _contextFactory.CreateDbContextAsync(token);
                var requests = await context.ToolsRequest
                    .Where(tr => tr.RequestItem == requestItem)
                    .Select(tr => tr.RequestStatusCodeFK)
                    .ToListAsync(token);

                int total = requests.Count;
                
                // Status codes considered "finished"
                var finishedStatuses = new List<int> { 3, 4, 7, 8, 9 };
                int completed = requests.Count(status => status.HasValue && finishedStatuses.Contains(status.Value));
                
                return (total, completed, total > 0 && completed == total);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while checking completion status for RequestItem: {requestItem}", ex);
                throw;
            }
        }
        
        /// <inheritdoc/>
        public async Task<IEnumerable<ToolsRequest>> GetToolsRequestsForReinstateAsync(
            string criteria, 
            string searchValue,
            string domain = null,
            CancellationToken token = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(criteria))
                {
                    throw new ArgumentException("Search criteria cannot be null or empty.", nameof(criteria));
                }
                
                if (string.IsNullOrWhiteSpace(searchValue))
                {
                    throw new ArgumentException("Search value cannot be null or empty.", nameof(searchValue));
                }
                
                using var context = await _contextFactory.CreateDbContextAsync(token);
                
                // Start with base query for dispose operations
                var query = context.ToolsRequest
                    .AsNoTracking()
                    .Where(tr => tr.AccessSubType == "Dispose" && tr.RequestDisableState == true);
                
                // Add filter to exclude pending (1) and in-progress (2) requests
                // Only return requests with status codes indicating completion or failure
                query = query.Where(tr => tr.RequestStatusCodeFK != 1 && tr.RequestStatusCodeFK != 2);
                
                // Apply search criteria filter
                if (criteria.Equals("SAMAccount", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(tr => tr.SAMAccount == searchValue);
                    
                    // Add domain filter if provided for SAM account lookups
                    if (!string.IsNullOrWhiteSpace(domain))
                    {
                        query = query.Where(tr => tr.Domain == domain);
                    }
                }
                else if (criteria.Equals("EmployeeID", StringComparison.OrdinalIgnoreCase))
                {
                    query = query.Where(tr => tr.EmployeeID == searchValue);
                }
                else
                {
                    throw new ArgumentException($"Invalid search criteria: {criteria}. Must be 'SAMAccount' or 'EmployeeID'.", nameof(criteria));
                }
                
                // Order by request status date descending to get the most recent first
                return await query
                    .OrderByDescending(tr => tr.RequestStatusDate)
                    .ToListAsync(token);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving disposed accounts for reinstatement. Criteria: {criteria}, Value: {searchValue}", ex);
                throw;
            }
        }

        #endregion

        #region Create Methods

        /// <inheritdoc/>
        public async Task<ToolsRequest> CreateAsync(ToolsRequest toolsRequest, CancellationToken token = default)
        {
            try
            {
                if (toolsRequest == null)
                {
                    throw new ArgumentNullException(nameof(toolsRequest));
                }

                using var context = await _contextFactory.CreateDbContextAsync(token);
                context.ToolsRequest.Add(toolsRequest);
                await context.SaveChangesAsync(token);
                return toolsRequest;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurred while creating ToolsRequest", ex);
                throw;
            }
        }

        #endregion

        #region Update Methods

        /// <inheritdoc/>
        public async Task<ToolsRequest> UpdateAsync(ToolsRequest toolsRequest, CancellationToken token = default)
        {
            try
            {
                if (toolsRequest == null)
                {
                    throw new ArgumentNullException(nameof(toolsRequest));
                }

                using var context = await _contextFactory.CreateDbContextAsync(token);
                context.Entry(toolsRequest).State = EntityState.Modified;
                await context.SaveChangesAsync(token);
                return toolsRequest;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while updating ToolsRequest: {toolsRequest.ToolsRequestPK}", ex);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateStatusAsync(int toolsRequestPK, int statusCodeFK, string statusComments, CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                
                // Use a more direct and explicit query approach to avoid null reference issues
                var toolsRequest = await context.ToolsRequest
                    .Where(tr => tr.ToolsRequestPK == toolsRequestPK)
                    .FirstOrDefaultAsync(token);
                
                if (toolsRequest == null)
                {
                    _logger.LogError($"ToolsRequest with ID {toolsRequestPK} not found for status update", null);
                    return false;
                }

                toolsRequest.RequestStatusCodeFK = statusCodeFK;
                toolsRequest.RequestStatusComments = statusComments ?? string.Empty; // Ensure comments is never null
                toolsRequest.RequestStatusDate = DateTime.UtcNow;
                
                // Use a try-catch block specifically for the SaveChanges operation
                try
                {
                    await context.SaveChangesAsync(token);
                    return true;
                }
                catch (DbUpdateException dbEx)
                {
                    _logger.LogError($"Database error updating status for ToolsRequest: {toolsRequestPK}", dbEx);
                    
                    // If there's a specific SQL error that needs handling, do it here
                    if (dbEx.InnerException is SqlException sqlEx)
                    {
                        _logger.LogError($"SQL error code: {sqlEx.Number} for ToolsRequest: {toolsRequestPK}", null);
                    }
                    
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while updating status for ToolsRequest: {toolsRequestPK}", ex);
                throw;
            }
        }

        /// <summary>
        /// Updates only response-related fields of a ToolsRequest entity without altering other fields.
        /// </summary>
        /// <param name="requestId">The ID of the ToolsRequest to update.</param>
        /// <param name="responseJson">The JSON response from the API.</param>
        /// <param name="statusCodeFK">The status code foreign key.</param>
        /// <param name="statusComments">The status comments.</param>
        /// <param name="username">The username of the user updating the request.</param>
        /// <param name="token">Cancellation token.</param>
        /// <returns>True if update was successful, false otherwise.</returns>
        public async Task<bool> UpdateRequestResponseAsync(
            int requestId,
            string responseJson,
            int statusCodeFK,
            string statusComments,
            string username,
            CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                
                // Use a more direct and explicit query approach to avoid null reference issues
                var toolsRequest = await context.ToolsRequest
                    .Where(tr => tr.ToolsRequestPK == requestId)
                    .FirstOrDefaultAsync(token);
                
                if (toolsRequest == null)
                {
                    _logger.LogError($"ToolsRequest with ID {requestId} not found", null);
                    return false;
                }
                
                // Log values before update
                _logger.LogMessage($"Before response update: EmployeeID = {toolsRequest.EmployeeID}, SAMAccount = {toolsRequest.SAMAccount}, Domain = {toolsRequest.Domain}");
                
                // Update only the specific fields we want to change
                toolsRequest.ResponseJSON = responseJson;
                toolsRequest.RequestStatusCodeFK = statusCodeFK;
                toolsRequest.RequestStatusDate = DateTime.UtcNow;
                toolsRequest.RequestStatusComments = statusComments;
                toolsRequest.RequestUsername = username;
                
                // Save changes
                await context.SaveChangesAsync(token);
                
                // Log values after update to verify they were preserved
                _logger.LogMessage($"After response update: EmployeeID = {toolsRequest.EmployeeID}, SAMAccount = {toolsRequest.SAMAccount}, Domain = {toolsRequest.Domain}");
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while updating response for ToolsRequest: {requestId}", ex);
                return false;
            }
        }
        
        /// <inheritdoc/>
        public async Task<bool> UpdateResponseJsonAsync(
            int toolsRequestPK,
            string responseJson,
            CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                
                // Use a more direct and explicit query approach to avoid null reference issues
                var toolsRequest = await context.ToolsRequest
                    .Where(tr => tr.ToolsRequestPK == toolsRequestPK)
                    .FirstOrDefaultAsync(token);
                
                if (toolsRequest == null)
                {
                    _logger.LogError($"ToolsRequest with ID {toolsRequestPK} not found for ResponseJSON update", null);
                    return false;
                }
                
                // Update only the ResponseJSON field
                toolsRequest.ResponseJSON = responseJson;
                
                // Save changes
                await context.SaveChangesAsync(token);
                
                _logger.LogMessage($"Successfully updated ResponseJSON for ToolsRequest: {toolsRequestPK}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error occurred while updating ResponseJSON for ToolsRequest: {toolsRequestPK}", ex);
                return false;
            }
        }

        #endregion

        #region Utility Methods

        /// <inheritdoc/>
        public async Task<string> GenerateRequestNumberAsync(CancellationToken token = default)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync(token);
                
                // Use a more robust query approach that handles null values
                var lastRequestNumber = await context.ToolsRequest
                    .AsNoTracking()
                    .Where(r => r.Request != null && r.Request.StartsWith("TOOLS"))
                    .Select(r => new { RequestNumber = r.Request })
                    .OrderByDescending(r => r.RequestNumber)
                    .FirstOrDefaultAsync(token);
                
                int nextNumber = 1;
                
                if (lastRequestNumber != null && !string.IsNullOrEmpty(lastRequestNumber.RequestNumber))
                {
                    // Safe substring operation with null check
                    string numberPart = lastRequestNumber.RequestNumber.Length > 5 
                        ? lastRequestNumber.RequestNumber.Substring(5) // Remove "TOOLS" prefix
                        : "0";
                        
                    if (int.TryParse(numberPart, out int currentNumber))
                    {
                        nextNumber = currentNumber + 1;
                    }
                }
                else
                {
                    // Fallback if no existing request numbers found
                    nextNumber = 1;
                }
                
                return $"TOOLS{nextNumber:D6}";
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurred while generating request number", ex);
                throw;
            }
        }

        #endregion

        #endregion
    }
}