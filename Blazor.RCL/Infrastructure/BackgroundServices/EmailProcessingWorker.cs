using Blazor.RCL.Application.Interfaces;
using Blazor.RCL.NLog.LogService.Interface;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Blazor.RCL.Infrastructure.BackgroundServices
{
    /// <summary>
    /// Configuration options for the EmailProcessingWorker
    /// </summary>
    public class EmailProcessingOptions
    {
        /// <summary>
        /// Gets or sets the interval for processing the email notification queue
        /// </summary>
        public TimeSpan ProcessingInterval { get; set; } = TimeSpan.FromMinutes(1);
        
        /// <summary>
        /// Gets or sets the batch size for processing emails
        /// </summary>
        public int BatchSize { get; set; } = 50;
        
        /// <summary>
        /// Gets or sets whether to process scheduled digests
        /// </summary>
        public bool ProcessScheduledDigests { get; set; } = true;
        
        /// <summary>
        /// Gets or sets the interval for processing scheduled digests
        /// </summary>
        public TimeSpan DigestProcessingInterval { get; set; } = TimeSpan.FromHours(1);
    }

    /// <summary>
    /// Background worker dedicated to processing email notifications
    /// </summary>
    public class EmailProcessingWorker : BackgroundService
    {
        private readonly ILogHelper _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _processingInterval;
        private readonly int _batchSize;
        private readonly bool _processScheduledDigests;
        private readonly TimeSpan _digestProcessingInterval;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="EmailProcessingWorker"/> class
        /// </summary>
        public EmailProcessingWorker(
            ILogHelper logger,
            IServiceProvider serviceProvider,
            IOptions<EmailProcessingOptions> options)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _processingInterval = options?.Value?.ProcessingInterval ?? TimeSpan.FromMinutes(1);
            _batchSize = options?.Value?.BatchSize ?? 50;
            _processScheduledDigests = options?.Value?.ProcessScheduledDigests ?? true;
            _digestProcessingInterval = options?.Value?.DigestProcessingInterval ?? TimeSpan.FromHours(1);
        }
        
        /// <inheritdoc/>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogMessage("Email Processing Worker started");
            
            DateTime lastDigestProcessing = DateTime.MinValue;
            
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var emailService = scope.ServiceProvider.GetService<IEmailNotificationService>();
                        
                        if (emailService == null)
                        {
                            _logger.LogWarn("IEmailNotificationService not registered. Email processing worker shutting down.", null);
                            break;
                        }
                        
                        // Process regular email queue
                        try
                        {
                            var processedCount = await emailService.ProcessEmailQueueAsync(_batchSize);
                            
                            if (processedCount > 0)
                            {
                                _logger.LogMessage($"Processed {processedCount} email(s) from the queue");
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError("Error processing email queue", ex);
                        }
                        
                        // Process scheduled digests if enabled and it's time
                        if (_processScheduledDigests && 
                            (DateTime.UtcNow - lastDigestProcessing) >= _digestProcessingInterval)
                        {
                            try
                            {
                                var digestCount = await emailService.ProcessScheduledDigestsAsync();
                                
                                if (digestCount > 0)
                                {
                                    _logger.LogMessage($"Processed {digestCount} scheduled digest email(s)");
                                }
                                
                                lastDigestProcessing = DateTime.UtcNow;
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError("Error processing scheduled digests", ex);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError("Unhandled exception in EmailProcessingWorker", ex);
                }
                
                // Wait for the next processing interval
                try
                {
                    await Task.Delay(_processingInterval, stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    // This is expected when cancellation is requested
                    break;
                }
            }
            
            _logger.LogMessage("Email Processing Worker stopped");
        }
    }
}