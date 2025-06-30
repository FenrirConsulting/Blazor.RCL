using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Session;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.SignalR;

using Blazor.RCL.Application.Common.Configuration.Interfaces;
using Blazor.RCL.Infrastructure.Services;
using Blazor.RCL.Infrastructure.Authentication;
using Blazor.RCL.Infrastructure.Common.Configuration;
using Blazor.RCL.Infrastructure.Authentication.Interfaces;
using Blazor.RCL.Infrastructure.Navigation;
using Blazor.RCL.Infrastructure.Services.Interfaces;
using Blazor.RCL.NLog.LogService.Interface;
using Blazor.RCL.NLog.LogService;
using Blazor.RCL.Application.Interfaces;
using Blazor.RCL.Infrastructure.Data.Repositories;
using Blazor.RCL.Infrastructure.Data;
using Blazor.RCL.Infrastructure.Common;
using Blazor.RCL.Application.Models;
using Blazor.RCL.Automation.Services;
using Blazor.RCL.Automation.Services.Interfaces;
using Company.Identity.PAM.Akeyless.CredentialManager;
using Blazor.RCL.Automation.Data;
using Blazor.RCL.Automation.AutomationDirectory.AutomationDirectoryRepositories.Interfaces;
using Blazor.RCL.Automation.AutomationDirectory.AutomationDirectoryRepositories;
using Blazor.RCL.Infrastructure.Hubs;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor.Extensions;
using MudBlazor.Services;
using StackExchange.Redis;
using MudBlazor;

