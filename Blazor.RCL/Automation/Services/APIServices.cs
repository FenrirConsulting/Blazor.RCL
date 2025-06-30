using Blazor.RCL.Application.Common.Configuration.Interfaces;
using Blazor.RCL.Application.Interfaces;
using Blazor.RCL.Infrastructure.Services.Interfaces;
using Blazor.RCL.NLog.LogService.Interface;
using Microsoft.Identity.Client;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Blazor.RCL.Automation.AutomationTasks;
using Blazor.RCL.Automation.AutomationRequest;
using Blazor.RCL.Automation.AutomationRequest.Interfaces;

namespace Blazor.RCL.Automation.Services
{
    /// <summary>
    /// Provides services for making API requests related to enable/disable operations.
    /// </summary>
    public class APIServices
    {
        #region Fields

        private readonly IAppConfiguration _appConfiguration;
        private readonly HttpClient _httpClient;
        private readonly IAzureAdOptions _azureAdOptions;
        private readonly ILogHelper _logger;
        private readonly IToolsRequestService _toolsRequestService;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="APIServices"/> class.
        /// </summary>
        /// <param name="appConfiguration">The application configuration.</param>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="azureAdOptions">The Azure AD options.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="toolsRequestService">The tools request service.</param>
        public APIServices(
            IAppConfiguration appConfiguration, 
            HttpClient httpClient, 
            IAzureAdOptions azureAdOptions, 
            ILogHelper logger,
            IToolsRequestService toolsRequestService)
        {
            _appConfiguration = appConfiguration ?? throw new ArgumentNullException(nameof(appConfiguration));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _azureAdOptions = azureAdOptions ?? throw new ArgumentNullException(nameof(azureAdOptions));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _toolsRequestService = toolsRequestService ?? throw new ArgumentNullException(nameof(toolsRequestService));
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Posts a request message to the Automation API.
        /// </summary>
        /// <param name="token">The authentication token.</param>
        /// <param name="requestModel">The request model containing all request data.</param>
        /// <param name="username">The username of the requester.</param>
        /// <param name="applicationName">The name of the application making the request.</param>
        /// <returns>An <see cref="AutomationResponse"/> object.</returns>
        public async Task<AutomationResponse> PostRequestMessage<T>(
            string token, 
            RequestModel<T> requestModel,
            string username = "", 
            string applicationName = "") where T : class, IRequestPayloadModel
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentException("Token cannot be null or empty.", nameof(token));

            if (requestModel == null)
                throw new ArgumentNullException(nameof(requestModel), "Request model cannot be null.");

            // Generate the request body JSON from the model, excluding the database-only properties
            string requestBodyJson = JsonSerializer.Serialize(requestModel, new JsonSerializerOptions
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            });

            string AutomationRequestURL = _appConfiguration.DynamicValues["RequestAPIURL"];

            // Record the request in the database before making the API call
            int? requestId = null;
            try
            {
                // Use empty strings if username or applicationName not provided
                string effectiveUsername = string.IsNullOrEmpty(username) ? "System" : username;
                string effectiveAppName = string.IsNullOrEmpty(applicationName) 
                    ? _appConfiguration.AppName 
                    : applicationName;

                // Record the initial request in the database (without response)
                requestId = await _toolsRequestService.RecordInitialRequestAsync(
                    requestModel,
                    effectiveUsername,
                    effectiveAppName);
            }
            catch (Exception ex)
            {
                // Log the error and return an error response if database recording fails
                _logger.LogError("Failed to record initial request in database", ex);
                return new AutomationResponse
                {
                    RequestStatusCode = 500,
                    RequestStatusDesc = "Internal Server Error",
                    StatusComments = "Failed to record request in database. API call aborted."
                };
            }

            // Now proceed with the API call
            using var request = new HttpRequestMessage(HttpMethod.Post, AutomationRequestURL);
            request.Content = new StringContent(requestBodyJson, Encoding.UTF8, "application/json");
            request.Headers.Add("Authorization", $"Bearer {token}");

