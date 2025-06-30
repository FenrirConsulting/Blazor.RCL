using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Text.Json;
using System.Text.Json.Serialization;
using Blazor.RCL.NLog.LogService.Interface;

namespace Blazor.RCL.UIComponents.Components.JSON
{
    /// <summary>
    /// Reusable component for viewing response data in a structured format.
    /// Displays request status, details, and task information.
    /// </summary>
    public partial class ResponseDataViewer : ComponentBase
    {
        #region Parameters and Injected Services
        [CascadingParameter] IMudDialogInstance MudDialog { get; set; } = default!;
        [Inject] private ILogHelper Logger { get; set; } = default!;

        /// <summary>
        /// The original JSON response data to display.
        /// </summary>
        [Parameter] public string JsonContent { get; set; } = string.Empty;
        
        /// <summary>
        /// Optional title for the dialog. If not provided, "Response Data" will be used.
        /// </summary>
        [Parameter] public string Title { get; set; } = string.Empty;
        #endregion

        #region Private Fields
        private bool IsOriginalJsonVisible { get; set; } = false;
        private string OriginalJson => JsonContent;
        private string SourceId { get; set; } = string.Empty;
        private string RequestItem { get; set; } = string.Empty;
        private string AccessType { get; set; } = string.Empty;
        private string AccessSubType { get; set; } = "Request";
        private int RequestStatusCode { get; set; }
        private string RequestStatusDesc { get; set; } = string.Empty;
        private string StatusComments { get; set; } = string.Empty;
        private List<TaskDetailInfo> TaskDetails { get; set; } = new();
        private string AutomationId { get; set; } = string.Empty;
        #endregion

