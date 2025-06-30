using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Blazor.RCL.Application.Common.Configuration.Interfaces;
using Blazor.RCL.Application.Models.Configuration;
using Blazor.RCL.Infrastructure.Services;
using Blazor.RCL.Infrastructure.Services.Interfaces;
using Blazor.RCL.NLog.LogService.Interface;
using Newtonsoft.Json.Bson;

namespace Blazor.RCL.Infrastructure.Common.Configuration
{
    /// <summary>
    /// Retrieves and manages configuration values from appsettings.json and AKeyless.
    /// </summary>
    public class ServerAppConfiguration : IAppConfiguration
    {
        #region Private Fields

        private readonly IConfiguration _configuration;
        private readonly IRegistryHelperService _registryHelper;
        private readonly ILogHelper _logger;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the ServerAppConfiguration class.
        /// </summary>
        /// <param name="configuration">The configuration interface.</param>
        /// <param name="registryHelper">The registry helper service.</param>
        /// <param name="logger">The logging helper.</param>
        public ServerAppConfiguration(IConfiguration configuration, IRegistryHelperService registryHelper, ILogHelper logger)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _registryHelper = registryHelper ?? throw new ArgumentNullException(nameof(registryHelper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            try
            {
                LoadDynamicPropertiesFromRegistry();
                ConnectionString = BuildConnectionString();
                BuildAdditionalConnectionStrings();
                LoadSmtpSettings();
                LoadRedisSettings();
                LoadNotificationSettings();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurred while initializing ServerAppConfiguration", ex);
                throw;
            }
        }

        #endregion

        #region Public Properties

        public string BaseUrl => _configuration["BaseUrl"]!;
        public string AppName => _configuration["AppName"]!;
        public string AppLabel => _configuration["AppLabel"]!;
        public string EnvironmentLoaded => _configuration["EnvironmentLoaded"]!;
        public string DomainSite => _configuration["DomainSite"]!;
        public string AuthenticationAPIURL => _configuration["AuthenticationAPIURL"]!;
        public string ReturnURL => _configuration["ReturnURL"]!;
        public Dictionary<string, string> RegistryValues { get; private set; } = new Dictionary<string, string>();
        public Dictionary<string, string> DynamicValues { get; private set; } = new Dictionary<string, string>();
        public string ConnectionString { get; private set; }
        public string CookieName => _configuration["CookieName"]!;
        public double SessionTimeout => double.Parse(_configuration["SessionTimeout"]!);
        public Dictionary<string, string> AdditionalConnectionStrings { get; private set; } = new Dictionary<string, string>();
        public SmtpSettings? SmtpSettings { get; private set; }
        public RedisSettings? RedisSettings { get; private set; }
        public NotificationSettings NotificationSettings { get; private set; } = new NotificationSettings();

        #endregion

        #region Private Methods

        /// <summary>
        /// Loads dynamic properties from the registry based on configuration.
        /// </summary>
        private void LoadDynamicPropertiesFromRegistry()
        {
            try
            {
                // Try to load from AKeyless first using the static method
                var aKeylessSecrets = AKeylessManager.GetStartupSecrets(_configuration, _logger);
                
                if (aKeylessSecrets != null && aKeylessSecrets.Any())
                {
                    RegistryValues = aKeylessSecrets;
                }
                else
                {
                    // Fall back to registry if AKeyless fails
                    var registryPropertyNames = _configuration.GetSection("RegistryConfiguration").GetChildren();
                    var registryPropertyNamesList = new List<string>();
                    foreach (var property in registryPropertyNames)
                    {
                        registryPropertyNamesList.Add(property.Key);
                    }
                    RegistryValues = _registryHelper.GetRegistryValues(registryPropertyNamesList);
                }

                // Load Remaining Dynamic Properties
                var dynamicPropertyNames = _configuration.GetSection("DynamicProperties").GetChildren();
                var dynamicPropertyNamesList = new List<string>();
                foreach (var property in dynamicPropertyNames)
                {
                    dynamicPropertyNamesList.Add(property.Key);
                }
                DynamicValues = _configuration.GetSection("DynamicProperties").Get<Dictionary<string, string>>()!;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurred while loading dynamic properties", ex);
                throw;
            }
        }

        /// <summary>
        /// Builds the connection string using configuration and registry values.
        /// </summary>
        /// <returns>The constructed connection string.</returns>
        private string BuildConnectionString(string databaseName = null!)
        {
            try
            {
                var server = _configuration["Database:Server"];
                var database = databaseName ?? _configuration["Database:Database"];
                var username = _configuration["Database:Username"];
                var password = RegistryValues.TryGetValue("SQLConnectionKey", out var pwd) ? pwd : string.Empty;

                var builder = new StringBuilder();
                builder.Append($"Server={server};");
                builder.Append($"initial catalog={database};");
                builder.Append("integrated security=False;");
                builder.Append($"User Id={username};");
                builder.Append($"password={password};");

                return builder.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurred while building connection string", ex);
                throw;
            }
        }

        /// <summary>
        /// Builds the dynamic list of additional databases from configuration.
        /// </summary>
        /// <returns>The constructed connection string.</returns>
        private void BuildAdditionalConnectionStrings()
        {
            try
            {
                var additionalDatabases = _configuration.GetSection("AdditionalDatabases").Get<List<string>>();
                if (additionalDatabases != null)
                {
                    foreach (var database in additionalDatabases)
                    {
                        AdditionalConnectionStrings[database] = BuildConnectionString(database);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurred while building connection string", ex);
                throw;
            }
        }

        /// <summary>
        /// Loads SMTP settings from configuration, with support for AKeyless secrets or registry fallback for password.
        /// </summary>
        private void LoadSmtpSettings()
        {
            try
            {
                // First, try to load base SMTP settings from configuration
                var smtpSection = _configuration.GetSection("SmtpSettings");
                if (!smtpSection.Exists())
                {
                    return;
                }

                SmtpSettings = smtpSection.Get<SmtpSettings>() ?? new SmtpSettings();

                // If authentication is enabled, load password from secure storage
                if (SmtpSettings.UseAuthentication)
                {
                    // Check if we have SMTP password in RegistryValues (from AKeyless)
                    if (RegistryValues.TryGetValue("SmtpPassword", out var smtpPassword))
                    {
                        // Password will be stored in RegistryValues for secure access
                        // Username comes from configuration
                    }
                    else
                    {
                        // Fallback to registry
                        var smtpRegistryKeys = new List<string> { "SmtpPassword" };
                        var smtpRegistryValues = _registryHelper.GetRegistryValues(smtpRegistryKeys);
                        
                        if (smtpRegistryValues.TryGetValue("SmtpPassword", out var regPassword))
                        {
                            // Store in RegistryValues for consistent access pattern
                            RegistryValues["SmtpPassword"] = regPassword;
                        }
                        else
                        {
                            _logger.LogWarn("SMTP authentication enabled but password not found in AKeyless or Registry");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error loading SMTP settings", ex);
                // Don't throw - SMTP is not critical for app startup
                SmtpSettings = null;
            }
        }

        /// <summary>
        /// Loads Redis settings from configuration, with support for AKeyless secrets or registry fallback for password.
        /// </summary>
        private void LoadRedisSettings()
        {
            try
            {
                // First, try to load Redis settings from configuration
                var redisSection = _configuration.GetSection("Redis");
                if (!redisSection.Exists())
                {
                    return;
                }

                RedisSettings = redisSection.Get<RedisSettings>();
                
                if (RedisSettings != null && RedisSettings.IsValid())
                {
                    // Load Redis password from secure storage
                    if (RegistryValues.TryGetValue("RedisPassword", out var redisPassword))
                    {
                        RedisSettings.Password = redisPassword;
                    }
                    else
                    {
                        // Try to load from registry if not in AKeyless
                        var redisRegistryKeys = new List<string> { "RedisPassword" };
                        var redisRegistryValues = _registryHelper.GetRegistryValues(redisRegistryKeys);
                        
                        if (redisRegistryValues.TryGetValue("RedisPassword", out var regPassword))
                        {
                            RedisSettings.Password = regPassword;
                            RegistryValues["RedisPassword"] = regPassword;
                        }
                        else
                        {
                            RedisSettings = null;
                        }
                    }
                }
                else
                {
                    RedisSettings = null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error loading Redis settings", ex);
                // Don't throw - Redis is optional, fall back to polling mode
                RedisSettings = null;
            }
        }

        /// <summary>
        /// Loads notification settings from configuration.
        /// </summary>
        private void LoadNotificationSettings()
        {
            try
            {
                var notificationSection = _configuration.GetSection("NotificationSettings");
                if (notificationSection.Exists())
                {
                    NotificationSettings = notificationSection.Get<NotificationSettings>() ?? new NotificationSettings();
                }
                else
                {
                    // Use defaults if no configuration exists
                    NotificationSettings = new NotificationSettings();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error loading notification settings", ex);
                // Use defaults on error
                NotificationSettings = new NotificationSettings();
            }
        }
        #endregion
    }
}