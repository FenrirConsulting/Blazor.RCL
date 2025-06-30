using System.Threading.Tasks;
using Blazor.RCL.Domain.Entities;
using Blazor.RCL.Automation.AutomationTasks;
using Blazor.RCL.Automation.AutomationRequest;
using Blazor.RCL.Automation.AutomationRequest.Interfaces;

namespace Blazor.RCL.Application.Interfaces
{
    /// <summary>
    /// Interface for handling ToolsRequest database operations.
    /// </summary>
    public interface IToolsRequestService
    {
        /// <summary>
        /// Records a request in the database after API submission.
        /// </summary>
        /// <typeparam name="T">The type of the request payload.</typeparam>
        /// <param name="request">The request model.</param>
        /// <param name="responseContent">The API response content.</param>
        /// <param name="isSuccess">Whether the API call was successful.</param>
        /// <param name="username">The username of the requester.</param>
        /// <param name="applicationName">The name of the application making the request.</param>
        /// <returns>The created ToolsRequest.</returns>
        Task<ToolsRequest> RecordRequestAsync<T>(
            RequestModel<T> request, 
            string responseContent, 
            bool isSuccess, 
            string username, 
            string applicationName) where T : class, IRequestPayloadModel;

        /// <summary>
        /// Updates the status of a request.
        /// </summary>
        /// <param name="toolsRequestPK">The primary key of the request.</param>
        /// <param name="statusCodeFK">The new status code.</param>
        /// <param name="statusComments">Comments about the status change.</param>
        /// <returns>True if successful; otherwise, false.</returns>
        Task<bool> UpdateRequestStatusAsync(int toolsRequestPK, int statusCodeFK, string statusComments);

        /// <summary>
        /// Checks if all requests associated with a RequestItem are complete.
        /// </summary>
        /// <param name="requestItem">The RequestItem identifier.</param>
        /// <returns>A tuple containing the total count, completed count, and completion status.</returns>
        Task<(int total, int completed, bool isComplete)> CheckRequestItemCompletionAsync(string requestItem);

        /// <summary>
        /// Records an initial request in the database before API submission.
        /// </summary>
        /// <typeparam name="T">The type of the request payload.</typeparam>
        /// <param name="request">The request model.</param>
        /// <param name="username">The username of the requester.</param>
        /// <param name="applicationName">The name of the application making the request.</param>
        /// <returns>The ID of the created ToolsRequest.</returns>
        Task<int> RecordInitialRequestAsync<T>(
            RequestModel<T> request,
            string username,
            string applicationName) where T : class, IRequestPayloadModel;

        /// <summary>
        /// Updates an existing request with response information after API submission.
        /// </summary>
        /// <param name="requestId">The ID of the request to update.</param>
        /// <param name="responseContent">The API response content.</param>
        /// <param name="isSuccess">Whether the API call was successful.</param>
        /// <param name="username">The username of the requester.</param>
        /// <returns>True if successful; otherwise, false.</returns>
        Task<bool> UpdateRequestWithResponseAsync(
            int requestId,
            string responseContent,
            bool isSuccess,
            string username);
    }
}
