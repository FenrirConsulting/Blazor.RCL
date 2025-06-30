using Blazor.RCL.Infrastructure.BackgroundServices.Interfaces;
using Blazor.RCL.NLog.LogService.Interface;
using Blazor.RCL.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Blazor.RCL.Infrastructure.BackgroundServices
{
    /// <summary>
    /// Configuration options for the RequestProcessingWorker
    /// </summary>
    public class RequestProcessingOptions
    {
        /// <summary>
        /// Gets or sets the polling interval for the main worker cycle
        /// </summary>
        public TimeSpan PollingInterval { get; set; } = TimeSpan.FromMinutes(1);
        
        /// <summary>
        /// Gets or sets the interval for checking the external RequestLog table when processing requests
        /// This interval should typically be longer than the main polling interval
        /// </summary>
        public TimeSpan RequestLogCheckInterval { get; set; } = TimeSpan.FromMinutes(2);
        
        /// <summary>
        /// Gets or sets the interval for processing the email notification queue
        /// </summary>
        public TimeSpan EmailProcessingInterval { get; set; } = TimeSpan.FromMinutes(1);
        
        /// <summary>
        /// Gets or sets whether email processing is enabled
        /// </summary>
        public bool EnableEmailProcessing { get; set; } = true;
    }

    /// <summary>
    /// Background worker that polls for request status and updates the database
    /// </summary>
    public class RequestProcessingWorker : BackgroundService
    {
        private readonly ILogHelper _logger;
        private readonly IRequestPollingService _requestPollingService;
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _pollingInterval;
        private readonly TimeSpan _requestLogCheckInterval;
        private readonly TimeSpan _emailProcessingInterval;
        private readonly bool _enableEmailProcessing;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="RequestProcessingWorker"/> class
        /// </summary>
        public RequestProcessingWorker(
            ILogHelper logger,
            IRequestPollingService requestPollingService,
            IServiceProvider serviceProvider,
            IOptions<RequestProcessingOptions> options)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _requestPollingService = requestPollingService ?? throw new ArgumentNullException(nameof(requestPollingService));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _pollingInterval = options?.Value?.PollingInterval ?? TimeSpan.FromMinutes(5);
            _requestLogCheckInterval = options?.Value?.RequestLogCheckInterval ?? TimeSpan.FromMinutes(15);
            _emailProcessingInterval = options?.Value?.EmailProcessingInterval ?? TimeSpan.FromMinutes(1);
            _enableEmailProcessing = options?.Value?.EnableEmailProcessing ?? true;
        }
        
        /// <inheritdoc/>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Track when we last checked the RequestLog table and email queue
            DateTime lastRequestLogCheck = DateTime.MinValue;
            DateTime lastEmailProcessing = DateTime.MinValue;
            
            while (!stoppingToken.IsCancellationRequested)
            {
                var tasks = new List<Task>();
                
                try
                {
                    // Determine if we should check the RequestLog table in this cycle
                    bool shouldCheckRequestLog = (DateTime.UtcNow - lastRequestLogCheck) >= _requestLogCheckInterval;
                    
                    // Create task for request processing
                    var requestTask = Task.Run(async () =>
                    {
                        try
                        {
                            await _requestPollingService.PollAndProcessRequestsAsync(shouldCheckRequestLog, stoppingToken);
                            
                            // Update the last check time if we checked the RequestLog
                            if (shouldCheckRequestLog)
                            {
                                lastRequestLogCheck = DateTime.UtcNow;
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError("Error processing requests", ex);
                        }
                    }, stoppingToken);
                    tasks.Add(requestTask);
                    
                    // Process email queue if enabled and it's time
                    if (_enableEmailProcessing && (DateTime.UtcNow - lastEmailProcessing) >= _emailProcessingInterval)
                    {
                        var emailTask = Task.Run(async () =>
                        {
                            try
                            {
                                await ProcessEmailQueueAsync(stoppingToken);
                                lastEmailProcessing = DateTime.UtcNow;
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError("Error processing email queue", ex);
                            }
                        }, stoppingToken);
                        tasks.Add(emailTask);
                    }
                    
                    // Wait for all tasks to complete
                    await Task.WhenAll(tasks);
                }
                catch (Exception ex)
                {
                    _logger.LogError("Unhandled exception in RequestProcessingWorker", ex);
                }
                
                // Wait for the next polling interval
                try
                {
                    await Task.Delay(_pollingInterval, stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    // This is expected when cancellation is requested
                    break;
                }
            }
        }
        
        private async Task ProcessEmailQueueAsync(CancellationToken cancellationToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var emailService = scope.ServiceProvider.GetService<IEmailNotificationService>();
                
                if (emailService == null)
                {
                    _logger.LogWarn("IEmailNotificationService not registered. Email processing skipped.", null);
                    return;
                }
                
                _logger.LogMessage("Processing email notification queue");
                
                try
                {
                    // Process up to 50 emails per batch
                    var processedCount = await emailService.ProcessEmailQueueAsync(50);
                }
                catch (Exception ex)
                {
                    _logger.LogError("Error in email queue processing", ex);
                }
            }
        }
    }
}
