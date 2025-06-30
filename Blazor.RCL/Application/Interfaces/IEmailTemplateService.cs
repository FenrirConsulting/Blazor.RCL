using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazor.RCL.Domain.Entities.Notifications;
using Blazor.RCL.Application.Models.Notifications;

namespace Blazor.RCL.Application.Interfaces
{
    /// <summary>
    /// Service interface for email template management and rendering
    /// </summary>
    public interface IEmailTemplateService
    {
        /// <summary>
        /// Renders an email template with the provided variables
        /// </summary>
        /// <param name="templateKey">The template key</param>
        /// <param name="applicationName">The application name</param>
        /// <param name="variables">Dictionary of variable names and values</param>
        /// <returns>Rendered email content (subject, html, text)</returns>
        Task<RenderedEmailContent> RenderTemplateAsync(
            string templateKey, 
            string applicationName, 
            Dictionary<string, object> variables);

        /// <summary>
        /// Renders an email template for a notification
        /// </summary>
        /// <param name="notification">The notification to render</param>
        /// <param name="additionalVariables">Additional variables beyond notification data</param>
        /// <returns>Rendered email content</returns>
        Task<RenderedEmailContent> RenderNotificationEmailAsync(
            NotificationMessage notification,
            Dictionary<string, object>? additionalVariables = null);

        /// <summary>
        /// Gets all available templates for an application
        /// </summary>
        /// <param name="applicationName">The application name</param>
        /// <param name="includeGlobal">Whether to include global templates</param>
        /// <returns>Collection of email template view models</returns>
        Task<IEnumerable<EmailTemplateViewModel>> GetApplicationTemplatesAsync(
            string applicationName, 
            bool includeGlobal = true);

        /// <summary>
        /// Creates a new email template
        /// </summary>
        /// <param name="request">Template creation request</param>
        /// <returns>Created template view model</returns>
        Task<EmailTemplateViewModel> CreateTemplateAsync(CreateEmailTemplateRequest request);

        /// <summary>
        /// Updates an existing email template (creates new version)
        /// </summary>
        /// <param name="templateId">The template ID</param>
        /// <param name="request">Template update request</param>
        /// <returns>Updated template view model</returns>
        Task<EmailTemplateViewModel> UpdateTemplateAsync(
            Guid templateId, 
            UpdateEmailTemplateRequest request);

        /// <summary>
        /// Validates a template's syntax and variables
        /// </summary>
        /// <param name="template">The template to validate</param>
        /// <param name="testVariables">Optional test variables to use</param>
        /// <returns>Validation result with any errors</returns>
        Task<EmailTemplateValidation> ValidateTemplateAsync(
            EmailTemplate template,
            Dictionary<string, object>? testVariables = null);

        /// <summary>
        /// Previews a template with sample data
        /// </summary>
        /// <param name="templateId">The template ID</param>
        /// <param name="sampleData">Sample variable data</param>
        /// <returns>Preview of rendered content</returns>
        Task<RenderedEmailContent> PreviewTemplateAsync(
            Guid templateId,
            Dictionary<string, object>? sampleData = null);

        /// <summary>
        /// Clones a template for a different application
        /// </summary>
        /// <param name="templateId">Source template ID</param>
        /// <param name="targetApplication">Target application name</param>
        /// <returns>Cloned template</returns>
        Task<EmailTemplateViewModel> CloneTemplateAsync(
            Guid templateId, 
            string targetApplication);

        /// <summary>
        /// Gets the default variables available for all templates
        /// </summary>
        /// <returns>Dictionary of variable names and descriptions</returns>
        Task<Dictionary<string, string>> GetDefaultVariablesAsync();

        /// <summary>
        /// Gets variables specific to an alert type
        /// </summary>
        /// <param name="alertType">The alert type</param>
        /// <returns>Dictionary of variable names and descriptions</returns>
        Task<Dictionary<string, string>> GetAlertTypeVariablesAsync(AlertType alertType);

        /// <summary>
        /// Activates a specific template version
        /// </summary>
        /// <param name="templateId">The template ID to activate</param>
        /// <returns>Activated template</returns>
        Task<EmailTemplateViewModel> ActivateTemplateAsync(Guid templateId);

        /// <summary>
        /// Deactivates a template
        /// </summary>
        /// <param name="templateId">The template ID to deactivate</param>
        /// <returns>Deactivated template</returns>
        Task<EmailTemplateViewModel> DeactivateTemplateAsync(Guid templateId);

        /// <summary>
        /// Gets template version history
        /// </summary>
        /// <param name="templateKey">The template key</param>
        /// <param name="applicationName">The application name</param>
        /// <returns>Collection of template versions</returns>
        Task<IEnumerable<EmailTemplateViewModel>> GetTemplateHistoryAsync(
            string templateKey, 
            string? applicationName);

        /// <summary>
        /// Imports templates from JSON
        /// </summary>
        /// <param name="json">JSON containing template definitions</param>
        /// <param name="overwrite">Whether to overwrite existing templates</param>
        /// <returns>Import result with success/failure counts</returns>
        Task<EmailTemplateImportResult> ImportTemplatesAsync(string json, bool overwrite = false);

        /// <summary>
        /// Exports templates to JSON
        /// </summary>
        /// <param name="applicationName">Optional application filter</param>
        /// <param name="includeInactive">Whether to include inactive templates</param>
        /// <returns>JSON string of exported templates</returns>
        Task<string> ExportTemplatesAsync(string? applicationName = null, bool includeInactive = false);

        /// <summary>
        /// Seeds default templates for a new application
        /// </summary>
        /// <param name="applicationName">The application name</param>
        /// <returns>Number of templates created</returns>
        Task<int> SeedDefaultTemplatesAsync(string applicationName);
    }
}