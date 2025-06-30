using System;

namespace Blazor.RCL.Application.Models.Configuration
{
    /// <summary>
    /// Configuration settings for Redis backplane connection.
    /// When not configured, the system falls back to polling mode.
    /// </summary>
    public class RedisSettings
    {
        /// <summary>
        /// Gets or sets the Redis server endpoint (e.g., "redis-server.domain.com:6379").
        /// </summary>
        public string Server { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets whether to use SSL/TLS for Redis connection.
        /// </summary>
        public bool UseSSL { get; set; } = true;

        /// <summary>
        /// Gets or sets the connection timeout in milliseconds.
        /// </summary>
        public int ConnectTimeout { get; set; } = 5000;

        /// <summary>
        /// Gets or sets the synchronous operation timeout in milliseconds.
        /// </summary>
        public int SyncTimeout { get; set; } = 5000;

        /// <summary>
        /// Gets or sets whether to abort connection on failure.
        /// Setting to false allows graceful fallback to polling mode.
        /// </summary>
        public bool AbortConnect { get; set; } = false;

        /// <summary>
        /// Gets or sets the channel prefix for SignalR Redis backplane.
        /// Helps isolate different environments (DEV, QA, PROD).
        /// </summary>
        public string ChannelPrefix { get; set; } = "AutomationNotifications";

        /// <summary>
        /// Gets or sets the Redis password. This will be loaded from AKeyless or Registry.
        /// </summary>
        public string? Password { get; set; }

        /// <summary>
        /// Gets or sets whether to allow admin mode operations (e.g., INFO, CONFIG commands).
        /// Required for advanced diagnostics and monitoring.
        /// </summary>
        public bool AllowAdmin { get; set; } = false;

        /// <summary>
        /// Validates if the Redis settings are properly configured.
        /// </summary>
        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Server);
        }
    }
}