            using var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            var AutomationResponse = new AutomationResponse
            {
                RequestStatusCode = (int)response.StatusCode,
                RequestStatusDesc = response.ReasonPhrase
            };

            if (!string.IsNullOrEmpty(content))
            {
                try
                {
                    var deserializationOptions = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    var deserializedResponse = JsonSerializer.Deserialize<AutomationResponse>(content, deserializationOptions);
                    if (deserializedResponse != null)
                    {
                        AutomationResponse = deserializedResponse;
                        AutomationResponse.RequestStatusCode = (int)response.StatusCode;
                        AutomationResponse.RequestStatusDesc = response.ReasonPhrase;
                    }
                    else
                    {
                        AutomationResponse.StatusComments = "Response was successfully deserialized, but resulted in a null object.";
                    }
                }
                catch (JsonException ex)
                {
                    AutomationResponse.StatusComments = $"Error deserializing response: {ex.Message}";
                    _logger.LogError("Error deserializing API response", ex);
                }
            }

            // Update the database record with the response information
            if (requestId.HasValue)
            {
                try
                {
                    // Use empty strings if username or applicationName not provided
                    string effectiveUsername = string.IsNullOrEmpty(username) ? "System" : username;
                    string effectiveAppName = string.IsNullOrEmpty(applicationName) 
                        ? _appConfiguration.AppName 
                        : applicationName;

                    // Update the request record with response information
                    await _toolsRequestService.UpdateRequestWithResponseAsync(
                        requestId.Value,
                        content,
                        response.IsSuccessStatusCode,
                        effectiveUsername);
                }
                catch (Exception ex)
                {
                    // Log the error but don't fail the API call if database update fails
                    _logger.LogError("Failed to update request with response information", ex);
                }
            }

            if (!response.IsSuccessStatusCode)
            {
                AutomationResponse.StatusComments = $"Non-success status code received: {response.StatusCode}";
                _logger?.LogError($"Non-success status code: {response.StatusCode}. Content: {content}", null);
            }

            return AutomationResponse;
        }

        /// <summary>
        /// Posts AD user details to the Automation API.
        /// </summary>
        /// <param name="token">The authentication token.</param>
        /// <param name="requestBodyJson">The request body in JSON format.</param>
        /// <returns>An <see cref="ADUserDetails"/> object with the response.</returns>
        public async Task<ADUserDetails> PostADUserAccountTaskAPIDetails(string token, string requestBodyJson)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentException("Token cannot be null or empty.", nameof(token));

            if (string.IsNullOrEmpty(requestBodyJson))
                throw new ArgumentException("Request body JSON cannot be null or empty.", nameof(requestBodyJson));

            string AutomationTaskURL = _appConfiguration.DynamicValues["TaskAPIURL"] + "ADUserAccount/UserAccount/Details";

            using var request = new HttpRequestMessage(HttpMethod.Post, AutomationTaskURL);
            request.Content = new StringContent(requestBodyJson, Encoding.UTF8, "application/json");
            request.Headers.Add("Authorization", $"Bearer {token}");

            using var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            var deserializationOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            ADUserDetails? adUserDetails = null;

            if (response.IsSuccessStatusCode && !string.IsNullOrEmpty(content))
            {
                try
                {
                    adUserDetails = JsonSerializer.Deserialize<ADUserDetails>(content, deserializationOptions);
                    if (adUserDetails != null)
                    {
                        adUserDetails.SuccessCode = true;
                    }
                    else
                    {
                        _logger?.LogError("Response was successfully deserialized, but resulted in a null object.", null);
                    }
                }
                catch (JsonException ex)
                {
                    _logger?.LogError($"Error deserializing response: {ex.Message}", ex);
                }
            }

            if (adUserDetails == null)
            {
                adUserDetails = new ADUserDetails
                {
                    SuccessCode = false,
                    ErrorDesc = $"Failed to deserialize response. Status code: {response.StatusCode}"
                };
                _logger?.LogError($"Failed to deserialize response. Status code: {response.StatusCode}. Content: {content}", null);
            }

