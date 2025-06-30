using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using StackExchange.Redis;
using Blazor.RCL.Application.Interfaces;
using Blazor.RCL.Domain.Entities.Notifications;
using Blazor.RCL.Infrastructure.Hubs;
using Blazor.RCL.NLog.LogService.Interface;

namespace Blazor.RCL.Infrastructure.Services
{
    /// <summary>
    /// Redis-based notification publisher for real-time cross-server message delivery.
    /// Uses Redis pub/sub to distribute notifications across all application instances.
    /// </summary>
    public class RedisNotificationPublisher : INotificationPublisher
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly ILogHelper _logger;
        private readonly string _channelPrefix;
        private DateTime? _lastSuccessfulOperation;
        private string? _lastError;

        public RedisNotificationPublisher(
            IConnectionMultiplexer redis,
            IHubContext<NotificationHub> hubContext,
            ILogHelper logger,
            string channelPrefix = "AutomationNotifications")
        {
            _redis = redis ?? throw new ArgumentNullException(nameof(redis));
            _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _channelPrefix = channelPrefix;

            // Subscribe to Redis channels for incoming notifications
            SubscribeToRedisChannels();
        }

        /// <inheritdoc/>
        public bool IsRealTimeAvailable => _redis?.IsConnected ?? false;

        /// <inheritdoc/>
        public async Task PublishAsync(NotificationMessage notification, IEnumerable<string> targetUsers)
        {
            if (!IsRealTimeAvailable)
            {
                return;
            }

            try
            {
                var subscriber = _redis.GetSubscriber();
                var channel = $"{_channelPrefix}:users";
                
                var message = new
                {
                    Type = "UserNotification",
                    Notification = notification,
                    TargetUsers = targetUsers.ToList(),
                    Timestamp = DateTime.UtcNow,
                    SourceServer = Environment.MachineName
                };

                var json = JsonSerializer.Serialize(message);
                await subscriber.PublishAsync(channel, json);

                _lastSuccessfulOperation = DateTime.UtcNow;
                _logger.LogInfo($"Published notification {notification.MessageId} to {targetUsers.Count()} users via Redis", 
                    "NotificationPublished", 
                    new { NotificationId = notification.Id, UserCount = targetUsers.Count() });
            }
            catch (Exception ex)
            {
                _lastError = ex.Message;
                _logger.LogError($"Failed to publish notification to Redis: {ex.Message}", ex);
                // Don't throw - gracefully degrade to polling mode
            }
        }

        /// <inheritdoc/>
        public async Task PublishToApplicationAsync(NotificationMessage notification, string applicationName)
        {
            if (!IsRealTimeAvailable)
            {
                return;
            }

            try
            {
                var subscriber = _redis.GetSubscriber();
                var channel = $"{_channelPrefix}:apps";
                
                var message = new
                {
                    Type = "ApplicationNotification",
                    Notification = notification,
                    ApplicationName = applicationName,
                    Timestamp = DateTime.UtcNow,
                    SourceServer = Environment.MachineName
                };

                var json = JsonSerializer.Serialize(message);
                await subscriber.PublishAsync(channel, json);

                _lastSuccessfulOperation = DateTime.UtcNow;
                _logger.LogInfo($"Published notification {notification.MessageId} to application {applicationName} via Redis", 
                    "ApplicationNotificationPublished", 
                    new { NotificationId = notification.Id, ApplicationName = applicationName });
            }
            catch (Exception ex)
            {
                _lastError = ex.Message;
                _logger.LogError($"Failed to publish application notification to Redis: {ex.Message}", ex);
                // Don't throw - gracefully degrade to polling mode
            }
        }

        /// <inheritdoc/>
        public async Task PublishToRoleAsync(NotificationMessage notification, string role)
        {
            if (!IsRealTimeAvailable)
            {
                return;
            }

            try
            {
                var subscriber = _redis.GetSubscriber();
                var channel = $"{_channelPrefix}:roles";
                
                var message = new
                {
                    Type = "RoleNotification",
                    Notification = notification,
                    Role = role,
                    Timestamp = DateTime.UtcNow,
                    SourceServer = Environment.MachineName
                };

                var json = JsonSerializer.Serialize(message);
                await subscriber.PublishAsync(channel, json);

                _lastSuccessfulOperation = DateTime.UtcNow;
                _logger.LogInfo($"Published notification {notification.MessageId} to role {role} via Redis", 
                    "RoleNotificationPublished", 
                    new { NotificationId = notification.Id, Role = role });
            }
            catch (Exception ex)
            {
                _lastError = ex.Message;
                _logger.LogError($"Failed to publish role notification to Redis: {ex.Message}", ex);
                // Don't throw - gracefully degrade to polling mode
            }
        }

