using System;

namespace Blazor.RCL.Domain.Entities.Configuration
{
    /// <summary>
    /// Configuration settings for SMTP email server.
    /// </summary>
    public class SmtpSettings
    {
        /// <summary>
        /// Gets or sets the SMTP server address.
        /// </summary>
        public string Server { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the SMTP server port.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Gets or sets the SMTP server username.
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the SMTP server password.
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the sender email address.
        /// </summary>
        public string FromEmail { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the sender display name.
        /// </summary>
        public string FromName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets whether to use SSL.
        /// </summary>
        public bool EnableSsl { get; set; } = true;
    }
}
