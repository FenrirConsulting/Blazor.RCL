using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Blazor.RCL.Application.Common.Configuration.Interfaces;
using Blazor.RCL.Application.Interfaces;
using Blazor.RCL.Application.Models.Notifications;
using Blazor.RCL.Domain.Entities.Notifications;
using Blazor.RCL.NLog.LogService.Interface;
using StackExchange.Redis;

namespace Blazor.RCL.Infrastructure.Services
{
    /// <summary>
    /// Service implementation for managing email notification delivery
    /// </summary>
    public class EmailNotificationService : IEmailNotificationService
    {
        private readonly IEmailNotificationQueueRepository _emailQueueRepository;
        private readonly IUserNotificationSettingsRepository _userSettingsRepository;
        private readonly IApplicationNotificationProfileRepository _appProfileRepository;
        private readonly INotificationDeliveryRepository _deliveryRepository;
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly IAppConfiguration _appConfig;
        private readonly ILogHelper _logger;
        private readonly IConnectionMultiplexer? _redisConnection;

        public EmailNotificationService(
            IEmailNotificationQueueRepository emailQueueRepository,
            IUserNotificationSettingsRepository userSettingsRepository,
            IApplicationNotificationProfileRepository appProfileRepository,
            INotificationDeliveryRepository deliveryRepository,
            IEmailTemplateService emailTemplateService,
            IAppConfiguration appConfig,
            ILogHelper logger,
            IConnectionMultiplexer? redisConnection = null)
        {
            _emailQueueRepository = emailQueueRepository;
            _userSettingsRepository = userSettingsRepository;
            _appProfileRepository = appProfileRepository;
            _deliveryRepository = deliveryRepository;
            _emailTemplateService = emailTemplateService;
            _appConfig = appConfig;
            _logger = logger;
            _redisConnection = redisConnection;
        }

        public async Task<EmailNotificationQueue> QueueEmailNotificationAsync(
            NotificationMessage notification, 
            string username,
            EmailPriority priority = EmailPriority.Normal)
        {
            try
            {
                var userSettings = await _userSettingsRepository.GetByUsernameAsync(username);
                if (userSettings == null || !userSettings.EnableEmailNotifications)
                {
                    throw new InvalidOperationException($"User {username} has email notifications disabled");
                }

                // Build email content
                var (subject, htmlBody, textBody) = await BuildEmailContentAsync(notification, username);

                // Calculate scheduled send time based on user's email frequency
                var scheduledTime = CalculateScheduledSendTime((EmailFrequency)userSettings.GlobalEmailFrequency);

                var emailQueue = new EmailNotificationQueue
                {
                    Id = Guid.NewGuid(),
                    NotificationId = notification.Id,
                    Username = username,
                    EmailAddress = userSettings.EmailAddress,
                    Subject = subject,
                    HtmlBody = htmlBody,
                    TextBody = textBody,
                    Priority = (int)priority,
                    Status = (int)EmailStatus.Pending,
                    ScheduledSendTime = scheduledTime,
                    CreatedAt = DateTime.UtcNow,
                    RetryCount = 0
                };

                await _emailQueueRepository.CreateAsync(emailQueue);

                _logger.LogInfo("Email notification queued", "EmailQueued", new { EmailId = emailQueue.Id, Username = username });

                return emailQueue;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error queueing email notification", ex);
                throw;
            }
        }

        public async Task<IEnumerable<EmailNotificationQueue>> QueueEmailNotificationBatchAsync(
            NotificationMessage notification,
            IEnumerable<string> usernames,
            EmailPriority priority = EmailPriority.Normal)
        {
            var queuedEmails = new List<EmailNotificationQueue>();

            foreach (var username in usernames.Distinct())
            {
                try
                {
                    var email = await QueueEmailNotificationAsync(notification, username, priority);
                    queuedEmails.Add(email);
                }
                catch (Exception ex)
                {
                    _logger.LogWarn("Failed to queue email for user", username, ex.Message);
                }
            }

            return queuedEmails;
        }