namespace Blazor.RCL.Infrastructure.Extensions
{
    /// <summary>
    /// Extension methods for IServiceCollection and WebApplication to configure standard services and middleware for Automation applications.
    /// </summary>
    [System.Runtime.Versioning.SupportedOSPlatform("windows")]
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds standard services required by Automation web applications to the service collection.
        /// </summary>
        /// <param name="services">The IServiceCollection to add services to.</param>
        /// <param name="config">The configuration object.</param>
        /// <returns>The updated IServiceCollection.</returns>
        public static IServiceCollection AddStandardServices(this IServiceCollection services, IConfiguration config)
        {
            #region Core Application Services
            // MVC and Razor components
            services.AddControllers();
            services.AddRazorComponents()
                    .AddInteractiveServerComponents();
            services.AddHttpContextAccessor();
            services.AddScoped<ErrorHandlerService>();
            services.AddHttpClient();

            // Blazor server configuration
            services.AddServerSideBlazor(options =>
            {
                options.DisconnectedCircuitRetentionPeriod = TimeSpan.FromMinutes(3);
                options.MaxBufferedUnacknowledgedRenderBatches = 10;
                options.JSInteropDefaultCallTimeout = TimeSpan.FromSeconds(60);
                options.DisconnectedCircuitMaxRetained = 100; // Limit the number of disconnected circuits
            })
            .AddHubOptions(options =>
            {
                options.ClientTimeoutInterval = TimeSpan.FromSeconds(60);
                options.KeepAliveInterval = TimeSpan.FromSeconds(15);
                options.HandshakeTimeout = TimeSpan.FromSeconds(15);
                options.MaximumReceiveMessageSize = 32 * 1024; // 32KB
            });

            // SignalR configuration with conditional Redis backplane
            var signalRBuilder = services.AddSignalR(options =>
            {
                options.EnableDetailedErrors = config["EnvironmentLoaded"]?.Equals("Development", StringComparison.OrdinalIgnoreCase) == true;
                options.KeepAliveInterval = TimeSpan.FromSeconds(15);
                options.ClientTimeoutInterval = TimeSpan.FromSeconds(60);
                options.HandshakeTimeout = TimeSpan.FromSeconds(15);
            });

            // Configure Redis backplane if settings are available
            var redisSection = config.GetSection("Redis");
            if (redisSection.Exists() && !string.IsNullOrEmpty(redisSection["Server"]))
            {
                // Build Redis connection string from configuration
                var redisBuilder = new System.Text.StringBuilder();
                redisBuilder.Append(redisSection["Server"]);
                
                // Password will be loaded by ServerAppConfiguration from AKeyless/Registry
                redisBuilder.Append(",abortConnect=false");
                
                if (bool.TryParse(redisSection["UseSSL"], out var useSSL) && useSSL)
                {
                    redisBuilder.Append(",ssl=true");
                }
                
                if (int.TryParse(redisSection["ConnectTimeout"], out var connectTimeout))
                {
                    redisBuilder.Append($",connectTimeout={connectTimeout}");
                }
                
                if (int.TryParse(redisSection["SyncTimeout"], out var syncTimeout))
                {
                    redisBuilder.Append($",syncTimeout={syncTimeout}");
                }
                
                if (bool.TryParse(redisSection["AllowAdmin"], out var allowAdmin) && allowAdmin)
                {
                    redisBuilder.Append(",allowAdmin=true");
                }

                // Add Redis backplane to SignalR
                signalRBuilder.AddStackExchangeRedis(options =>
                {
                    options.ConnectionFactory = async writer =>
                    {
                        var appConfig = services.BuildServiceProvider().GetRequiredService<IAppConfiguration>();
                        var connectionString = redisBuilder.ToString();
                        
                        // Add password if available
                        if (!string.IsNullOrEmpty(appConfig.RedisSettings?.Password))
                        {
                            connectionString = connectionString.Replace(redisSection["Server"]!, 
                                $"{redisSection["Server"]},password={appConfig.RedisSettings.Password}");
                        }
                        
                        // Add allowAdmin if configured
                        if (appConfig.RedisSettings?.AllowAdmin == true && !connectionString.Contains("allowAdmin"))
                        {
                            connectionString += ",allowAdmin=true";
                        }
                        
                        var connection = await ConnectionMultiplexer.ConnectAsync(connectionString, writer);
                        return connection;
                    };
                    
                    if (!string.IsNullOrEmpty(redisSection["ChannelPrefix"]))
                    {
                        options.Configuration.ChannelPrefix = redisSection["ChannelPrefix"]!;
                    }
                });
            }

            // Register notification publisher based on Redis availability
            services.AddSingleton<INotificationPublisher>(sp =>
            {
                var appConfig = sp.GetRequiredService<IAppConfiguration>();
                var logHelper = sp.GetRequiredService<ILogHelper>();
                
                // Check if Redis settings exist in configuration
                if (appConfig.RedisSettings == null || string.IsNullOrEmpty(appConfig.RedisSettings.Server))
                {
                    return new LocalNotificationPublisher(logHelper);
                }

                try
                {
                    // Build Redis connection string
                    var redisConnectionString = $"{appConfig.RedisSettings.Server},password={appConfig.RedisSettings.Password}";
                    if (appConfig.RedisSettings.UseSSL)
                    {
                        redisConnectionString += ",ssl=true";
                    }
                    redisConnectionString += $",connectTimeout={appConfig.RedisSettings.ConnectTimeout},syncTimeout={appConfig.RedisSettings.SyncTimeout},abortConnect=false";
                    
                    if (appConfig.RedisSettings.AllowAdmin)
                    {
                        redisConnectionString += ",allowAdmin=true";
                    }

                    // Try to connect to Redis
                    var redis = ConnectionMultiplexer.Connect(redisConnectionString);
                    
                    return new RedisNotificationPublisher(redis, sp.GetRequiredService<IHubContext<NotificationHub>>(), 
                        logHelper, appConfig.RedisSettings.ChannelPrefix);
                }
                catch (Exception ex)
                {
                    logHelper.LogError("Failed to connect to Redis. Using local notification publisher.", ex);
                    return new LocalNotificationPublisher(logHelper);
                }
            });
            #endregion

            #region Logging Services
            // NLog Services for logging
            services.AddTransient<ILogService, LogService>();
            services.AddTransient<ILogHelper, LogHelper>();
            #endregion

            #region UI Services
            // MudBlazor UI Components
            services.AddMudServices();
            services.AddMudBlazorSnackbar();
            services.AddMudServicesWithExtensions();
            services.AddScoped<IThemeService, ThemeService>();

            // Navigation and UI state management
            services.AddScoped<IRequestRefresh, RequestRefresh>();
            services.AddScoped<IdentityRedirectManager>();
            services.AddSingleton<INotificationManager, NotificationManager>();
            #endregion

            #region Caching Services
            // Memory cache for in-process caching
            services.AddMemoryCache();
            
            // Distributed cache configuration
            services.AddScoped<IDistributedCacheRepository, DistributedCacheRepository>();
            services.AddSingleton<IDistributedCache>(sp => new CustomDistributedCache(sp));

            // Session configuration
            services.AddSingleton<ISessionStore>(provider =>
            {
                var distributedCache = provider.GetRequiredService<IDistributedCache>();
                var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
                return new DistributedSessionStore(distributedCache, loggerFactory);
            });

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            #endregion

            #region Configuration Services
            // Static configuration services
            services.AddSingleton<LdapServerList>();
            services.AddSingleton<LdapRoleMappingConfig>();
            services.AddSingleton<NavLinksInfoList>();
            services.AddSingleton<RemoteScriptList>();
            services.AddSingleton<ServiceAccountList>();
            services.AddSingleton<ServerHostList>();
            services.AddSingleton<IRegistryHelperService, RegistryHelperService>();

            // AppSettings configuration service
            services.AddSingleton<IAppConfiguration>(sp =>
            {
                var registryHelper = sp.GetRequiredService<IRegistryHelperService>();
                var logHelper = sp.GetRequiredService<ILogHelper>();
                return new ServerAppConfiguration(config, registryHelper, logHelper);
            });

            // Azure AD configuration service
            services.AddSingleton<IAzureAdOptions>(sp =>
            {
                var registryHelper = sp.GetRequiredService<IRegistryHelperService>();
                var logHelper = sp.GetRequiredService<ILogHelper>();
                return new AzureAdOptionsService(config, registryHelper, logHelper);
            });

            // Initialization service
            services.AddHostedService<ConfigurationInitializationService>();
            #endregion

            #region Authentication & Authorization Services
            // Authentication services
            services.AddScoped<IDomainUserGroupService, DomainUserGroupService>();
            services.AddScoped<IClaimsTransformation, CustomClaimsTransform>();
            services.AddCascadingAuthenticationState();
            services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
            services.AddScoped<ILdapAuthenticationService, LdapAuthenticationService>();
            services.AddScoped<RoleUpdateRequest>();

            // AKeyless services for secure credential management
            services.AddScoped<CredentialManager>();
            services.AddScoped<IAKeylessManager, AKeylessManager>();
            services.AddScoped<IUserSettingsService, UserSettingsService>();

            // Data protection configuration
            if (config["EnvironmentLoaded"]?.Equals("Development", StringComparison.OrdinalIgnoreCase) == true)
            {
                services.AddDataProtection()
               .PersistKeysToFileSystem(new DirectoryInfo(@"E:\Automation\TOKENS"))
               .SetApplicationName(config["CookieName"]!);
            }
            else
            {
                services.AddDataProtection()
               .PersistKeysToDbContext<AppDbContext>()
               .SetApplicationName(config["CookieName"]!);
            }

            // Cookie authentication
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.Cookie.Name = config["CookieName"];
                    options.Cookie.Domain = config["DomainSite"];
                    options.Cookie.Path = "/";
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    options.Cookie.SameSite = SameSiteMode.Lax;
                    options.LoginPath = "/account/cookielogin";
                    options.ExpireTimeSpan = TimeSpan.FromHours(config.GetValue<int>("SessionTimeout", 1));
                    options.SlidingExpiration = true;
                });

            // Cookie handler for API requests
            services.AddTransient<CookieHandler>();
            services.AddHttpClient(config["CookieName"]!)
                    .AddHttpMessageHandler<CookieHandler>();

            services.AddHttpClient("AuthenticationAPI", c =>
            {
                c.BaseAddress = new Uri(config["AuthenticationAPIURL"]!);
            })
            .AddHttpMessageHandler<CookieHandler>();

            // Authorization configuration
            services.AddAuthorization();
            #endregion

            #region Database Services

            // Configure standard logging for all DbContexts
            var loggerFactory = LoggerFactory.Create(builder => builder.AddFilter((category, level) =>
                category == DbLoggerCategory.Database.Command.Name
                && level == LogLevel.Information));

            // Main application DbContext
            services.AddDbContextFactory<AppDbContext>((sp, options) =>
            {
                var appConfiguration = sp.GetRequiredService<IAppConfiguration>();
                options.UseSqlServer(appConfiguration.ConnectionString);
                options.UseLoggerFactory(loggerFactory);
            });

            // Automation database contexts configuration
            services.AddDbContextFactory<AutomationRequestDbContext>((sp, options) =>
            {
                var appConfiguration = sp.GetRequiredService<IAppConfiguration>();
                if (appConfiguration.AdditionalConnectionStrings.TryGetValue("AutomationRequest", out var connectionString))
                {
                    options.UseSqlServer(connectionString);
                    options.UseLoggerFactory(loggerFactory);
                }
            });

            services.AddDbContextFactory<AutomationAppDbContext>((sp, options) =>
            {
                var appConfiguration = sp.GetRequiredService<IAppConfiguration>();
                if (appConfiguration.AdditionalConnectionStrings.TryGetValue("AutomationApp", out var connectionString))
                {
                    options.UseSqlServer(connectionString);
                    options.UseLoggerFactory(loggerFactory);
                }
            });

            services.AddDbContextFactory<AutomationBatchDbContext>((sp, options) =>
            {
                var appConfiguration = sp.GetRequiredService<IAppConfiguration>();
                if (appConfiguration.AdditionalConnectionStrings.TryGetValue("AutomationBatch", out var connectionString))
                {
                    options.UseSqlServer(connectionString);
                    options.UseLoggerFactory(loggerFactory);
                }
            });

            services.AddDbContextFactory<AutomationDirectoryDbContext>((sp, options) =>
            {
                var appConfiguration = sp.GetRequiredService<IAppConfiguration>();
                if (appConfiguration.AdditionalConnectionStrings.TryGetValue("AutomationDirectory", out var connectionString))
                {
                    options.UseSqlServer(connectionString);
                    options.UseLoggerFactory(loggerFactory);
                }
            });

            services.AddDbContextFactory<AutomationMailDbContext>((sp, options) =>
            {
                var appConfiguration = sp.GetRequiredService<IAppConfiguration>();
                if (appConfiguration.AdditionalConnectionStrings.TryGetValue("AutomationMail", out var connectionString))
                {
                    options.UseSqlServer(connectionString);
                    options.UseLoggerFactory(loggerFactory);
                }
            });
            #endregion

            #region Repository Services
            // Data access repositories
            services.AddScoped<ILDAPServerRepository, LDAPServerRepository>();
            services.AddScoped<IToolsConfigurationRepository, ToolsConfigurationRepository>();
            services.AddScoped<IRemoteScriptRepository, RemoteScriptRepository>();
            services.AddScoped<IServiceAccountRepository, ServiceAccountRepository>();
            services.AddScoped<IUserSettingsRepository, UserSettingsRepository>();
            services.AddScoped<IServerHostRepository, ServerHostRepository>();
            services.AddScoped<IDistributedCacheRepository, DistributedCacheRepository>();
            services.AddScoped<IToolsRequestRepository, ToolsRequestRepository>();
            services.AddScoped<IADADRequestRepository, ADADRequestRepository>();

            // Notification repositories
            services.AddScoped<INotificationMessageRepository, NotificationMessageRepository>();
            services.AddScoped<IUserNotificationSettingsRepository, UserNotificationSettingsRepository>();
            services.AddScoped<IApplicationNotificationProfileRepository, ApplicationNotificationProfileRepository>();
            services.AddScoped<IUserApplicationNotificationSettingsRepository, UserApplicationNotificationSettingsRepository>();
            services.AddScoped<INotificationDeliveryRepository, NotificationDeliveryRepository>();
            services.AddScoped<IEmailNotificationQueueRepository, EmailNotificationQueueRepository>();
            services.AddScoped<IEmailTemplateRepository, EmailTemplateRepository>();

            // SharePoint service
            services.AddSingleton<ISharePointService, SharePointService>();
            #endregion

            #region Automation Services
            // Tools request services
            services.AddScoped<IToolsRequestService, ToolsRequestService>();
            services.AddScoped<IRequestStatusCodeRepository, RequestStatusCodeRepository>();

            // Request data services
            services.AddScoped<RequestResponseService>();
            services.AddScoped<RequestJsonService>();

            // API communication services
            services.AddScoped<ResponseProcessingService>();
            services.AddScoped<APIServices>();
            services.AddHttpClient<APIServices>();

            // Notification services
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IUserNotificationSettingsService, UserNotificationSettingsService>();
            services.AddScoped<IApplicationNotificationProfileService, ApplicationNotificationProfileService>();
            services.AddScoped<IEmailTemplateService, EmailTemplateService>();
            services.AddScoped<IEmailNotificationService, EmailNotificationService>();
            #endregion

            return services;
        }

        /// <summary>
        /// Configures standard middleware components required by Automation applications.
        /// </summary>
        /// <typeparam name="TApp">The type of the App component to use for Razor component mapping.</typeparam>
        /// <param name="app">The WebApplication to configure.</param>
        /// <param name="config">The configuration object.</param>
        /// <returns>The configured WebApplication.</returns>
        public static WebApplication ConfigureStandardMiddleware<TApp>(this WebApplication app, IConfiguration config)
        {
            #region Middleware Configuration
            // Set the Base HREF from AppSettings.JSON Property
            var appConfiguration = app.Services.GetRequiredService<IAppConfiguration>();
            var baseUrl = appConfiguration.BaseUrl;
            app.UsePathBase(baseUrl);

            // Configure environment-specific error handling
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error", createScopeForErrors: true);
            }

            // --- Begin Core Middleware Pipeline ---

            // HTTPS redirection and basic static files
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            // Configure static files with no-cache headers to prevent browser caching
            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = ctx =>
                {
                    // Prevent caching for CSS and JS files
                    if (ctx.File.Name.EndsWith(".css") || ctx.File.Name.EndsWith(".js"))
                    {
                        ctx.Context.Response.Headers.Append("Cache-Control", "no-cache, no-store, must-revalidate");
                        ctx.Context.Response.Headers.Append("Pragma", "no-cache");
                        ctx.Context.Response.Headers.Append("Expires", "0");
                    }
                }
            });

            // Add file provider for media files in E:\Automation\Media\
            var mediaPath = Path.Combine("E:", "Automation", "Media");
            if (Directory.Exists(mediaPath))
            {
                app.UseStaticFiles(new StaticFileOptions
                {
                    FileProvider = new PhysicalFileProvider(mediaPath),
                    RequestPath = "/Media",
                    OnPrepareResponse = ctx =>
                    {
                        // Apply no-cache headers to media files
                        ctx.Context.Response.Headers.Append("Cache-Control", "no-cache, no-store, must-revalidate");
                        ctx.Context.Response.Headers.Append("Pragma", "no-cache");
                        ctx.Context.Response.Headers.Append("Expires", "0");
                    }
                });
            }
            else
            {
                var logger = app.Services.GetRequiredService<ILogHelper>();
                logger.LogWarn("Media directory not found: {0}. Static files middleware for media not configured.", mediaPath);
            }

            // --- Authentication/Authorization Pipeline ---
            // Important: UseRouting must come before UseAuthentication and UseAuthorization
            app.UseRouting();

            // Session and authentication middleware (must be after UseRouting)
            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();

            // Anti-forgery middleware (must be after authentication but before endpoints)
            app.UseAntiforgery();

            // --- Endpoint Mapping ---

            // Map API controllers
            app.MapControllers();

            // Map Blazor components and configure render mode
            app.MapRazorComponents<TApp>()
                .AddInteractiveServerRenderMode()
                .AddAdditionalAssemblies(typeof(ServiceCollectionExtensions).Assembly);

            // Map SignalR notification hub
            app.MapHub<NotificationHub>("notificationHub");
            #endregion

            return app;
        }

        /// <summary>
        /// Adds standard services required by Automation API applications to the service collection.
        /// </summary>
        /// <param name="services">The IServiceCollection to add services to.</param>
        /// <param name="config">The configuration object.</param>
        /// <returns>The updated IServiceCollection.</returns>
        public static IServiceCollection AddStandardAPIServices(this IServiceCollection services, IConfiguration config)
        {
            #region Core Application Services
            // MVC and API services
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            // Note: Swagger setup should be done in the client application
            // as it requires additional NuGet packages not included in RCL
            services.AddHttpContextAccessor();
            services.AddScoped<ErrorHandlerService>();
            services.AddHttpClient();

            // SignalR configuration with conditional Redis backplane
            var signalRBuilder = services.AddSignalR(options =>
            {
                options.EnableDetailedErrors = config["EnvironmentLoaded"]?.Equals("Development", StringComparison.OrdinalIgnoreCase) == true;
                options.KeepAliveInterval = TimeSpan.FromSeconds(15);
                options.ClientTimeoutInterval = TimeSpan.FromSeconds(60);
                options.HandshakeTimeout = TimeSpan.FromSeconds(15);
            });

            // Configure Redis backplane if settings are available
            var redisSection = config.GetSection("Redis");
            if (redisSection.Exists() && !string.IsNullOrEmpty(redisSection["Server"]))
            {
                // Build Redis connection string from configuration
                var redisBuilder = new System.Text.StringBuilder();
                redisBuilder.Append(redisSection["Server"]);
                
                // Password will be loaded by ServerAppConfiguration from AKeyless/Registry
                redisBuilder.Append(",abortConnect=false");
                
                if (bool.TryParse(redisSection["UseSSL"], out var useSSL) && useSSL)
                {
                    redisBuilder.Append(",ssl=true");
                }
                
                if (int.TryParse(redisSection["ConnectTimeout"], out var connectTimeout))
                {
                    redisBuilder.Append($",connectTimeout={connectTimeout}");
                }
                
                if (int.TryParse(redisSection["SyncTimeout"], out var syncTimeout))
                {
                    redisBuilder.Append($",syncTimeout={syncTimeout}");
                }

                // Add Redis backplane to SignalR
                signalRBuilder.AddStackExchangeRedis(options =>
                {
                    options.ConnectionFactory = async writer =>
                    {
                        var appConfig = services.BuildServiceProvider().GetRequiredService<IAppConfiguration>();
                        var connectionString = redisBuilder.ToString();
                        
                        // Add password if available
                        if (!string.IsNullOrEmpty(appConfig.RedisSettings?.Password))
                        {
                            connectionString = connectionString.Replace(redisSection["Server"]!, 
                                $"{redisSection["Server"]},password={appConfig.RedisSettings.Password}");
                        }
                        
                        var connection = await ConnectionMultiplexer.ConnectAsync(connectionString, writer);
                        return connection;
                    };
                    
                    if (!string.IsNullOrEmpty(redisSection["ChannelPrefix"]))
                    {
                        options.Configuration.ChannelPrefix = redisSection["ChannelPrefix"];
                    }
                });
            }

            // Register notification publisher based on Redis availability
            services.AddSingleton<INotificationPublisher>(sp =>
            {
                var appConfig = sp.GetRequiredService<IAppConfiguration>();
                var logHelper = sp.GetRequiredService<ILogHelper>();
                
                // Check if Redis settings exist in configuration
                if (appConfig.RedisSettings == null || string.IsNullOrEmpty(appConfig.RedisSettings.Server))
                {
                    return new LocalNotificationPublisher(logHelper);
                }

                try
                {
                    // Build Redis connection string
                    var redisConnectionString = $"{appConfig.RedisSettings.Server},password={appConfig.RedisSettings.Password}";
                    if (appConfig.RedisSettings.UseSSL)
                    {
                        redisConnectionString += ",ssl=true";
                    }
                    redisConnectionString += $",connectTimeout={appConfig.RedisSettings.ConnectTimeout},syncTimeout={appConfig.RedisSettings.SyncTimeout},abortConnect=false";

                    // Try to connect to Redis
                    var redis = ConnectionMultiplexer.Connect(redisConnectionString);
                    
                    return new RedisNotificationPublisher(redis, sp.GetRequiredService<IHubContext<NotificationHub>>(), 
                        logHelper, appConfig.RedisSettings.ChannelPrefix);
                }
                catch (Exception ex)
                {
                    logHelper.LogError("Failed to connect to Redis. Using local notification publisher.", ex);
                    return new LocalNotificationPublisher(logHelper);
                }
            });
            #endregion

            #region Logging Services
            // NLog Services for logging
            services.AddTransient<ILogService, LogService>();
            services.AddTransient<ILogHelper, LogHelper>();
            #endregion

            #region UI Services
            // Notification Manager (only UI service needed for API)
            services.AddSingleton<INotificationManager, NotificationManager>();
            #endregion

            #region Caching Services
            // Memory cache for in-process caching
            services.AddMemoryCache();
            
            // Distributed cache configuration
            services.AddScoped<IDistributedCacheRepository, DistributedCacheRepository>();
            services.AddSingleton<IDistributedCache>(sp => new CustomDistributedCache(sp));

            // Session configuration
            services.AddSingleton<ISessionStore>(provider =>
            {
                var distributedCache = provider.GetRequiredService<IDistributedCache>();
                var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
                return new DistributedSessionStore(distributedCache, loggerFactory);
            });

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            #endregion

            #region Configuration Services
            // Static configuration services
            services.AddSingleton<LdapServerList>();
            services.AddSingleton<LdapRoleMappingConfig>();
            services.AddSingleton<NavLinksInfoList>();
            services.AddSingleton<RemoteScriptList>();
            services.AddSingleton<ServiceAccountList>();
            services.AddSingleton<ServerHostList>();
            services.AddSingleton<IRegistryHelperService, RegistryHelperService>();

            // AppSettings configuration service
            services.AddSingleton<IAppConfiguration>(sp =>
            {
                var registryHelper = sp.GetRequiredService<IRegistryHelperService>();
                var logHelper = sp.GetRequiredService<ILogHelper>();
                return new ServerAppConfiguration(config, registryHelper, logHelper);
            });

            // Azure AD configuration service
            services.AddSingleton<IAzureAdOptions>(sp =>
            {
                var registryHelper = sp.GetRequiredService<IRegistryHelperService>();
                var logHelper = sp.GetRequiredService<ILogHelper>();
                return new AzureAdOptionsService(config, registryHelper, logHelper);
            });

            // Initialization service
            services.AddHostedService<ConfigurationInitializationService>();
            #endregion

            #region Authentication & Authorization Services
            // Authentication services
            services.AddSingleton<IDomainUserGroupService, DomainUserGroupService>();
            services.AddScoped<IClaimsTransformation, CustomClaimsTransform>();
            services.AddScoped<ILdapAuthenticationService, LdapAuthenticationService>();
            services.AddScoped<RoleUpdateRequest>();

            // AKeyless services for secure credential management
            services.AddSingleton<CredentialManager>();
            services.AddSingleton<IAKeylessManager, AKeylessManager>();
            services.AddScoped<IUserSettingsService, UserSettingsService>();

            // Data protection configuration
            if (config["EnvironmentLoaded"]?.Equals("Development", StringComparison.OrdinalIgnoreCase) == true)
            {
                services.AddDataProtection()
               .PersistKeysToFileSystem(new DirectoryInfo(@"E:\Automation\TOKENS"))
               .SetApplicationName(config["CookieName"]!);
            }
            else
            {
                services.AddDataProtection()
               .PersistKeysToDbContext<AppDbContext>()
               .SetApplicationName(config["CookieName"]!);
            }

            // Cookie authentication
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.Cookie.Name = config["CookieName"];
                    options.Cookie.Domain = config["DomainSite"];
                    options.Cookie.Path = "/";
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    options.Cookie.SameSite = SameSiteMode.Lax;
                    options.LoginPath = "/account/cookielogin";
                    options.ExpireTimeSpan = TimeSpan.FromHours(config.GetValue<int>("SessionTimeout", 1));
                    options.SlidingExpiration = true;
                });

            // Cookie handler for API requests
            services.AddTransient<CookieHandler>();
            services.AddHttpClient(config["CookieName"]!)
                    .AddHttpMessageHandler<CookieHandler>();

            // API-specific CORS policy
            services.AddCors(options =>
            {
                options.AddPolicy("AllowLocalhost",
                     builder =>
                     {
                         builder.SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost")
                                .AllowAnyHeader()
                                .AllowAnyMethod()
                                .AllowCredentials();
                     });
            });

            // Authorization configuration
            services.AddAuthorization();
            #endregion

            #region Database Services

            // Configure standard logging for all DbContexts
            var loggerFactory = LoggerFactory.Create(builder => builder.AddFilter((category, level) =>
                category == DbLoggerCategory.Database.Command.Name
                && level == LogLevel.Information));

            // Main application DbContext
            services.AddDbContextFactory<AppDbContext>((sp, options) =>
            {
                var appConfiguration = sp.GetRequiredService<IAppConfiguration>();
                options.UseSqlServer(appConfiguration.ConnectionString);
                options.UseLoggerFactory(loggerFactory);
            });

            // Automation database contexts configuration
            services.AddDbContextFactory<AutomationRequestDbContext>((sp, options) =>
            {
                var appConfiguration = sp.GetRequiredService<IAppConfiguration>();
                if (appConfiguration.AdditionalConnectionStrings.TryGetValue("AutomationRequest", out var connectionString))
                {
                    options.UseSqlServer(connectionString);
                    options.UseLoggerFactory(loggerFactory);
                }
            });

            services.AddDbContextFactory<AutomationAppDbContext>((sp, options) =>
            {
                var appConfiguration = sp.GetRequiredService<IAppConfiguration>();
                if (appConfiguration.AdditionalConnectionStrings.TryGetValue("AutomationApp", out var connectionString))
                {
                    options.UseSqlServer(connectionString);
                    options.UseLoggerFactory(loggerFactory);
                }
            });

            services.AddDbContextFactory<AutomationBatchDbContext>((sp, options) =>
            {
                var appConfiguration = sp.GetRequiredService<IAppConfiguration>();
                if (appConfiguration.AdditionalConnectionStrings.TryGetValue("AutomationBatch", out var connectionString))
                {
                    options.UseSqlServer(connectionString);
                    options.UseLoggerFactory(loggerFactory);
                }
            });

            services.AddDbContextFactory<AutomationDirectoryDbContext>((sp, options) =>
            {
                var appConfiguration = sp.GetRequiredService<IAppConfiguration>();
                if (appConfiguration.AdditionalConnectionStrings.TryGetValue("AutomationDirectory", out var connectionString))
                {
                    options.UseSqlServer(connectionString);
                    options.UseLoggerFactory(loggerFactory);
                }
            });

            services.AddDbContextFactory<AutomationMailDbContext>((sp, options) =>
            {
                var appConfiguration = sp.GetRequiredService<IAppConfiguration>();
                if (appConfiguration.AdditionalConnectionStrings.TryGetValue("AutomationMail", out var connectionString))
                {
                    options.UseSqlServer(connectionString);
                    options.UseLoggerFactory(loggerFactory);
                }
            });
            #endregion

            #region Repository Services
            // Data access repositories
            services.AddScoped<ILDAPServerRepository, LDAPServerRepository>();
            services.AddScoped<IToolsConfigurationRepository, ToolsConfigurationRepository>();
            services.AddScoped<IRemoteScriptRepository, RemoteScriptRepository>();
            services.AddScoped<IServiceAccountRepository, ServiceAccountRepository>();
            services.AddScoped<IUserSettingsRepository, UserSettingsRepository>();
            services.AddScoped<IServerHostRepository, ServerHostRepository>();
            services.AddScoped<IDistributedCacheRepository, DistributedCacheRepository>();
            services.AddScoped<IToolsRequestRepository, ToolsRequestRepository>();
            services.AddScoped<IADADRequestRepository, ADADRequestRepository>();

            // Notification repositories
            services.AddScoped<INotificationMessageRepository, NotificationMessageRepository>();
            services.AddScoped<IUserNotificationSettingsRepository, UserNotificationSettingsRepository>();
            services.AddScoped<IApplicationNotificationProfileRepository, ApplicationNotificationProfileRepository>();
            services.AddScoped<IUserApplicationNotificationSettingsRepository, UserApplicationNotificationSettingsRepository>();
            services.AddScoped<INotificationDeliveryRepository, NotificationDeliveryRepository>();
            services.AddScoped<IEmailNotificationQueueRepository, EmailNotificationQueueRepository>();
            services.AddScoped<IEmailTemplateRepository, EmailTemplateRepository>();
            #endregion

            #region Automation Services
            // Tools request services
            services.AddScoped<IToolsRequestService, ToolsRequestService>();
            services.AddScoped<IRequestStatusCodeRepository, RequestStatusCodeRepository>();

            // API communication services
            services.AddSingleton<APIServices>();
            services.AddHttpClient<APIServices>();

            // Notification services
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IUserNotificationSettingsService, UserNotificationSettingsService>();
            services.AddScoped<IApplicationNotificationProfileService, ApplicationNotificationProfileService>();
            services.AddScoped<IEmailTemplateService, EmailTemplateService>();
            services.AddScoped<IEmailNotificationService, EmailNotificationService>();
            #endregion

            return services;
        }

        /// <summary>
        /// Configures standard middleware components required by Automation API applications.
        /// </summary>
        /// <param name="app">The WebApplication to configure.</param>
        /// <param name="config">The configuration object.</param>
        /// <returns>The configured WebApplication.</returns>
        public static WebApplication ConfigureStandardAPIMiddleware(this WebApplication app, IConfiguration config)
        {
            #region Middleware Configuration
            // Set the Base HREF from AppSettings.JSON Property
            var appConfiguration = app.Services.GetRequiredService<IAppConfiguration>();
            var baseUrl = appConfiguration.BaseUrl;
            app.UsePathBase(baseUrl);

            // Configure environment-specific error handling
            if (app.Environment.IsDevelopment())
            {
                // Note: Swagger setup should be done in the client application
                // as it requires additional NuGet packages not included in RCL
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error");
                app.UseHsts();
            }

            // --- Begin Core Middleware Pipeline ---

            // HTTPS redirection and basic static files
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            // CORS policy (API-specific)
            app.UseCors("AllowLocalhost");

            // --- Authentication/Authorization Pipeline ---
            // Important: UseRouting must come before UseAuthentication and UseAuthorization
            app.UseRouting();

            // Session and authentication middleware (must be after UseRouting)
            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();

            // --- Endpoint Mapping ---

            // Map API controllers
            app.MapControllers();

            // Map SignalR notification hub
            app.MapHub<NotificationHub>("notificationHub");
            #endregion

            return app;
        }
    }
}