            return adUserDetails;
        }

        /// <summary>
        /// Gets a request by its GUID from the Automation API.
        /// </summary>
        /// <param name="token">The authentication token.</param>
        /// <param name="guid">The GUID of the request to retrieve.</param>
        /// <returns>An <see cref="AutomationResponse"/> object with the response.</returns>
        public async Task<AutomationResponse> GetRequestByGuid(string token, string guid)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentException("Token cannot be null or empty.", nameof(token));

            if (string.IsNullOrEmpty(guid))
                throw new ArgumentException("GUID cannot be null or empty.", nameof(guid));

            string AutomationRequestURL = _appConfiguration.DynamicValues["RequestAPIURL"] + $"?guid={guid}";

            using var request = new HttpRequestMessage(HttpMethod.Get, AutomationRequestURL);
            request.Headers.Add("Authorization", $"Bearer {token}");

            using var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            var AutomationResponse = new AutomationResponse
            {
                RequestStatusCode = (int)response.StatusCode,
                RequestStatusDesc = response.ReasonPhrase
            };

            if (!string.IsNullOrEmpty(content))
            {
                try
                {
                    var deserializationOptions = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    var deserializedResponse = JsonSerializer.Deserialize<AutomationResponse>(content, deserializationOptions);
                    if (deserializedResponse != null)
                    {
                        AutomationResponse = deserializedResponse;
                        AutomationResponse.RequestStatusCode = (int)response.StatusCode;
                        AutomationResponse.RequestStatusDesc = response.ReasonPhrase;
                    }
                    else
                    {
                        AutomationResponse.StatusComments = "Response was successfully deserialized, but resulted in a null object.";
                    }
                }
                catch (JsonException ex)
                {
                    AutomationResponse.StatusComments = $"Error deserializing response: {ex.Message}";
                    _logger.LogError("Error deserializing API response", ex);
                }
            }

            if (!response.IsSuccessStatusCode)
            {
                AutomationResponse.StatusComments = $"Non-success status code received: {response.StatusCode}";
                _logger?.LogError($"Non-success status code: {response.StatusCode}. Content: {content}", null);
            }

            return AutomationResponse;
        }

        /// <summary>
        /// Gets an access token for authentication.
        /// </summary>
        /// <param name="clientId">The client ID.</param>
        /// <param name="secretValue">The client secret.</param>
        /// <param name="tenantId">The tenant ID.</param>
        /// <returns>An access token string.</returns>
        public async Task<AuthenticationResult> GetAccessToken(string clientId, string secretValue, string tenantId, string apiName)
        {
            if (string.IsNullOrEmpty(clientId))
                throw new ArgumentException("Client ID cannot be null or empty.", nameof(clientId));

            if (string.IsNullOrEmpty(secretValue))
                throw new ArgumentException("Secret value cannot be null or empty.", nameof(secretValue));

            if (string.IsNullOrEmpty(tenantId))
                throw new ArgumentException("Tenant ID cannot be null or empty.", nameof(tenantId));

            var app = ConfidentialClientApplicationBuilder
                .Create(clientId)
                .WithClientSecret(secretValue)  // Once AKeyLess is working, can set to ServicePrincipalAKeylessKey
                .WithAuthority(new Uri($"https://login.microsoftonline.com/{tenantId}"))
                .Build();

            // Extract scopes from ApiPermissions
            string[] scopes = _azureAdOptions!.ApiPermissions
                .Where(ap => ap.ApiName!.Equals(apiName, StringComparison.OrdinalIgnoreCase))
                .SelectMany(apiPermission => apiPermission.Scopes!)
                .ToArray();

            return await app.AcquireTokenForClient(scopes)
                .ExecuteAsync();
        }

        #endregion

        /// <summary>
        /// Safely gets a property value from a dynamic JsonElement object.
        /// </summary>
        /// <param name="jsonObject">The dynamic JSON object.</param>
        /// <param name="propertyName">The name of the property to get.</param>
        /// <returns>The property value or null if not found or not accessible.</returns>
        private string GetPropertySafe(dynamic jsonObject, string propertyName)
        {
            try
            {
                // Handle null check without using == operator
                if (jsonObject is null)
                {
                    return null;
                }

                // Special handling for critical database fields
                bool isCriticalField = propertyName == "Source" || propertyName == "Request" || propertyName == "RequestItem";
                
                // Get lowercase version for case-insensitive matching
                string propertyNameLower = propertyName.ToLowerInvariant();
                
                // Handle JsonElement type specifically
                if (jsonObject is JsonElement jsonElement)
                {
                    // First try exact property name match
                    if (jsonElement.ValueKind == JsonValueKind.Object && 
                        jsonElement.TryGetProperty(propertyName, out JsonElement propElement))
                    {
                        // Handle different value kinds
                        switch (propElement.ValueKind)
                        {
                            case JsonValueKind.String:
                                return propElement.GetString();
                            case JsonValueKind.Number:
                                return propElement.GetRawText();
                            case JsonValueKind.True:
                                return "true";
                            case JsonValueKind.False:
                                return "false";
                            case JsonValueKind.Null:
                                return null;
                            case JsonValueKind.Object:
                            case JsonValueKind.Array:
                                return propElement.GetRawText();
                            default:
                                return null;
                        }
                    }
                    
                    // If exact match fails, try case-insensitive match by enumerating properties
                    if (jsonElement.ValueKind == JsonValueKind.Object)
                    {
                        foreach (var property in jsonElement.EnumerateObject())
                        {
                            if (property.Name.Equals(propertyNameLower, StringComparison.OrdinalIgnoreCase))
                            {
                                // Found a case-insensitive match
                                var matchedElement = property.Value;
                                
                                // Handle different value kinds
                                switch (matchedElement.ValueKind)
                                {
                                    case JsonValueKind.String:
                                        return matchedElement.GetString();
                                    case JsonValueKind.Number:
                                        return matchedElement.GetRawText();
                                    case JsonValueKind.True:
                                        return "true";
                                    case JsonValueKind.False:
                                        return "false";
                                    case JsonValueKind.Null:
                                        return null;
                                    case JsonValueKind.Object:
                                    case JsonValueKind.Array:
                                        return matchedElement.GetRawText();
                                    default:
                                        return null;
                                }
                            }
                        }
                    }
                    
                    // If property not found at root level, try to find it in the itemData property
                    // This handles the RequestModel<T> structure where critical fields might be in the wrapper
                    if (jsonElement.ValueKind == JsonValueKind.Object && 
                        jsonElement.TryGetProperty("itemData", out JsonElement itemData) &&
                        itemData.ValueKind == JsonValueKind.Object &&
                        itemData.TryGetProperty(propertyName, out JsonElement nestedProp))
                    {
                        // Handle different value kinds for nested property
                        switch (nestedProp.ValueKind)
                        {
                            case JsonValueKind.String:
                                return nestedProp.GetString();
                            case JsonValueKind.Number:
                                return nestedProp.GetRawText();
                            case JsonValueKind.True:
                                return "true";
                            case JsonValueKind.False:
                                return "false";
                            default:
                                return null;
                        }
                    }
                    
                    // If it's a critical field and not found anywhere, return null
                    return null;
                }
                
                // For non-JsonElement types, use reflection to safely get property
                try
                {
                    var prop = jsonObject.GetType().GetProperty(propertyName);
                    if (prop != null)
                    {
                        var value = prop.GetValue(jsonObject);
                        if (value != null)
                        {
                            return value.ToString();
                        }
                    }
                }
                catch
                {
                    // Ignore reflection errors and try the next approach
                }
                
                // Try to access as a dynamic property
                try
                {
                    var value = jsonObject[propertyName];
                    if (value != null)
                    {
                        return value.ToString();
                    }
                }
                catch
                {
                    // Ignore dynamic access errors
                }
                
                return null;
            }
            catch (Exception ex)
            {
                // If any error occurs during property access, log it and return null
                _logger.LogError($"Error accessing property {propertyName}", ex);
                return null;
            }
        }
    }
}