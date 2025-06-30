using System;
using System.Text.Json;
using System.Threading.Tasks;
using Blazor.RCL.Application.Interfaces;
using Blazor.RCL.Domain.Entities;
using Blazor.RCL.Automation.AutomationTasks;
using Blazor.RCL.NLog.LogService.Interface;
using Blazor.RCL.Automation.AutomationRequest;
using Blazor.RCL.Automation.AutomationRequest.Interfaces;
using Blazor.RCL.Application.Common.Configuration.Interfaces;

namespace Blazor.RCL.Infrastructure.Services
{
    /// <summary>
    /// Service for handling ToolsRequest database operations.
    /// </summary>
    public class ToolsRequestService : IToolsRequestService
    {
        private readonly IToolsRequestRepository _toolsRequestRepository;
        private readonly ILogHelper _logger;
        private readonly IAppConfiguration _appConfiguration;

        /// <summary>
        /// Initializes a new instance of the <see cref="ToolsRequestService"/> class.
        /// </summary>
        /// <param name="toolsRequestRepository">The tools request repository.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="appConfiguration">The application configuration.</param>
        public ToolsRequestService(IToolsRequestRepository toolsRequestRepository, ILogHelper logger, IAppConfiguration appConfiguration)
        {
            _toolsRequestRepository = toolsRequestRepository ?? throw new ArgumentNullException(nameof(toolsRequestRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _appConfiguration = appConfiguration ?? throw new ArgumentNullException(nameof(appConfiguration));
        }

        /// <inheritdoc/>
        public async Task<ToolsRequest> RecordRequestAsync<T>(
            RequestModel<T> request, 
            string responseContent, 
            bool isSuccess, 
            string username, 
            string applicationName) where T : class, IRequestPayloadModel
        {
            try
            {
                var toolsRequest = new ToolsRequest
                {
                    Source = request.Source,
                    SourceId = Guid.Parse(request.SourceId),
                    Request = request.Request,
                    RequestItem = request.RequestItem,
                    CatalogItem = request.CatalogItem,
                    AccessType = request.AccessType,
                    AccessSubType = request.AccessSubType,
                    ItemData = JsonSerializer.Serialize(request.ItemData),
                    BatchId = request.BatchId,
                    RequestUsername = username,
                    RequestApplication = applicationName,
                    ResponseJSON = responseContent,
                    RequestDisableState = request.AccessSubType == "Dispose", // Set based on operation type
                    Comments = string.IsNullOrEmpty(request.Comments) ? "" : request.Comments,
                    
                    // Add the new properties
                    EmployeeID = request.EmployeeID,
                    SAMAccount = request.SAMAccount,
                    Domain = request.Domain,
                    
                    // Set status based on response
                    RequestStatusCodeFK = isSuccess ? 2 : 7, // 2=In-Progress, 7=Failed
                    RequestStatusDate = DateTime.UtcNow,
                    RequestStatusComments = isSuccess ? 
                        "Request submitted successfully" : 
                        "Request failed during submission"
                };
                
                return await _toolsRequestRepository.CreateAsync(toolsRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error recording request in database", ex);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateRequestStatusAsync(int toolsRequestPK, int statusCodeFK, string statusComments)
        {
            try
            {
                return await _toolsRequestRepository.UpdateStatusAsync(toolsRequestPK, statusCodeFK, statusComments);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating request status for ID {toolsRequestPK}", ex);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<(int total, int completed, bool isComplete)> CheckRequestItemCompletionAsync(string requestItem)
        {
            try
            {
                return await _toolsRequestRepository.GetRequestItemCompletionStatusAsync(requestItem);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error checking completion status for RequestItem {requestItem}", ex);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<int> RecordInitialRequestAsync<T>(
            RequestModel<T> request,
            string username,
            string applicationName) where T : class, IRequestPayloadModel
        {
            try
            {
                var toolsRequest = new ToolsRequest
                {
                    Source = request.Source,
                    SourceId = Guid.Parse(request.SourceId),
                    Request = request.Request,
                    RequestItem = request.RequestItem,
                    CatalogItem = request.CatalogItem,
                    AccessType = request.AccessType,
                    AccessSubType = request.AccessSubType,
                    ItemData = JsonSerializer.Serialize(request.ItemData),
                    BatchId = request.BatchId,
                    RequestUsername = username,
                    RequestApplication = applicationName,
                    ResponseJSON = "", // Empty initially
                    RequestDisableState = request.AccessSubType == "Dispose", // Set based on operation type
                    Comments = string.IsNullOrEmpty(request.Comments) ? "" : request.Comments,
                    
                    // Add the new properties
                    EmployeeID = request.EmployeeID,
                    SAMAccount = request.SAMAccount,
                    Domain = request.Domain,
                    
                    // Set initial status
                    RequestStatusCodeFK = 1, // 1=Pending
                    RequestStatusDate = DateTime.UtcNow,
                    RequestStatusComments = "Request pending submission"
                };
                
                // Create the record and return its ID
                var createdRequest = await _toolsRequestRepository.CreateAsync(toolsRequest);
                return createdRequest.ToolsRequestPK;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error recording initial request in database", ex);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateRequestWithResponseAsync(
            int requestId,
            string responseContent,
            bool isSuccess,
            string username)
        {
            try
            {
                // Get the status code and message based on success
                int statusCodeFK = isSuccess ? 2 : 7; // 2=In-Progress, 7=Failed
                string statusComments = isSuccess ? 
                    "Request submitted successfully" : 
                    "Request failed during submission";
                
                // Use the specialized method to update only response-related fields
                return await _toolsRequestRepository.UpdateRequestResponseAsync(
                    requestId,
                    responseContent,
                    statusCodeFK,
                    statusComments,
                    username);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating request {requestId} with response information", ex);
                return false; // Don't rethrow to avoid failing the API call
            }
        }
    }
}
