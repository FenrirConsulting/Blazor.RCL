using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Blazor.RCL.Domain.Entities;

namespace Blazor.RCL.Application.Interfaces
{
    /// <summary>
    /// Interface for operations on ToolsRequest entities.
    /// </summary>
    public interface IToolsRequestRepository
    {
        #region Query/Retrieval Methods
        
        /// <summary>
        /// Gets a ToolsRequest by its primary key.
        /// </summary>
        /// <param name="toolsRequestPK">The primary key of the request.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The ToolsRequest if found; otherwise, null.</returns>
        Task<ToolsRequest> GetByIdAsync(int toolsRequestPK, CancellationToken token = default);
        
        /// <summary>
        /// Gets all ToolsRequest for a specific Request identifier.
        /// </summary>
        /// <param name="request">The Request identifier.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A list of ToolsRequest.</returns>
        Task<IEnumerable<ToolsRequest>> GetByRequestAsync(string request, CancellationToken token = default);
        
        /// <summary>
        /// Gets all ToolsRequest for a specific source identifier.
        /// </summary>
        /// <param name="sourceId">The source identifier.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A list of ToolsRequest.</returns>
        Task<IEnumerable<ToolsRequest>> GetBySourceIdAsync(Guid sourceId, CancellationToken token = default);
        
        /// <summary>
        /// Gets all ToolsRequest for a specific batch identifier.
        /// </summary>
        /// <param name="batchId">The batch identifier.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A list of ToolsRequest.</returns>
        Task<IEnumerable<ToolsRequest>> GetByBatchIdAsync(string batchId, CancellationToken token = default);
        
        /// <summary>
        /// Gets all ToolsRequest for a specific username.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A list of ToolsRequest.</returns>
        Task<IEnumerable<ToolsRequest>> GetByUsernameAsync(string username, CancellationToken token = default);
        
        /// <summary>
        /// Gets all ToolsRequest with a specific status code.
        /// </summary>
        /// <param name="statusCodeFK">The status code to filter by.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A list of ToolsRequest with the specified status.</returns>
        Task<List<ToolsRequest>> GetRequestsByStatusAsync(int statusCodeFK, CancellationToken token = default);
        
        /// <summary>
        /// Checks if all ToolsRequest for a specific RequestItem are complete.
        /// </summary>
        /// <param name="requestItem">The RequestItem identifier.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A tuple containing the total count, completed count, and completion status.</returns>
        Task<(int total, int completed, bool isComplete)> GetRequestItemCompletionStatusAsync(string requestItem, CancellationToken token = default);
        
        /// <summary>
        /// Gets ToolsRequest records for reinstatement validation based on search criteria.
        /// </summary>
        /// <param name="criteria">The search criteria type ("SAMAccount" or "EmployeeID").</param>
        /// <param name="searchValue">The value to search for (SAM account or employee ID).</param>
        /// <param name="domain">The domain (optional, used only for SAMAccount criteria).</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A list of ToolsRequest that match the criteria.</returns>
        Task<IEnumerable<ToolsRequest>> GetToolsRequestsForReinstateAsync(
            string criteria, 
            string searchValue,
            string domain = null,
            CancellationToken token = default);
            
        /// <summary>
        /// Extracts the Automationid value from the ResponseJSON of a ToolsRequest.
        /// </summary>
        /// <param name="request">The Request identifier.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The Automationid as a string if found; otherwise, null.</returns>
        Task<string> GetAutomationIDByRequest(string request, CancellationToken token = default);
        
        #endregion
        
        #region Create Methods
        
        /// <summary>
        /// Creates a new ToolsRequest.
        /// </summary>
        /// <param name="toolsRequest">The ToolsRequest to create.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The created ToolsRequest with its assigned primary key.</returns>
        Task<ToolsRequest> CreateAsync(ToolsRequest toolsRequest, CancellationToken token = default);
        
        #endregion
        
        #region Update Methods
        
        /// <summary>
        /// Updates an existing ToolsRequest.
        /// </summary>
        /// <param name="toolsRequest">The ToolsRequest to update.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>The updated ToolsRequest.</returns>
        Task<ToolsRequest> UpdateAsync(ToolsRequest toolsRequest, CancellationToken token = default);
        
        /// <summary>
        /// Updates the status of a ToolsRequest.
        /// </summary>
        /// <param name="toolsRequestPK">The primary key of the request.</param>
        /// <param name="statusCodeFK">The new status code.</param>
        /// <param name="statusComments">Comments about the status change.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns>True if successful; otherwise, false.</returns>
        Task<bool> UpdateStatusAsync(int toolsRequestPK, int statusCodeFK, string statusComments, CancellationToken token = default);
        
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
        Task<bool> UpdateRequestResponseAsync(
            int requestId,
            string responseJson,
            int statusCodeFK,
            string statusComments,
            string username,
            CancellationToken token = default);
            
        /// <summary>
        /// Updates only the ResponseJSON field of a ToolsRequest entity.
        /// </summary>
        /// <param name="toolsRequestPK">The primary key of the ToolsRequest to update.</param>
        /// <param name="responseJson">The new JSON response to store.</param>
        /// <param name="token">Cancellation token.</param>
        /// <returns>True if update was successful, false otherwise.</returns>
        Task<bool> UpdateResponseJsonAsync(
            int toolsRequestPK,
            string responseJson,
            CancellationToken token = default);
            
        #endregion
        
        #region Utility Methods
        
        /// <summary>
        /// Generates a new request number.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns>A new request number in the format TOOLSXXXXXX.</returns>
        Task<string> GenerateRequestNumberAsync(CancellationToken token = default);
        
        #endregion
    }
}