using Blazor.RCL.Automation.AutomationRequest;
using Blazor.RCL.NLog.LogService.Interface;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazor.RCL.Automation.Services
{
    /// <summary>
    /// Service for processing Automation API responses.
    /// </summary>
    public class ResponseProcessingService
    {
        #region Fields

        private readonly ILogHelper _logger;
        private readonly ISnackbar _snackbar;
        
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ResponseProcessingService"/> class.
        /// </summary>
        /// <param name="logger">The logging service.</param>
        /// <param name="snackbar">The snackbar notification service.</param>
        public ResponseProcessingService(
            ILogHelper logger,
            ISnackbar snackbar)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _snackbar = snackbar ?? throw new ArgumentNullException(nameof(snackbar));
        }
        
        #endregion

        #region Public Methods

        /// <summary>
        /// Processes a list of Automation responses and displays appropriate notifications.
        /// </summary>
        /// <param name="responses">The list of Automation responses to process.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task ProcessResponsesAsync(List<AutomationResponse> responses)
        {
            if (responses == null)
                throw new ArgumentNullException(nameof(responses));
                
            if (!responses.Any())
            {
                _snackbar.Add("No responses received", Severity.Warning);
                return;
            }
            
            var successCount = responses.Count(r => r.RequestStatusCode == 200);
            var failureCount = responses.Count - successCount;
            
            // Show summary notification
            _snackbar.Add($"Processed {responses.Count} requests: {successCount} successful, {failureCount} failed",
                failureCount == 0 ? Severity.Success : Severity.Warning);
            
            // Process failures with detailed information
            foreach (var failedResponse in responses.Where(r => r.RequestStatusCode != 200))
            {
                await ProcessFailedResponseAsync(failedResponse);
            }
            
            // Note: History functionality removed - will be reworked in future phase
            await Task.CompletedTask; // Just to make the method async
        }
        
        /// <summary>
        /// Processes a single Automation response and displays appropriate notifications.
        /// </summary>
        /// <param name="response">The Automation response to process.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task ProcessResponseAsync(AutomationResponse response)
        {
            if (response == null)
                throw new ArgumentNullException(nameof(response));
            
            bool isSuccess = response.RequestStatusCode == 200;
            
            // Show appropriate notification based on success/failure
            if (isSuccess)
            {
                _snackbar.Add("Request processed successfully", Severity.Success);
            }
            else
            {
                await ProcessFailedResponseAsync(response);
            }
            
            await Task.CompletedTask; // Just to make the method async
        }
        
        /// <summary>
        /// Processes a failed Automation response and displays appropriate notifications.
        /// </summary>
        /// <param name="failedResponse">The failed Automation response to process.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task ProcessFailedResponseAsync(AutomationResponse failedResponse)
        {
            // Determine if this is a database validation error or API error
            bool isDatabaseValidationError = 
                failedResponse.StatusComments?.Contains("database recording") == true ||
                failedResponse.StatusComments?.Contains("required fields") == true;
            
            // Log with appropriate level
            if (isDatabaseValidationError)
            {
                _logger.LogError($"Database validation failed: {failedResponse.StatusComments}", null);
                
                // Show a user-friendly message for database validation errors
                _snackbar.Add(
                    "Unable to process request: Database validation failed. Please check your input data.", 
                    Severity.Warning);
            }
            else
            {
                // For other errors, log the details and show a generic message
                string errorDetails = failedResponse.ErrorDetails != null
                    ? failedResponse.ErrorDetails.ToString() 
                    : failedResponse.StatusComments ?? "Unknown error";
                
                _logger.LogError($"Failed request: {errorDetails}", null);
                
                // Show a more detailed error message if available
                if (!string.IsNullOrEmpty(failedResponse.StatusComments))
                {
                    _snackbar.Add(
                        $"Request failed: {failedResponse.StatusComments}", 
                        Severity.Error);
                }
            }
            
            await Task.CompletedTask; // Just to make the method async
        }
        
        #endregion
    }
}