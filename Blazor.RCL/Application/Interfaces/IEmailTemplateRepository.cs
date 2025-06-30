using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazor.RCL.Domain.Entities.Notifications;

namespace Blazor.RCL.Application.Interfaces
{
    /// <summary>
    /// Repository interface for email template operations
    /// </summary>
    public interface IEmailTemplateRepository
    {
        /// <summary>
        /// Gets a template by its unique ID
        /// </summary>
        /// <param name="id">The template ID</param>
        /// <returns>The email template or null if not found</returns>
        Task<EmailTemplate?> GetByIdAsync(Guid id);

        /// <summary>
        /// Gets a template by its key and application
        /// </summary>
        /// <param name="templateKey">The template key (e.g., "SystemAlert")</param>
        /// <param name="applicationName">The application name (null for global templates)</param>
        /// <returns>The email template or null if not found</returns>
        Task<EmailTemplate?> GetByKeyAsync(string templateKey, string? applicationName = null);

        /// <summary>
        /// Gets the active template for a specific key and application
        /// Falls back to global template if no application-specific template exists
        /// </summary>
        /// <param name="templateKey">The template key</param>
        /// <param name="applicationName">The application name</param>
        /// <returns>The best matching active template or null</returns>
        Task<EmailTemplate?> GetActiveTemplateAsync(string templateKey, string applicationName);

        /// <summary>
        /// Gets all templates for an application
        /// </summary>
        /// <param name="applicationName">The application name (null for global templates)</param>
        /// <param name="includeInactive">Whether to include inactive templates</param>
        /// <returns>Collection of email templates</returns>
        Task<IEnumerable<EmailTemplate>> GetByApplicationAsync(string? applicationName, bool includeInactive = false);

        /// <summary>
        /// Gets all active templates
        /// </summary>
        /// <returns>Collection of active email templates</returns>
        Task<IEnumerable<EmailTemplate>> GetActiveTemplatesAsync();

        /// <summary>
        /// Gets templates by alert type
        /// </summary>
        /// <param name="alertType">The alert type</param>
        /// <param name="applicationName">Optional application filter</param>
        /// <returns>Collection of matching templates</returns>
        Task<IEnumerable<EmailTemplate>> GetByAlertTypeAsync(AlertType alertType, string? applicationName = null);

        /// <summary>
        /// Gets the default template for a given key
        /// </summary>
        /// <param name="templateKey">The template key</param>
        /// <returns>The default template or null</returns>
        Task<EmailTemplate?> GetDefaultTemplateAsync(string templateKey);

        /// <summary>
        /// Creates a new email template
        /// </summary>
        /// <param name="template">The template to create</param>
        /// <returns>The created template</returns>
        Task<EmailTemplate> CreateAsync(EmailTemplate template);

        /// <summary>
        /// Updates an existing email template
        /// Creates a new version instead of overwriting
        /// </summary>
        /// <param name="template">The template to update</param>
        /// <returns>The updated template</returns>
        Task<EmailTemplate> UpdateAsync(EmailTemplate template);

        /// <summary>
        /// Creates a new version of an existing template
        /// </summary>
        /// <param name="templateId">The ID of the template to version</param>
        /// <returns>The new template version</returns>
        Task<EmailTemplate> CreateVersionAsync(Guid templateId);

        /// <summary>
        /// Activates a template (deactivates other versions)
        /// </summary>
        /// <param name="templateId">The template ID to activate</param>
        /// <returns>The activated template</returns>
        Task<EmailTemplate> ActivateTemplateAsync(Guid templateId);

        /// <summary>
        /// Deactivates a template
        /// </summary>
        /// <param name="templateId">The template ID to deactivate</param>
        /// <returns>The deactivated template</returns>
        Task<EmailTemplate> DeactivateTemplateAsync(Guid templateId);

        /// <summary>
        /// Sets a template as the default for its key
        /// </summary>
        /// <param name="templateId">The template ID</param>
        /// <returns>The updated template</returns>
        Task<EmailTemplate> SetAsDefaultAsync(Guid templateId);

        /// <summary>
        /// Deletes a template (soft delete by deactivating)
        /// </summary>
        /// <param name="templateId">The template ID to delete</param>
        /// <returns>True if deleted, false if not found</returns>
        Task<bool> DeleteAsync(Guid templateId);

        /// <summary>
        /// Gets template version history
        /// </summary>
        /// <param name="templateKey">The template key</param>
        /// <param name="applicationName">The application name</param>
        /// <returns>Collection of all versions of the template</returns>
        Task<IEnumerable<EmailTemplate>> GetVersionHistoryAsync(string templateKey, string? applicationName);

        /// <summary>
        /// Checks if a template key exists for an application
        /// </summary>
        /// <param name="templateKey">The template key</param>
        /// <param name="applicationName">The application name</param>
        /// <returns>True if exists</returns>
        Task<bool> ExistsAsync(string templateKey, string? applicationName);

        /// <summary>
        /// Gets all unique template keys
        /// </summary>
        /// <returns>Collection of template keys</returns>
        Task<IEnumerable<string>> GetAllTemplateKeysAsync();
    }
}