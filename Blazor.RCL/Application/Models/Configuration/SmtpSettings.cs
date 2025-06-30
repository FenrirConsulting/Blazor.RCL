namespace Blazor.RCL.Application.Models.Configuration
{
    /// <summary>
    /// SMTP configuration settings for email notifications
    /// </summary>
    public class SmtpSettings
    {
        /// <summary>
        /// SMTP server hostname
        /// </summary>
        public string Host { get; set; } = string.Empty;

        /// <summary>
        /// SMTP server port
        /// </summary>
        public int Port { get; set; } = 587;

        /// <summary>
        /// Whether to use SSL/TLS
        /// </summary>
        public bool EnableSsl { get; set; } = true;

        /// <summary>
        /// Whether authentication is required
        /// </summary>
        public bool UseAuthentication { get; set; } = true;

        /// <summary>
        /// SMTP username (if UseAuthentication is true)
        /// </summary>
        public string? Username { get; set; }

        /// <summary>
        /// From email address
        /// </summary>
        public string FromEmail { get; set; } = string.Empty;

        /// <summary>
        /// From display name
        /// </summary>
        public string FromDisplayName { get; set; } = "Automation Notifications";

        /// <summary>
        /// Connection timeout in seconds
        /// </summary>
        public int Timeout { get; set; } = 30;

        /// <summary>
        /// Test email recipient for validation
        /// </summary>
        public string? TestEmailRecipient { get; set; }
    }
}