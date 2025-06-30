using Blazor.RCL.Infrastructure.Services.Interfaces;
using Blazor.RCL.NLog.LogService.Interface;
using Company.Identity.PAM.Akeyless.CredentialManager;
using Blazor.RCL.Automation.Services;
using Microsoft.Identity.Client;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazor.RCL.Infrastructure.Services
{
    /// <summary>
    /// Service for managing AKeyless credentials and generating access tokens.
    /// Automatically uses configuration from AzureAdOptions.
    /// </summary>
    public class AKeylessManager : IAKeylessManager
    {
        private readonly IAzureAdOptions _azureAdOptions;
        private readonly ILogHelper _logger;
        private readonly CredentialManager _credentialManager;
        private readonly APIServices _apiServices;
        
        // Static cache for startup secrets
        private static Dictionary<string, string> _startupSecretsCache;
        private static readonly object _cacheLock = new object();

        /// <summary>
        /// Initializes a new instance of the AKeylessManager class.
        /// </summary>
        /// <param name="azureAdOptions">Azure AD configuration options.</param>
        /// <param name="logger">Logger for error logging.</param>
        /// <param name="credentialManager">AKeyLess credential manager.</param>
        /// <param name="apiServices">API services for token acquisition.</param>
        public AKeylessManager(
            IAzureAdOptions azureAdOptions,
            ILogHelper logger,
            CredentialManager credentialManager,
            APIServices apiServices)
        {
            _azureAdOptions = azureAdOptions ?? throw new ArgumentNullException(nameof(azureAdOptions));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _credentialManager = credentialManager ?? throw new ArgumentNullException(nameof(credentialManager));
            _apiServices = apiServices ?? throw new ArgumentNullException(nameof(apiServices));
        }

        /// <inheritdoc/>
        public async Task<string> GetAzureCredentialAsync()
        {
            try
            {
                // Validate AKeyLess configuration
                if (_azureAdOptions.AKeyLess == null || string.IsNullOrEmpty(_azureAdOptions.AKeyLess.AccessID))
                {
                    _logger.LogError("AKeyLess configuration is missing or incomplete", null);
                    throw new InvalidOperationException("AKeyLess configuration is missing or incomplete");
                }

                // Wrap synchronous operation in Task.Run to avoid blocking
                var accessKeyTask = Task.Run(() => _credentialManager.GetAzureCredential(
                    _azureAdOptions.AKeyLess.AccessID,
                    _azureAdOptions.AKeyLess.UiD, // This may be null for non-development environments
                    _azureAdOptions.AKeyLess.env  // Prod or NonProd
                ));

                var accessKey = await accessKeyTask;
                
                if (string.IsNullOrEmpty(accessKey))
                {
                    _logger.LogError("Failed to retrieve access key from credential manager", null);
                    throw new InvalidOperationException("Failed to retrieve access key from credential manager");
                }
                
                return accessKey;
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to retrieve Azure credential", ex);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<AuthenticationResult> GetAuthenticationResultAsync(string scope)
        {
            if (string.IsNullOrEmpty(scope))
            {
                throw new ArgumentException("API scope cannot be null or empty", nameof(scope));
            }

            try
            {
                // Get the Azure credential
                var accessKey = await GetAzureCredentialAsync();
                
                // Use the credential to get an access token
                var authResult = await _apiServices.GetAccessToken(
                    _azureAdOptions.ClientId,
                    accessKey,
                    _azureAdOptions.TenantId,
                    scope);

                if (authResult == null)
                {
                    _logger.LogError($"Failed to obtain authentication result for scope {scope}", null);
                    throw new InvalidOperationException($"Failed to obtain authentication result for scope {scope}");
                }

                return authResult;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting authentication result for scope {scope}", ex);
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<string> GetAccessTokenAsync(string scope)
        {
            try
            {
                var authResult = await GetAuthenticationResultAsync(scope);
                return authResult.AccessToken;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting access token for scope {scope}", ex);
                throw;
            }
        }
        
        /// <inheritdoc/>
        public async Task<AuthenticationResult> GetAuthenticationResultWithClientAsync(string clientId, string scope)
        {
            if (string.IsNullOrEmpty(clientId))
            {
                throw new ArgumentException("Client ID cannot be null or empty", nameof(clientId));
            }
            
            if (string.IsNullOrEmpty(scope))
            {
                throw new ArgumentException("API scope cannot be null or empty", nameof(scope));
            }

            try
            {
                // Get the Azure credential
                var accessKey = await GetAzureCredentialAsync();
                
                // Use the credential to get an access token with the specified client ID
                var authResult = await _apiServices.GetAccessToken(
                    clientId,
                    accessKey,
                    _azureAdOptions.TenantId,
                    scope);

                if (authResult == null)
                {
                    _logger.LogError($"Failed to obtain authentication result for client {clientId} and scope {scope}", null);
                    throw new InvalidOperationException($"Failed to obtain authentication result for client {clientId} and scope {scope}");
                }

                return authResult;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting authentication result for client {clientId} and scope {scope}", ex);
                throw;
            }
        }
        
        /// <inheritdoc/>
        public async Task<string> GetAccessTokenWithClientAsync(string clientId, string scope)
        {
            if (string.IsNullOrEmpty(clientId))
            {
                throw new ArgumentException("Client ID cannot be null or empty", nameof(clientId));
            }

            if (string.IsNullOrEmpty(scope))
            {
                throw new ArgumentException("API scope cannot be null or empty", nameof(scope));
            }

            try
            {
                // Get the Azure credential
                var accessKey = await GetAzureCredentialAsync();
                
                // Use the credential to get an access token with the specified client ID
                var authResult = await _apiServices.GetAccessToken(
                    clientId,
                    accessKey,
                    _azureAdOptions.TenantId,
                    scope);

                if (authResult == null || string.IsNullOrEmpty(authResult.AccessToken))
                {
                    _logger.LogError($"Failed to obtain access token for client {clientId} and scope {scope}", null);
                    throw new InvalidOperationException($"Failed to obtain access token for client {clientId} and scope {scope}");
                }

                return authResult.AccessToken;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting access token for client {clientId} and scope {scope}", ex);
                throw;
            }
        }
        
        /// <inheritdoc/>
        public async Task<Dictionary<string, string>> GetStaticCredentialsAsync(List<string> secretPaths)
        {
            if (secretPaths == null || !secretPaths.Any())
            {
                throw new ArgumentException("Secret paths cannot be null or empty", nameof(secretPaths));
            }

            try
            {
                // Validate AKeyLess configuration and secrets
                if (_azureAdOptions.AKeyLess?.Secrets == null)
                {
                    _logger.LogError("GetStaticCredentialsAsync: AKeyLess Secrets configuration is missing", null);
                    throw new InvalidOperationException("AKeyLess Secrets configuration is missing");
                }
                
                var prodAccessId = _azureAdOptions.AKeyLess.Secrets.ProdAccessID;
                var prodUId = _azureAdOptions.AKeyLess.Secrets.ProdUiD;
                var nonProdAccessId = _azureAdOptions.AKeyLess.Secrets.NonProdAccessID;
                var nonProdUId = _azureAdOptions.AKeyLess.Secrets.NonProdUiD;
                
                
                if (string.IsNullOrEmpty(prodAccessId) || string.IsNullOrEmpty(prodUId) ||
                    string.IsNullOrEmpty(nonProdAccessId) || string.IsNullOrEmpty(nonProdUId))
                {
                    _logger.LogError("GetStaticCredentialsAsync: AKeyLess credentials are missing or incomplete", null);
                    throw new InvalidOperationException("AKeyLess credentials are missing or incomplete");
                }
                
                // Wrap the call in Task.Run to avoid blocking
                var credentialsTask = Task.Run(() => _credentialManager.GetStaticCredential(
                    prodAccessId,
                    prodUId,
                    nonProdAccessId,
                    nonProdUId,
                    secretPaths
                ));
                
                var credentials = await credentialsTask;
                
                if (credentials == null || !credentials.Any())
                {
                    _logger.LogError("GetStaticCredentialsAsync: Failed to retrieve static credentials from AKeyLess", null);
                    throw new InvalidOperationException("Failed to retrieve static credentials from AKeyLess");
                }
                
                return credentials;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error retrieving static credentials from AKeyLess", ex);
                throw;
            }
        }
        
        /// <inheritdoc/>
        public async Task<Dictionary<string, string>> GetAllMappedSecretsAsync()
        {
            try
            {
                // Validate AKeyLess configuration and secret mappings
                if (_azureAdOptions.AKeyLess?.Secrets?.SecretMappings == null || 
                    !_azureAdOptions.AKeyLess.Secrets.SecretMappings.Any())
                {
                    _logger.LogError("GetAllMappedSecretsAsync: AKeyLess SecretMappings configuration is missing or empty", null);
                    throw new InvalidOperationException("AKeyLess SecretMappings configuration is missing or empty");
                }
                
                // Get all secret paths from the mappings
                var secretPaths = _azureAdOptions.AKeyLess.Secrets.SecretMappings.Values.ToList();
                
                // Get all the secrets at once
                var secretValues = await GetStaticCredentialsAsync(secretPaths);
                
                // Create a new dictionary mapping configuration keys to secret values
                var result = new Dictionary<string, string>();
                foreach (var mapping in _azureAdOptions.AKeyLess.Secrets.SecretMappings)
                {
                    if (secretValues.TryGetValue(mapping.Value, out var secretValue))
                    {
                        result[mapping.Key] = secretValue;
                    }
                    else
                    {
                    }
                }
                
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error retrieving all mapped secrets from AKeyLess", ex);
                throw;
            }
        }
        
        /// <inheritdoc/>
        public async Task<string> GetMappedSecretAsync(string configKey)
        {
            if (string.IsNullOrEmpty(configKey))
            {
                throw new ArgumentException("Configuration key cannot be null or empty", nameof(configKey));
            }

            try
            {
                // Validate AKeyLess configuration and secret mappings
                if (_azureAdOptions.AKeyLess?.Secrets?.SecretMappings == null)
                {
                    _logger.LogError("AKeyLess SecretMappings configuration is missing", null);
                    throw new InvalidOperationException("AKeyLess SecretMappings configuration is missing");
                }
                
                // Check if the key exists in the mappings
                if (!_azureAdOptions.AKeyLess.Secrets.SecretMappings.TryGetValue(configKey, out var secretPath))
                {
                    _logger.LogError($"No secret mapping found for key: {configKey}", null);
                    throw new KeyNotFoundException($"No secret mapping found for key: {configKey}");
                }
                
                // Get the secret value
                var secretValues = await GetStaticCredentialsAsync(new List<string> { secretPath });
                
                if (!secretValues.TryGetValue(secretPath, out var secretValue))
                {
                    _logger.LogError($"Failed to retrieve secret for path: {secretPath}", null);
                    throw new InvalidOperationException($"Failed to retrieve secret for path: {secretPath}");
                }
                
                return secretValue;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving mapped secret for key: {configKey}", ex);
                throw;
            }
        }
        
        /// <summary>
        /// Static method to retrieve AKeyless secrets during startup without dependency injection.
        /// This avoids circular dependency issues during service registration.
        /// </summary>
        public static Dictionary<string, string> GetStartupSecrets(IConfiguration configuration, ILogHelper logger)
        {
            lock (_cacheLock)
            {
                if (_startupSecretsCache != null)
                {
                    return _startupSecretsCache;
                }
            }

            try
            {

                // Get AKeyless configuration
                var aKeylessConfig = configuration.GetSection("AKeyLess");
                var secretsConfig = aKeylessConfig.GetSection("Secrets");
                var secretMappings = secretsConfig.GetSection("SecretMappings").Get<Dictionary<string, string>>();

                if (secretMappings == null || !secretMappings.Any())
                {
                    logger.LogError("AKeylessManager.GetStartupSecrets: No secret mappings found in configuration", null);
                    return new Dictionary<string, string>();
                }

                // Get credentials from configuration
                var prodAccessId = secretsConfig["ProdAccessID"];
                var prodUId = secretsConfig["ProdUiD"];
                var nonProdAccessId = secretsConfig["NonProdAccessID"];
                var nonProdUId = secretsConfig["NonProdUiD"];


                // Get all secret paths
                var secretPaths = secretMappings.Values.ToList();

                // Create a temporary CredentialManager instance
                var credentialManager = new CredentialManager();

                // Call AKeyless synchronously (blocking is OK during startup)
                Dictionary<string, string> credentials = null;
                try
                {
                    var credentialsTask = credentialManager.GetStaticCredential(
                        prodAccessId,
                        prodUId,
                        nonProdAccessId,
                        nonProdUId,
                        secretPaths
                    );
                    
                    // Wait for the task to complete synchronously
                    credentials = credentialsTask.GetAwaiter().GetResult();
                }
                catch (Exception credEx)
                {
                    logger.LogError("AKeylessManager.GetStartupSecrets: Error calling CredentialManager", credEx);
                }


                // Map the results
                var result = new Dictionary<string, string>();
                if (credentials != null)
                {
                    foreach (var mapping in secretMappings)
                    {
                        if (credentials.TryGetValue(mapping.Value, out var secretValue))
                        {
                            result[mapping.Key] = secretValue;
                        }
                        else
                        {
                        }
                    }
                }

                lock (_cacheLock)
                {
                    _startupSecretsCache = result;
                }

                return result;
            }
            catch (Exception ex)
            {
                logger.LogError("AKeylessManager.GetStartupSecrets: Error retrieving secrets", ex);
                return new Dictionary<string, string>();
            }
        }
    }
}