        /// <inheritdoc/>
        public async Task<NotificationPublisherStatus> GetStatusAsync()
        {
            var status = new NotificationPublisherStatus
            {
                Mode = PublisherMode.Redis,
                IsConnected = IsRealTimeAvailable,
                LastError = _lastError,
                LastSuccessfulOperation = _lastSuccessfulOperation,
                Diagnostics = new Dictionary<string, object>()
            };

            if (_redis != null && IsRealTimeAvailable)
            {
                try
                {
                    // Basic connection test without requiring admin mode
                    var db = _redis.GetDatabase();
                    var ping = await db.PingAsync();
                    
                    status.Diagnostics["Connected"] = true;
                    status.Diagnostics["PingLatency"] = $"{ping.TotalMilliseconds:F1}ms";
                    status.Diagnostics["ChannelPrefix"] = _channelPrefix;
                    status.Diagnostics["Endpoints"] = _redis.GetEndPoints()?.Length ?? 0;
                    
                    // Get detailed server info
                    var server = _redis.GetServer(_redis.GetEndPoints().FirstOrDefault());
                    status.Diagnostics["ServerEndpoint"] = server?.EndPoint?.ToString() ?? "Unknown";
                    status.Diagnostics["IsConnected"] = server?.IsConnected ?? false;
                    
                    // Try to get server info if admin mode is enabled
                    if (server != null && server.IsConnected)
                    {
                        try
                        {
                            var info = await server.InfoAsync();
                            if (info != null && info.Any())
                            {
                                // Extract key metrics from INFO command
                                var serverSection = info.FirstOrDefault(g => g.Key == "Server");
                                if (serverSection != null)
                                {
                                    var versionKvp = serverSection.FirstOrDefault(x => x.Key == "redis_version");
                                    status.Diagnostics["RedisVersion"] = !string.IsNullOrEmpty(versionKvp.Value) ? versionKvp.Value : "Unknown";
                                    
                                    var modeKvp = serverSection.FirstOrDefault(x => x.Key == "redis_mode");
                                    status.Diagnostics["RedisMode"] = !string.IsNullOrEmpty(modeKvp.Value) ? modeKvp.Value : "Unknown";
                                }
                                
                                var memorySection = info.FirstOrDefault(g => g.Key == "Memory");
                                if (memorySection != null)
                                {
                                    var memoryKvp = memorySection.FirstOrDefault(x => x.Key == "used_memory_human");
                                    status.Diagnostics["UsedMemory"] = !string.IsNullOrEmpty(memoryKvp.Value) ? memoryKvp.Value : "Unknown";
                                }
                                
                                var clientsSection = info.FirstOrDefault(g => g.Key == "Clients");
                                if (clientsSection != null)
                                {
                                    var clientsKvp = clientsSection.FirstOrDefault(x => x.Key == "connected_clients");
                                    status.Diagnostics["ConnectedClients"] = !string.IsNullOrEmpty(clientsKvp.Value) ? clientsKvp.Value : "Unknown";
                                }
                            }
                        }
                        catch (RedisCommandException rce) when (rce.Message.Contains("ERR unknown command"))
                        {
                            status.Diagnostics["AdminMode"] = "INFO command not available - allowAdmin may not be enabled";
                        }
                        catch (Exception ex)
                        {
                            status.Diagnostics["InfoError"] = $"Unable to retrieve server info: {ex.Message}";
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Failed to get Redis diagnostics: {ex.Message}", ex);
                    status.Diagnostics["Error"] = ex.Message;
                }
            }

            return status;
        }

        /// <summary>
        /// Subscribes to Redis channels to receive notifications from other servers
        /// </summary>
        private void SubscribeToRedisChannels()
        {
            if (!IsRealTimeAvailable)
                return;

            try
            {
                var subscriber = _redis.GetSubscriber();

                // Subscribe to user notifications channel
                subscriber.Subscribe($"{_channelPrefix}:users", async (channel, message) =>
                {
                    await HandleUserNotificationMessage(message);
                });

                // Subscribe to application notifications channel
                subscriber.Subscribe($"{_channelPrefix}:apps", async (channel, message) =>
                {
                    await HandleApplicationNotificationMessage(message);
                });

                // Subscribe to role notifications channel
                subscriber.Subscribe($"{_channelPrefix}:roles", async (channel, message) =>
                {
                    await HandleRoleNotificationMessage(message);
                });

                _logger.LogInfo("Subscribed to Redis notification channels", "RedisSubscribed", 
                    new { ChannelPrefix = _channelPrefix });
            }
            catch (Exception ex)
            {
                _lastError = ex.Message;
                _logger.LogError($"Failed to subscribe to Redis channels: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Handles incoming user notification messages from Redis
        /// </summary>
        private async Task HandleUserNotificationMessage(RedisValue message)
        {
            try
            {
                var json = message.ToString();
                var data = JsonSerializer.Deserialize<UserNotificationMessage>(json);
                
                if (data?.TargetUsers != null && data.Notification != null)
                {
                    // Send to specific users via SignalR
                    foreach (var username in data.TargetUsers)
                    {
                        await _hubContext.Clients.Group($"user-{username}")
                            .SendAsync("ReceiveNotification", data.Notification);
                    }

                    _logger.LogDebug($"Delivered notification {data.Notification.MessageId} to {data.TargetUsers.Count} users", 
                        "NotificationDelivered");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error handling user notification message: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Handles incoming application notification messages from Redis
        /// </summary>
        private async Task HandleApplicationNotificationMessage(RedisValue message)
        {
            try
            {
                var json = message.ToString();
                var data = JsonSerializer.Deserialize<ApplicationNotificationMessage>(json);
                
                if (!string.IsNullOrEmpty(data?.ApplicationName) && data.Notification != null)
                {
                    // Send to all users subscribed to the application
                    await _hubContext.Clients.Group($"app-{data.ApplicationName}")
                        .SendAsync("ReceiveNotification", data.Notification);

                    _logger.LogDebug($"Delivered notification {data.Notification.MessageId} to application {data.ApplicationName}", 
                        "ApplicationNotificationDelivered");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error handling application notification message: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Handles incoming role notification messages from Redis
        /// </summary>
        private async Task HandleRoleNotificationMessage(RedisValue message)
        {
            try
            {
                var json = message.ToString();
                var data = JsonSerializer.Deserialize<RoleNotificationMessage>(json);
                
                if (!string.IsNullOrEmpty(data?.Role) && data.Notification != null)
                {
                    // Send to all users with the specified role
                    await _hubContext.Clients.Group($"role-{data.Role}")
                        .SendAsync("ReceiveNotification", data.Notification);

                    _logger.LogDebug($"Delivered notification {data.Notification.MessageId} to role {data.Role}", 
                        "RoleNotificationDelivered");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error handling role notification message: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Message structure for user notifications via Redis
        /// </summary>
        private class UserNotificationMessage
        {
            public string Type { get; set; } = string.Empty;
            public NotificationMessage? Notification { get; set; }
            public List<string> TargetUsers { get; set; } = new();
            public DateTime Timestamp { get; set; }
            public string SourceServer { get; set; } = string.Empty;
        }

        /// <summary>
        /// Message structure for application notifications via Redis
        /// </summary>
        private class ApplicationNotificationMessage
        {
            public string Type { get; set; } = string.Empty;
            public NotificationMessage? Notification { get; set; }
            public string ApplicationName { get; set; } = string.Empty;
            public DateTime Timestamp { get; set; }
            public string SourceServer { get; set; } = string.Empty;
        }

        /// <summary>
        /// Message structure for role notifications via Redis
        /// </summary>
        private class RoleNotificationMessage
        {
            public string Type { get; set; } = string.Empty;
            public NotificationMessage? Notification { get; set; }
            public string Role { get; set; } = string.Empty;
            public DateTime Timestamp { get; set; }
            public string SourceServer { get; set; } = string.Empty;
        }
    }
}