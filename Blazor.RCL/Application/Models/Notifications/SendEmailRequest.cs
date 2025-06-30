using System.Collections.Generic;

namespace Blazor.RCL.Application.Models.Notifications
{
    /// <summary>
    /// Request model for sending an email immediately
    /// </summary>
    public class SendEmailRequest
    {
        /// <summary>
        /// Recipient email address
        /// </summary>
        public string ToEmail { get; set; } = string.Empty;

        /// <summary>
        /// Email subject
        /// </summary>
        public string Subject { get; set; } = string.Empty;

        /// <summary>
        /// HTML body content
        /// </summary>
        public string HtmlBody { get; set; } = string.Empty;

        /// <summary>
        /// Plain text body content (optional)
        /// </summary>
        public string? TextBody { get; set; }

        /// <summary>
        /// CC recipients (optional)
        /// </summary>
        public List<string>? CcEmails { get; set; }

        /// <summary>
        /// BCC recipients (optional)
        /// </summary>
        public List<string>? BccEmails { get; set; }

        /// <summary>
        /// Reply-to address (optional)
        /// </summary>
        public string? ReplyToEmail { get; set; }

        /// <summary>
        /// Whether this is a high priority email
        /// </summary>
        public bool IsHighPriority { get; set; }

        /// <summary>
        /// Custom headers to include (optional)
        /// </summary>
        public Dictionary<string, string>? CustomHeaders { get; set; }
    }
}