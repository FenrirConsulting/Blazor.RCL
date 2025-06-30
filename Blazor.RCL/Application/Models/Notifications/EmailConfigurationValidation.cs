using System.Collections.Generic;

namespace Blazor.RCL.Application.Models.Notifications
{
    /// <summary>
    /// Result of email configuration validation
    /// </summary>
    public class EmailConfigurationValidation
    {
        /// <summary>
        /// Whether the configuration is valid
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// SMTP server configuration status
        /// </summary>
        public SmtpServerStatus SmtpStatus { get; set; } = new();

        /// <summary>
        /// List of configuration issues found
        /// </summary>
        public List<string> Issues { get; set; } = new();

        /// <summary>
        /// List of warnings (non-critical issues)
        /// </summary>
        public List<string> Warnings { get; set; } = new();

        /// <summary>
        /// Whether test email was sent successfully
        /// </summary>
        public bool TestEmailSent { get; set; }

        /// <summary>
        /// Test email error message if failed
        /// </summary>
        public string? TestEmailError { get; set; }
    }

    /// <summary>
    /// SMTP server status information
    /// </summary>
    public class SmtpServerStatus
    {
        /// <summary>
        /// SMTP server hostname
        /// </summary>
        public string Host { get; set; } = string.Empty;

        /// <summary>
        /// SMTP server port
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Whether SSL/TLS is enabled
        /// </summary>
        public bool UseSsl { get; set; }

        /// <summary>
        /// Whether server is reachable
        /// </summary>
        public bool IsReachable { get; set; }

        /// <summary>
        /// Whether authentication is configured
        /// </summary>
        public bool AuthenticationConfigured { get; set; }

        /// <summary>
        /// From email address configured
        /// </summary>
        public string FromEmail { get; set; } = string.Empty;

        /// <summary>
        /// From display name configured
        /// </summary>
        public string FromDisplayName { get; set; } = string.Empty;
    }
}