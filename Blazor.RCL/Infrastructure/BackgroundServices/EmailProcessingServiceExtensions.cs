using Microsoft.Extensions.DependencyInjection;
using System;

namespace Blazor.RCL.Infrastructure.BackgroundServices
{
    /// <summary>
    /// Extension methods for registering email processing services
    /// </summary>
    public static class EmailProcessingServiceExtensions
    {
        /// <summary>
        /// Adds the email processing background service to the service collection
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="configureOptions">Optional action to configure options</param>
        /// <returns>The service collection for chaining</returns>
        public static IServiceCollection AddEmailProcessingService(
            this IServiceCollection services,
            Action<EmailProcessingOptions> configureOptions = null)
        {
            // Register the options
            if (configureOptions != null)
            {
                services.Configure(configureOptions);
            }
            else
            {
                // Default configuration
                services.Configure<EmailProcessingOptions>(options =>
                {
                    options.ProcessingInterval = TimeSpan.FromMinutes(1);
                    options.BatchSize = 50;
                    options.ProcessScheduledDigests = true;
                    options.DigestProcessingInterval = TimeSpan.FromHours(1);
                });
            }
            
            // Register the hosted service
            services.AddHostedService<EmailProcessingWorker>();
            
            return services;
        }
    }
}