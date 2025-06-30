using Blazor.RCL.Domain.Entities.Notifications;
using System;
using System.Collections.Generic;

namespace Blazor.RCL.Application.Models.Notifications
{
    /// <summary>
    /// Represents rendered email content after template processing
    /// </summary>
    public class RenderedEmailContent
    {
        public string Subject { get; set; } = string.Empty;
        public string HtmlBody { get; set; } = string.Empty;
        public string TextBody { get; set; } = string.Empty;
        public Dictionary<string, string> Headers { get; set; } = new();
    }

    /// <summary>
    /// View model for email template display
    /// </summary>
    public class EmailTemplateViewModel
    {
        public Guid Id { get; set; }
        public string TemplateKey { get; set; } = string.Empty;
        public string? ApplicationName { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string SubjectTemplate { get; set; } = string.Empty;
        public string HtmlBodyTemplate { get; set; } = string.Empty;
        public string TextBodyTemplate { get; set; } = string.Empty;
        public List<TemplateVariable> Variables { get; set; } = new();
        public List<string> SupportedAlertTypes { get; set; } = new();
        public List<string> SupportedSeverityLevels { get; set; } = new();
        public int Version { get; set; }
        public bool IsActive { get; set; }
        public bool IsDefault { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
    }

    /// <summary>
    /// Represents a template variable
    /// </summary>
    public class TemplateVariable
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? DefaultValue { get; set; }
        public bool IsRequired { get; set; }
        public string? SampleValue { get; set; }
    }

    /// <summary>
    /// Request to create a new email template
    /// </summary>
    public class CreateEmailTemplateRequest
    {
        public string TemplateKey { get; set; } = string.Empty;
        public string? ApplicationName { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string SubjectTemplate { get; set; } = string.Empty;
        public string HtmlBodyTemplate { get; set; } = string.Empty;
        public string TextBodyTemplate { get; set; } = string.Empty;
        public List<TemplateVariable>? Variables { get; set; }
        public string? CustomStyles { get; set; }
        public List<AlertType>? SupportedAlertTypes { get; set; }
        public List<NotificationSeverity>? SupportedSeverityLevels { get; set; }
        public bool IsDefault { get; set; }
        public bool IsActive { get; set; } = true;
    }

    /// <summary>
    /// Request to update an email template
    /// </summary>
    public class UpdateEmailTemplateRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string SubjectTemplate { get; set; } = string.Empty;
        public string HtmlBodyTemplate { get; set; } = string.Empty;
        public string TextBodyTemplate { get; set; } = string.Empty;
        public List<TemplateVariable>? Variables { get; set; }
        public string? CustomStyles { get; set; }
        public List<AlertType>? SupportedAlertTypes { get; set; }
        public List<NotificationSeverity>? SupportedSeverityLevels { get; set; }
        public bool CreateNewVersion { get; set; } = true;
    }

    /// <summary>
    /// Result of template validation
    /// </summary>
    public class EmailTemplateValidation
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = new();
        public List<string> Warnings { get; set; } = new();
        public List<string> MissingVariables { get; set; } = new();
        public List<string> UnusedVariables { get; set; } = new();
        public RenderedEmailContent? PreviewContent { get; set; }
    }

    /// <summary>
    /// Result of template import operation
    /// </summary>
    public class EmailTemplateImportResult
    {
        public int TotalTemplates { get; set; }
        public int SuccessfulImports { get; set; }
        public int FailedImports { get; set; }
        public int SkippedTemplates { get; set; }
        public List<string> Errors { get; set; } = new();
        public List<string> ImportedTemplateKeys { get; set; } = new();
    }

    /// <summary>
    /// Default template definitions
    /// </summary>
    public static class DefaultEmailTemplates
    {
        public static readonly Dictionary<string, (string name, string description)> StandardTemplates = new()
        {
            ["SystemAlert"] = ("System Alert", "General system alerts and notifications"),
            ["SecurityAlert"] = ("Security Alert", "Security-related notifications"),
            ["MaintenanceNotice"] = ("Maintenance Notice", "Scheduled maintenance notifications"),
            ["PerformanceAlert"] = ("Performance Alert", "Performance and health check alerts"),
            ["ErrorNotification"] = ("Error Notification", "Application error notifications"),
            ["UserAction"] = ("User Action Required", "Notifications requiring user action"),
            ["StatusUpdate"] = ("Status Update", "General status update notifications"),
            ["DigestEmail"] = ("Email Digest", "Consolidated notification digest")
        };

        public static readonly Dictionary<string, string> StandardVariables = new()
        {
            ["Title"] = "Notification title",
            ["Content"] = "Main notification content",
            ["ApplicationName"] = "Source application name",
            ["ApplicationDisplayName"] = "Application display name",
            ["Severity"] = "Notification severity level",
            ["AlertType"] = "Type of alert",
            ["Timestamp"] = "When the event occurred",
            ["Username"] = "Recipient username",
            ["UserDisplayName"] = "Recipient display name",
            ["ActionUrl"] = "URL for user action (if applicable)",
            ["EntityType"] = "Type of entity affected",
            ["EntityId"] = "ID of entity affected",
            ["Environment"] = "Environment (DEV/QA/PROD)",
            ["NotificationId"] = "Unique notification ID"
        };
    }
}