using Blazor.RCL.Infrastructure.BackgroundServices.Interfaces;
using Blazor.RCL.Automation.AutomationRequest.Interfaces;
using Blazor.RCL.Automation.AutomationRequest.Repositories;
using Blazor.RCL.Automation.AutomationDirectory.AutomationDirectoryRepositories.Interfaces;
using Blazor.RCL.Automation.AutomationDirectory.AutomationDirectoryRepositories;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Blazor.RCL.Infrastructure.BackgroundServices
{
    /// <summary>
    /// Extension methods for registering request processing services
    /// </summary>
    public static class RequestProcessingServiceExtensions
    {
        /// <summary>
        /// Adds request processing services to the service collection
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="configureOptions">Optional action to configure options</param>
        /// <returns>The service collection for chaining</returns>
        public static IServiceCollection AddRequestProcessingServices(
            this IServiceCollection services,
            Action<RequestProcessingOptions> configureOptions = null)
        {
            // Register the options
            if (configureOptions != null)
            {
                services.Configure(configureOptions);
            }
            else
            {
                services.Configure<RequestProcessingOptions>(options =>
                {
                    options.PollingInterval = TimeSpan.FromMinutes(1);
                    options.RequestLogCheckInterval = TimeSpan.FromMinutes(2);
                });
            }
            
            // Register the repositories and services as singletons for the background worker
            services.AddSingleton<IRequestLogRepository, RequestLogRepository>();
            services.AddScoped<IADADRequestRepository, ADADRequestRepository>();
            services.AddSingleton<BackgroundRequestService>();
            services.AddSingleton<IRequestPollingService, RequestPollingService>();
            
            // Register the hosted service
            services.AddHostedService<RequestProcessingWorker>();
            
            return services;
        }
    }
}