        public async Task<int> ProcessEmailQueueAsync(int batchSize = 50)
        {
            var instanceId = $"{Environment.MachineName}_{Process.GetCurrentProcess().Id}";
            
            try
            {
                // Optional: Try Redis lock first when available
                if (_redisConnection?.IsConnected == true)
                {
                    var db = _redisConnection.GetDatabase();
                    var lockKey = "email-processing-lock";
                    var lockToken = instanceId;
                    
                    // Try to acquire lock (5 minute expiry)
                    if (!await db.StringSetAsync(lockKey, lockToken, TimeSpan.FromMinutes(5), When.NotExists))
                    {
                        _logger.LogDebug("Another instance is processing emails", "EmailProcessingSkipped");
                        return 0;
                    }
                    
                    try
                    {
                        return await ProcessEmailsWithClaimAsync(instanceId, batchSize);
                    }
                    finally
                    {
                        // Release lock if we still own it
                        var script = @"
                            if redis.call('get', KEYS[1]) == ARGV[1] then
                                return redis.call('del', KEYS[1])
                            else
                                return 0
                            end";
                        await db.ScriptEvaluateAsync(script, new RedisKey[] { lockKey }, new RedisValue[] { lockToken });
                    }
                }
                else
                {
                    // No Redis - rely on atomic claims only
                    return await ProcessEmailsWithClaimAsync(instanceId, batchSize);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing email queue for instance: {instanceId}", ex);
                throw;
            }
        }

        private async Task<int> ProcessEmailsWithClaimAsync(string instanceId, int batchSize)
        {
            try
            {
                // Atomic claim prevents duplicate processing
                var claimedEmails = await _emailQueueRepository.ClaimPendingEmailsAsync(
                    instanceId, batchSize, CancellationToken.None);
                
                _logger.LogInfo($"Claimed {claimedEmails.Count} emails for processing", "EmailsClaimed", 
                    new { Instance = instanceId, ClaimedCount = claimedEmails.Count });
                
                if (!claimedEmails.Any())
                    return 0;

                var processedCount = 0;
                using var smtpClient = await CreateSmtpClientAsync();

                foreach (var email in claimedEmails)
                {
                    try
                    {
                        await SendEmailAsync(smtpClient, email);
                        
                        await _emailQueueRepository.MarkAsSentAsync(email.Id, DateTime.UtcNow);
                        
                        // Update notification delivery status
                        var deliveries = await _deliveryRepository.GetByNotificationAndUserAsync(
                            email.NotificationId, 
                            email.Username);
                        
                        var emailDelivery = deliveries.FirstOrDefault(d => d.DeliveryChannel == (int)DeliveryChannel.Email);
                        if (emailDelivery != null)
                        {
                            await _deliveryRepository.UpdateDeliveryStatusAsync(emailDelivery.Id, DeliveryStatus.Delivered);
                        }

                        processedCount++;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Failed to send email {email.Id}", ex);
                        await _emailQueueRepository.MarkAsFailedAsync(email.Id, ex.Message);
                    }
                }
                
                _logger.LogInfo("Processed email batch", "EmailBatchProcessed", 
                    new { Instance = instanceId, ProcessedCount = processedCount });
                
                return processedCount;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing claimed emails for instance: {instanceId}", ex);
                throw;
            }
        }

        public async Task<bool> SendEmailImmediatelyAsync(SendEmailRequest request)
        {
            try
            {
                using var smtpClient = await CreateSmtpClientAsync();
                using var message = CreateMailMessage(request);
                
                await smtpClient.SendMailAsync(message);
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to send immediate email", ex);
                return false;
            }
        }

        public async Task<EmailQueueStatistics> GetEmailQueueStatisticsAsync()
        {
            try
            {
                var statsDict = await _emailQueueRepository.GetQueueStatisticsAsync();
                
                var statistics = new EmailQueueStatistics
                {
                    PendingCount = statsDict.GetValueOrDefault("Pending", 0),
                    ProcessingCount = statsDict.GetValueOrDefault("Processing", 0),
                    FailedCount = statsDict.GetValueOrDefault("Failed", 0),
                    TotalInQueue = statsDict.GetValueOrDefault("Pending", 0) + 
                                   statsDict.GetValueOrDefault("Processing", 0) + 
                                   statsDict.GetValueOrDefault("Failed", 0),
                    StatusBreakdown = new Dictionary<EmailStatus, int>
                    {
                        [EmailStatus.Pending] = statsDict.GetValueOrDefault("Pending", 0),
                        [EmailStatus.Processing] = statsDict.GetValueOrDefault("Processing", 0),
                        [EmailStatus.Sent] = statsDict.GetValueOrDefault("Sent", 0),
                        [EmailStatus.Failed] = statsDict.GetValueOrDefault("Failed", 0)
                    }
                };
                
                return statistics;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error getting email queue statistics", ex);
                throw;
            }
        }

        public async Task<int> RetryFailedEmailsAsync(int maxRetries = 3, int hoursOld = 1)
        {
            try
            {
                var cutoffTime = DateTime.UtcNow.AddHours(-hoursOld);
                var failedEmails = await _emailQueueRepository.GetFailedEmailsAsync(maxRetries);
                
                var retriedCount = 0;
                foreach (var email in failedEmails.Where(e => e.CreatedAt < cutoffTime))
                {
                    email.Status = (int)EmailStatus.Pending;
                    email.RetryCount++;
                    email.FailureReason = null;
                    
                    await _emailQueueRepository.UpdateAsync(email);
                    retriedCount++;
                }

                return retriedCount;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error retrying failed emails", ex);
                throw;
            }
        }

        public async Task<int> CleanupOldEmailsAsync(int daysToKeep = 7)
        {
            try
            {
                var cutoffDate = DateTime.UtcNow.AddDays(-daysToKeep);
                var deletedCount = await _emailQueueRepository.DeleteOldEmailsAsync(cutoffDate);
                
                _logger.LogInfo("Cleaned up old emails", "EmailCleanup", new { DeletedCount = deletedCount });
                
                return deletedCount;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error cleaning up old emails", ex);
                throw;
            }
        }

        public async Task<IEnumerable<EmailNotificationQueue>> GetUserEmailHistoryAsync(string username, int days = 7)
        {
            try
            {
                var sinceDate = DateTime.UtcNow.AddDays(-days);
                return await _emailQueueRepository.GetUserEmailsAsync(username, sinceDate);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error getting user email history", ex);
                throw;
            }
        }

        public async Task<EmailConfigurationValidation> ValidateEmailConfigurationAsync()
        {
            var validation = new EmailConfigurationValidation
            {
                SmtpStatus = new SmtpServerStatus
                {
                    Host = _appConfig.SmtpSettings?.Host ?? string.Empty,
                    Port = _appConfig.SmtpSettings?.Port ?? 587,
                    UseSsl = _appConfig.SmtpSettings?.EnableSsl ?? true,
                    FromEmail = _appConfig.SmtpSettings?.FromEmail ?? string.Empty,
                    FromDisplayName = _appConfig.SmtpSettings?.FromDisplayName ?? "Automation Notifications"
                }
            };

            try
            {
                // Validate SMTP configuration
                if (string.IsNullOrEmpty(validation.SmtpStatus.Host))
                {
                    validation.Issues.Add("SMTP host is not configured");
                }

                if (string.IsNullOrEmpty(validation.SmtpStatus.FromEmail))
                {
                    validation.Issues.Add("From email address is not configured");
                }

                // Check if credentials are configured
                if (_appConfig.SmtpSettings?.UseAuthentication == true)
                {
                    validation.SmtpStatus.AuthenticationConfigured = true;
                    
                    if (string.IsNullOrEmpty(_appConfig.SmtpSettings.Username))
                    {
                        validation.Issues.Add("SMTP username is not configured");
                    }
                }

                // Test SMTP connection
                if (validation.Issues.Count == 0)
                {
                    try
                    {
                        using var smtpClient = await CreateSmtpClientAsync();
                        validation.SmtpStatus.IsReachable = true;

                        // Try to send a test email
                        if (!string.IsNullOrEmpty(_appConfig.SmtpSettings?.TestEmailRecipient))
                        {
                            var testRequest = new SendEmailRequest
                            {
                                ToEmail = _appConfig.SmtpSettings.TestEmailRecipient,
                                Subject = "Automation Notification System Test",
                                HtmlBody = "<p>This is a test email from the Automation Notification System.</p>",
                                TextBody = "This is a test email from the Automation Notification System."
                            };

                            validation.TestEmailSent = await SendEmailImmediatelyAsync(testRequest);
                            if (!validation.TestEmailSent)
                            {
                                validation.TestEmailError = "Failed to send test email";
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        validation.SmtpStatus.IsReachable = false;
                        validation.Issues.Add($"SMTP connection failed: {ex.Message}");
                    }
                }

                validation.IsValid = validation.Issues.Count == 0;
                
                if (validation.IsValid)
                {
                    _logger.LogInfo("Email configuration validated successfully", "EmailConfigValidated", validation);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error validating email configuration", ex);
                validation.Issues.Add($"Validation error: {ex.Message}");
                validation.IsValid = false;
            }

            return validation;
        }

        public async Task<int> ProcessScheduledDigestsAsync()
        {
            try
            {
                var processedCount = 0;
                
                // Get users with hourly/daily digest preferences
                var digestUsers = await GetUsersForDigestAsync();
                
                foreach (var (username, frequency) in digestUsers)
                {
                    try
                    {
                        var notifications = await GetPendingNotificationsForDigestAsync(username, frequency);
                        if (notifications.Any())
                        {
                            await SendDigestEmailAsync(username, notifications);
                            processedCount++;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("Failed to process digest for user", ex);
                    }
                }
                
                _logger.LogInfo("Processed scheduled digests", "DigestsProcessed", new { ProcessedCount = processedCount });

                return processedCount;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error processing scheduled digests", ex);
                throw;
            }
        }

        private async Task<SmtpClient> CreateSmtpClientAsync()
        {
            var smtpSettings = _appConfig.SmtpSettings;
            if (smtpSettings == null)
            {
                throw new InvalidOperationException("SMTP settings not configured");
            }

            var smtpClient = new SmtpClient
            {
                Host = smtpSettings.Host,
                Port = smtpSettings.Port,
                EnableSsl = smtpSettings.EnableSsl,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Timeout = smtpSettings.Timeout * 1000 // Convert to milliseconds
            };

            if (smtpSettings.UseAuthentication)
            {
                // Get password from RegistryValues (loaded from AKeyless or Registry)
                if (_appConfig.RegistryValues.TryGetValue("SmtpPassword", out var smtpPassword))
                {
                    smtpClient.Credentials = new NetworkCredential(smtpSettings.Username, smtpPassword);
                }
                else
                {
                    throw new InvalidOperationException("SMTP authentication enabled but password not available");
                }
            }

            return await Task.FromResult(smtpClient);
        }

        private async Task SendEmailAsync(SmtpClient smtpClient, EmailNotificationQueue email)
        {
            using var message = new MailMessage
            {
                From = new MailAddress(
                    _appConfig.SmtpSettings!.FromEmail, 
                    _appConfig.SmtpSettings.FromDisplayName),
                Subject = email.Subject,
                Priority = email.Priority == (int)EmailPriority.High ? MailPriority.High : MailPriority.Normal
            };

            message.To.Add(new MailAddress(email.EmailAddress, email.Username));
            
            // When using AlternateViews, we must add both HTML and text views
            // The Body property is ignored when AlternateViews are present
            if (!string.IsNullOrEmpty(email.TextBody))
            {
                // Add plain text view
                message.AlternateViews.Add(
                    AlternateView.CreateAlternateViewFromString(
                        email.TextBody, 
                        Encoding.UTF8, 
                        "text/plain"));
                
                // Add HTML view
                message.AlternateViews.Add(
                    AlternateView.CreateAlternateViewFromString(
                        email.HtmlBody, 
                        Encoding.UTF8, 
                        "text/html"));
            }
            else
            {
                // If no text body, just set the HTML body directly
                message.IsBodyHtml = true;
                message.Body = email.HtmlBody;
            }

            await smtpClient.SendMailAsync(message);
        }

        private MailMessage CreateMailMessage(SendEmailRequest request)
        {
            var message = new MailMessage
            {
                From = new MailAddress(
                    _appConfig.SmtpSettings!.FromEmail,
                    _appConfig.SmtpSettings.FromDisplayName),
                Subject = request.Subject,
                Priority = request.IsHighPriority ? MailPriority.High : MailPriority.Normal
            };

            message.To.Add(request.ToEmail);
            
            request.CcEmails?.ForEach(cc => message.CC.Add(cc));
            request.BccEmails?.ForEach(bcc => message.Bcc.Add(bcc));
            
            if (!string.IsNullOrEmpty(request.ReplyToEmail))
            {
                message.ReplyToList.Add(request.ReplyToEmail);
            }

            // When using AlternateViews, we must add both HTML and text views
            // The Body property is ignored when AlternateViews are present
            if (!string.IsNullOrEmpty(request.TextBody))
            {
                // Add plain text view
                message.AlternateViews.Add(
                    AlternateView.CreateAlternateViewFromString(
                        request.TextBody, 
                        Encoding.UTF8, 
                        "text/plain"));
                
                // Add HTML view
                message.AlternateViews.Add(
                    AlternateView.CreateAlternateViewFromString(
                        request.HtmlBody, 
                        Encoding.UTF8, 
                        "text/html"));
            }
            else
            {
                // If no text body, just set the HTML body directly
                message.IsBodyHtml = true;
                message.Body = request.HtmlBody;
            }

            if (request.CustomHeaders != null)
            {
                foreach (var (key, value) in request.CustomHeaders)
                {
                    message.Headers.Add(key, value);
                }
            }

            return message;
        }

        private async Task<(string subject, string htmlBody, string textBody)> BuildEmailContentAsync(
            NotificationMessage notification, 
            string username)
        {
            try
            {
                // Get user notification settings for email context
                var userNotificationSettings = await _userSettingsRepository.GetByUsernameAsync(username);
                var appProfile = await _appProfileRepository.GetByNameAsync(notification.SourceApplication);
                
                // Build additional variables for template
                var additionalVariables = new Dictionary<string, object>
                {
                    ["Username"] = username,
                    ["UserDisplayName"] = username, // Use username as display name since it's not available
                    ["UserEmail"] = userNotificationSettings?.EmailAddress ?? string.Empty,
                    ["Environment"] = _appConfig.EnvironmentLoaded,
                    ["ApplicationDisplayName"] = appProfile?.DisplayName ?? notification.SourceApplication,
                    ["Timestamp"] = DateTime.UtcNow
                };
                
                // Use template service to render the email
                var rendered = await _emailTemplateService.RenderNotificationEmailAsync(
                    notification, 
                    additionalVariables);
                    
                return (rendered.Subject, rendered.HtmlBody, rendered.TextBody);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to render email template for notification {notification.Id}", ex);
                
                // Fallback to hardcoded template if template rendering fails
                return await BuildFallbackEmailContentAsync(notification, username);
            }
        }

        private async Task<(string subject, string htmlBody, string textBody)> BuildFallbackEmailContentAsync(
            NotificationMessage notification, 
            string username)
        {
            var appProfile = await _appProfileRepository.GetByNameAsync(notification.SourceApplication);
            var appName = appProfile?.DisplayName ?? notification.SourceApplication;

            var subject = $"[{appName}] {notification.Title}";
            
            var htmlBody = $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <div style='background-color: #f5f5f5; padding: 20px;'>
                        <h2 style='color: #333;'>{notification.Title}</h2>
                        <div style='background-color: white; padding: 15px; border-radius: 5px; margin-top: 10px;'>
                            <p><strong>Application:</strong> {appName}</p>
                            <p><strong>Severity:</strong> {notification.Severity}</p>
                            <p><strong>Type:</strong> {notification.AlertType}</p>
                            <p><strong>Time:</strong> {notification.CreatedAt:yyyy-MM-dd HH:mm:ss} UTC</p>
                            <hr/>
                            <div>{notification.Content}</div>
                        </div>
                        <p style='font-size: 12px; color: #666; margin-top: 20px;'>
                            This is an automated notification from the Automation Notification System.
                        </p>
                    </div>
                </body>
                </html>";

            var textBody = $@"{notification.Title}

Application: {appName}
Severity: {notification.Severity}
Type: {notification.AlertType}
Time: {notification.CreatedAt:yyyy-MM-dd HH:mm:ss} UTC

{notification.Content}

This is an automated notification from the Automation Notification System.";

            return (subject, htmlBody, textBody);
        }

        private DateTime CalculateScheduledSendTime(EmailFrequency frequency)
        {
            var now = DateTime.UtcNow;
            
            return frequency switch
            {
                EmailFrequency.Immediate => now,
                EmailFrequency.Hourly => now.AddHours(1).Date.AddHours(now.Hour + 1),
                EmailFrequency.Daily => now.AddDays(1).Date.AddHours(8), // 8 AM next day
                _ => now
            };
        }

        private async Task<List<(string username, EmailFrequency frequency)>> GetUsersForDigestAsync()
        {
            // This would query users with digest preferences
            // For now, returning empty list
            return new List<(string, EmailFrequency)>();
        }

        private async Task<List<NotificationMessage>> GetPendingNotificationsForDigestAsync(
            string username, 
            EmailFrequency frequency)
        {
            // This would get pending notifications for digest
            // For now, returning empty list
            return new List<NotificationMessage>();
        }

        private async Task SendDigestEmailAsync(string username, List<NotificationMessage> notifications)
        {
            // This would send a digest email with multiple notifications
            // Implementation would format all notifications into a single email
            await Task.CompletedTask;
        }
    }
}