using Blazor.RCL.Domain.Entities.AzureAD;
using Blazor.RCL.Infrastructure.Services.Interfaces;
using Blazor.RCL.NLog.LogService.Interface;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace Blazor.RCL.Infrastructure.Services
{
    /// <summary>
    /// Service for retrieving and managing Azure AD configuration options.
    /// </summary>
    public class AzureAdOptionsService : IAzureAdOptions
    {
        #region Fields
        private readonly AzureAdOptions _options;
        private readonly AKeyLessOptions _aKeyLessOptions;
        private readonly ILogHelper _logger;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the AzureAdOptionsService class.
        /// </summary>
        /// <param name="configuration">Application configuration.</param>
        /// <param name="registryHelper">Registry helper service.</param>
        /// <param name="logger">Logger instance.</param>
        public AzureAdOptionsService(IConfiguration configuration, IRegistryHelperService registryHelper, ILogHelper logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            try
            {
                _options = new AzureAdOptions();
                _aKeyLessOptions = new AKeyLessOptions();
                
                // Bind AzureAd section
                configuration.GetSection("AzureAd").Bind(_options);
                
                // Get the ServicePrincipalRegistryKey from the registry
                if (!string.IsNullOrEmpty(_options.ServicePrincipalRegistryKey))
                {
                    _options.ServicePrincipalRegistryKey = registryHelper.GetRegistryValue(_options.ServicePrincipalRegistryKey!);
                }
                
                // Try to bind AKeyLess section if it exists
                var akeylessSection = configuration.GetSection("AKeyLess");
                if (akeylessSection.Exists())
                {
                    akeylessSection.Bind(_aKeyLessOptions);
                    
                    // Ensure Secrets and SecretMappings are initialized
                    if (_aKeyLessOptions.Secrets == null)
                    {
                        _aKeyLessOptions.Secrets = new AKeyLessSecrets();
                    }
                    
                    if (_aKeyLessOptions.Secrets.SecretMappings == null)
                    {
                        _aKeyLessOptions.Secrets.SecretMappings = new Dictionary<string, string>();
                    }
                }
                else
                {
                    _logger.LogWarn("AKeyLess section not found in configuration");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurred while initializing AzureAdOptionsService", ex);
                // Don't throw the exception to prevent app startup failure
                // Just log the error and continue with default values
            }
        }
        #endregion

        #region Public Properties
        /// <inheritdoc/>
        public string ClientId => _options.ClientId!;

        /// <inheritdoc/>
        public string TenantId => _options.TenantId!;

        /// <inheritdoc/>
        public string ApplicationRegistrationObjectId => _options.ApplicationRegistrationObjectId!;

        /// <inheritdoc/>
        public string ServicePrincipalObjectId => _options.ServicePrincipalObjectId!;

        /// <inheritdoc/>
        public string ServicePrincipalRegistryKey => _options.ServicePrincipalRegistryKey!;

        /// <inheritdoc/>
        public string ServicePrincipalAKeylessKey => _options.ServicePrincipalAKeylessKey!;

        /// <inheritdoc/>
        public IReadOnlyList<ApiPermission> ApiPermissions => _options.ApiPermissions!.AsReadOnly();

        /// <inheritdoc/>
        public string SharePointSiteId => _options.SharePointSiteId!;

        /// <inheritdoc/>
        public AKeyLessOptions AKeyLess => _aKeyLessOptions!;
        #endregion
    }
}
