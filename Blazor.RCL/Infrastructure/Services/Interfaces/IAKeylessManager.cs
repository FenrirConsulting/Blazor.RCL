using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blazor.RCL.Infrastructure.Services.Interfaces
{
    /// <summary>
    /// Defines a contract for managing AKeyless credentials and generating access tokens.
    /// </summary>
    public interface IAKeylessManager
    {
        /// <summary>
        /// Gets the Azure credential using AKeyLess with configuration from AzureAdOptions.
        /// </summary>
        /// <returns>The Azure credential for authentication.</returns>
        Task<string> GetAzureCredentialAsync();
        
        /// <summary>
        /// Gets an authentication result for a specified API scope using the Azure credential.
        /// </summary>
        /// <param name="scope">The API scope to request access for.</param>
        /// <returns>Authentication result containing the access token.</returns>
        Task<AuthenticationResult> GetAuthenticationResultAsync(string scope);

        /// <summary>
        /// Gets an access token for a specified API scope using the Azure credential.
        /// </summary>
        /// <param name="scope">The API scope to request access for.</param>
        /// <returns>Access token for the specified scope.</returns>
        Task<string> GetAccessTokenAsync(string scope);

        /// <summary>
        /// Gets an access token with a specific client ID for a specified API scope.
        /// </summary>
        /// <param name="clientId">The client ID to use for the token request.</param>
        /// <param name="scope">The API scope to request access for.</param>
        /// <returns>Access token for the specified client and scope.</returns>
        Task<string> GetAccessTokenWithClientAsync(string clientId, string scope);
        
        /// <summary>
        /// Gets an authentication result with a specific client ID for a specified API scope.
        /// </summary>
        /// <param name="clientId">The client ID to use for the token request.</param>
        /// <param name="scope">The API scope to request access for.</param>
        /// <returns>Authentication result containing the access token.</returns>
        Task<AuthenticationResult> GetAuthenticationResultWithClientAsync(string clientId, string scope);
        
        /// <summary>
        /// Gets static credentials from AKeyless for the specified secret paths.
        /// </summary>
        /// <param name="secretPaths">List of secret paths to retrieve.</param>
        /// <returns>Dictionary mapping secret paths to their values.</returns>
        Task<Dictionary<string, string>> GetStaticCredentialsAsync(List<string> secretPaths);
        
        /// <summary>
        /// Gets all mapped secrets defined in the configuration.
        /// </summary>
        /// <returns>Dictionary mapping configuration keys to their secret values.</returns>
        Task<Dictionary<string, string>> GetAllMappedSecretsAsync();
        
        /// <summary>
        /// Gets a specific mapped secret by its configuration key.
        /// </summary>
        /// <param name="configKey">The configuration key from SecretMappings.</param>
        /// <returns>The secret value.</returns>
        Task<string> GetMappedSecretAsync(string configKey);
    }
}
