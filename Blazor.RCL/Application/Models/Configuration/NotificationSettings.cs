using System;

namespace Blazor.RCL.Application.Models.Configuration
{
    /// <summary>
    /// Configuration settings for the notification system behavior.
    /// Controls polling intervals, timeouts, and other notification-related settings.
    /// </summary>
    public class NotificationSettings
    {
        /// <summary>
        /// Gets or sets the polling interval in milliseconds for fallback mode.
        /// Used when Redis is not available and clients need to poll for notifications.
        /// Default: 30000ms (30 seconds)
        /// </summary>
        public int PollingInterval { get; set; } = 30000;

        /// <summary>
        /// Gets or sets the SignalR client timeout in milliseconds.
        /// Time before a client is considered disconnected.
        /// Default: 60000ms (60 seconds)
        /// </summary>
        public int ClientTimeout { get; set; } = 60000;

        /// <summary>
        /// Gets or sets the SignalR keep-alive interval in milliseconds.
        /// How often to send keep-alive pings to clients.
        /// Default: 15000ms (15 seconds)
        /// </summary>
        public int KeepAliveInterval { get; set; } = 15000;

        /// <summary>
        /// Gets or sets the SignalR handshake timeout in milliseconds.
        /// Maximum time allowed for initial connection handshake.
        /// Default: 15000ms (15 seconds)
        /// </summary>
        public int HandshakeTimeout { get; set; } = 15000;

        /// <summary>
        /// Gets or sets the maximum message size in bytes for SignalR.
        /// Default: 32768 bytes (32KB)
        /// </summary>
        public int MaximumReceiveMessageSize { get; set; } = 32768;

        /// <summary>
        /// Gets or sets whether to enable performance metrics collection.
        /// Useful for monitoring notification system health.
        /// Default: false
        /// </summary>
        public bool EnableMetrics { get; set; } = false;

        /// <summary>
        /// Gets or sets whether to enable detailed error messages in SignalR.
        /// Should only be true in development environments.
        /// Default: false
        /// </summary>
        public bool EnableDetailedErrors { get; set; } = false;

        /// <summary>
        /// Gets or sets the maximum number of disconnected circuits to retain.
        /// Helps prevent memory leaks from disconnected clients.
        /// Default: 100
        /// </summary>
        public int DisconnectedCircuitMaxRetained { get; set; } = 100;

        /// <summary>
        /// Gets or sets the retention period for disconnected circuits.
        /// How long to keep circuit state after disconnection.
        /// Default: 3 minutes
        /// </summary>
        public TimeSpan DisconnectedCircuitRetentionPeriod { get; set; } = TimeSpan.FromMinutes(3);

        /// <summary>
        /// Gets or sets the maximum buffered unacknowledged render batches.
        /// Controls memory usage for unacknowledged messages.
        /// Default: 10
        /// </summary>
        public int MaxBufferedUnacknowledgedRenderBatches { get; set; } = 10;

        /// <summary>
        /// Gets or sets the default timeout for JavaScript interop calls.
        /// Default: 60 seconds
        /// </summary>
        public TimeSpan JSInteropDefaultCallTimeout { get; set; } = TimeSpan.FromSeconds(60);
    }
}