        #region Lifecycle Methods
        /// <summary>
        /// Initialize component and parse the JSON data.
        /// </summary>
        protected override void OnInitialized()
        {
            try
            {
                if (!string.IsNullOrEmpty(JsonContent))
                {
                    ParseResponseData();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Error parsing response data JSON", ex);
            }
        }
        #endregion

        #region UI Event Handlers
        /// <summary>
        /// Close the dialog.
        /// </summary>
        private void Close() => MudDialog.Close();
        #endregion

        #region Helper Methods
        /// <summary>
        /// Get the appropriate color for the request status.
        /// </summary>
        private Color GetStatusColor()
        {
            return RequestStatusCode switch
            {
                200 => Color.Success, // OK
                202 => Color.Success, // Accepted
                3 => Color.Success,   // Completed (older format)
                4 => Color.Error,     // Incomplete (older format)
                5 => Color.Error,     // Failed Validating Schema (older format)
                6 => Color.Error,     // Failed Validating Data (older format)
                7 => Color.Error,     // Failed (older format)
                _ => Color.Default,
            };
        }

        /// <summary>
        /// Get the appropriate color for task status.
        /// </summary>
        private Color GetTaskStatusColor(string status)
        {
            if (string.IsNullOrEmpty(status))
                return Color.Default;

            return status.ToLower() switch
            {
                "success" => Color.Success,
                "failed" => Color.Error,
                "warning" => Color.Warning,
                "incomplete" => Color.Warning,
                "in progress" => Color.Info,
                _ => Color.Default,
            };
        }

        /// <summary>
        /// Parse the JSON response data into the structured model.
        /// </summary>
        private void ParseResponseData()
        {
            try
            {
                // Detect and handle the outer "Message" wrapping that some responses have
                string jsonToProcess = JsonContent;
                
                // Check if the response is wrapped in a "Message" property
                if (JsonContent.Contains("\"Message\":") && JsonContent.StartsWith("{") && JsonContent.EndsWith("}"))
                {
                    try
                    {
                        var messageWrapper = JsonSerializer.Deserialize<MessageWrapper>(JsonContent);
                        if (!string.IsNullOrEmpty(messageWrapper?.Message))
                        {
                            // Use the inner message as the JSON to process
                            jsonToProcess = messageWrapper.Message;
                            
                            // Convert escaped characters (like \r\n) to their actual values
                            if (jsonToProcess.Contains("\\r\\n"))
                            {
                                jsonToProcess = jsonToProcess.Replace("\\r\\n", "\r\n");
                            }
                            
                            // Remove any extra escaping of quotes
                            if (jsonToProcess.Contains("\\\""))
                            {
                                jsonToProcess = jsonToProcess.Replace("\\\"", "\"");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError("Error extracting message from wrapper", ex);
                        // Continue with original JSON if extraction fails
                    }
                }

                // Now parse the actual response data
                using var document = JsonDocument.Parse(jsonToProcess);
                var root = document.RootElement;

                // Extract basic properties with case-insensitive property name handling
                ExtractStringProperty(root, new[] { "sourceId", "SourceId" }, value => SourceId = value);
                ExtractStringProperty(root, new[] { "requestItem", "RequestItem" }, value => RequestItem = value);
                ExtractStringProperty(root, new[] { "accessType", "AccessType" }, value => AccessType = value);
                ExtractStringProperty(root, new[] { "accessSubtype", "AccessSubtype" }, value => AccessSubType = value);
                
                // Extract status code with both naming conventions
                if (TryGetProperty(root, new[] { "requestStatusCode", "RequestStatusCode" }, out var statusCodeElement)
                    && statusCodeElement.TryGetInt32(out int statusCode))
                {
                    RequestStatusCode = statusCode;
                }

                ExtractStringProperty(root, new[] { "requestStatusDesc", "RequestStatusDesc" }, value => RequestStatusDesc = value);
                ExtractStringProperty(root, new[] { "statusComments", "StatusComments" }, value => StatusComments = value);
                ExtractStringProperty(root, new[] { "AutomationId", "Automationid" }, value => AutomationId = value);

                // Extract task details
                if (TryGetProperty(root, new[] { "taskDetails", "TaskDetails" }, out var taskDetailsElement))
                {
                    ParseTaskDetails(taskDetailsElement);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Error parsing response data", ex);
            }
        }

        /// <summary>
        /// Helper method to try to get a property using any of the provided names.
        /// </summary>
        private bool TryGetProperty(JsonElement element, string[] propertyNames, out JsonElement result)
        {
            foreach (var name in propertyNames)
            {
                if (element.TryGetProperty(name, out result))
                {
                    return true;
                }
            }
            
            result = default;
            return false;
        }

        /// <summary>
        /// Helper method to extract string property with multiple possible property names.
        /// </summary>
        private void ExtractStringProperty(JsonElement element, string[] propertyNames, Action<string> setter)
        {
            if (TryGetProperty(element, propertyNames, out var propElement))
            {
                setter(propElement.GetString() ?? string.Empty);
            }
        }

        /// <summary>
        /// Parse task details from the response.
        /// </summary>
        private void ParseTaskDetails(JsonElement taskDetailsArray)
        {
            try
            {
                foreach (var task in taskDetailsArray.EnumerateArray())
                {
                    var taskDetail = new TaskDetailInfo();

                    if (task.TryGetProperty("TaskId", out var taskIdElement))
                    {
                        taskDetail.TaskId = taskIdElement.GetInt32();
                    }

                    if (task.TryGetProperty("TaskName", out var taskNameElement))
                    {
                        taskDetail.TaskName = taskNameElement.GetString() ?? string.Empty;
                    }

                    if (task.TryGetProperty("TaskStatus", out var taskStatusElement))
                    {
                        taskDetail.TaskStatus = taskStatusElement.GetString() ?? string.Empty;
                    }

                    if (task.TryGetProperty("ErrorDesc", out var errorDescElement))
                    {
                        taskDetail.ErrorDesc = errorDescElement.GetString() ?? string.Empty;
                    }
                    else if (task.TryGetProperty("TaskStatusComments", out var commentsElement))
                    {
                        taskDetail.ErrorDesc = commentsElement.GetString() ?? string.Empty;
                    }

                    TaskDetails.Add(taskDetail);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Error parsing task details", ex);
            }
        }
        #endregion

        #region Response Data Models
        /// <summary>
        /// Represents task detail information.
        /// </summary>
        private class TaskDetailInfo
        {
            public int TaskId { get; set; }
            public string TaskName { get; set; } = string.Empty;
            public string TaskStatus { get; set; } = string.Empty;
            public string ErrorDesc { get; set; } = string.Empty;
        }

        /// <summary>
        /// Wrapper class for responses that encapsulate the actual data in a Message property.
        /// </summary>
        private class MessageWrapper
        {
            [JsonPropertyName("Message")]
            public string Message { get; set; } = string.Empty;
        }
        #endregion
    }
}
