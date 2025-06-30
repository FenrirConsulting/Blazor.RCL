using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Blazor.RCL.Application.Interfaces;
using Blazor.RCL.Domain.Entities.Notifications;
using Blazor.RCL.NLog.LogService.Interface;
using Newtonsoft.Json;

namespace Blazor.RCL.Infrastructure.Data.Repositories
{
    /// <summary>
    /// Repository implementation for email template operations
    /// </summary>
    public class EmailTemplateRepository : IEmailTemplateRepository
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly ILogHelper _logger;

        public EmailTemplateRepository(
            IDbContextFactory<AppDbContext> contextFactory,
            ILogHelper logger)
        {
            _contextFactory = contextFactory;
            _logger = logger;
        }

        public async Task<EmailTemplate?> GetByIdAsync(Guid id)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync();
                return await context.EmailTemplates
                    .Include(e => e.Application)
                    .FirstOrDefaultAsync(e => e.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error getting email template by ID", ex);
                throw;
            }
        }

        public async Task<EmailTemplate?> GetByKeyAsync(string templateKey, string? applicationName = null)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync();
                return await context.EmailTemplates
                    .Include(e => e.Application)
                    .Where(e => e.TemplateKey == templateKey && 
                               e.ApplicationName == applicationName &&
                               e.IsActive)
                    .OrderByDescending(e => e.Version)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error getting email template by key", ex);
                throw;
            }
        }

        public async Task<EmailTemplate?> GetActiveTemplateAsync(string templateKey, string applicationName)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync();
                
                // First try to get application-specific template
                var appTemplate = await context.EmailTemplates
                    .Include(e => e.Application)
                    .Where(e => e.TemplateKey == templateKey && 
                               e.ApplicationName == applicationName &&
                               e.IsActive)
                    .OrderByDescending(e => e.Version)
                    .FirstOrDefaultAsync();

                if (appTemplate != null)
                    return appTemplate;

                // Fall back to global template
                return await context.EmailTemplates
                    .Include(e => e.Application)
                    .Where(e => e.TemplateKey == templateKey && 
                               e.ApplicationName == null &&
                               e.IsActive)
                    .OrderByDescending(e => e.Version)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error getting active email template", ex);
                throw;
            }
        }

        public async Task<IEnumerable<EmailTemplate>> GetByApplicationAsync(string? applicationName, bool includeInactive = false)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync();
                var query = context.EmailTemplates
                    .Include(e => e.Application)
                    .Where(e => e.ApplicationName == applicationName);

                if (!includeInactive)
                    query = query.Where(e => e.IsActive);

                return await query
                    .OrderBy(e => e.TemplateKey)
                    .ThenByDescending(e => e.Version)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error getting email templates by application", ex);
                throw;
            }
        }

        public async Task<IEnumerable<EmailTemplate>> GetActiveTemplatesAsync()
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync();
                return await context.EmailTemplates
                    .Include(e => e.Application)
                    .Where(e => e.IsActive)
                    .OrderBy(e => e.ApplicationName)
                    .ThenBy(e => e.TemplateKey)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error getting active email templates", ex);
                throw;
            }
        }

        public async Task<IEnumerable<EmailTemplate>> GetByAlertTypeAsync(AlertType alertType, string? applicationName = null)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync();
                var query = context.EmailTemplates
                    .Include(e => e.Application)
                    .Where(e => e.IsActive);

                if (applicationName != null)
                    query = query.Where(e => e.ApplicationName == applicationName || e.ApplicationName == null);

                // Filter by alert type if supported
                var templates = await query.ToListAsync();
                
                return templates.Where(t => 
                {
                    if (string.IsNullOrEmpty(t.SupportedAlertTypes))
                        return true; // Supports all types
                    
                    try
                    {
                        var supportedTypes = JsonConvert.DeserializeObject<List<int>>(t.SupportedAlertTypes);
                        return supportedTypes == null || supportedTypes.Contains((int)alertType);
                    }
                    catch
                    {
                        return true; // If parsing fails, assume it supports all types
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error getting email templates by alert type", ex);
                throw;
            }
        }

        public async Task<EmailTemplate?> GetDefaultTemplateAsync(string templateKey)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync();
                return await context.EmailTemplates
                    .Include(e => e.Application)
                    .Where(e => e.TemplateKey == templateKey && 
                               e.IsDefault &&
                               e.IsActive)
                    .OrderByDescending(e => e.Version)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error getting default email template", ex);
                throw;
            }
        }

        public async Task<EmailTemplate> CreateAsync(EmailTemplate template)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync();
                
                template.Id = Guid.NewGuid();
                template.Version = 1;
                template.CreatedAt = DateTime.UtcNow;
                template.UpdatedAt = DateTime.UtcNow;

                context.EmailTemplates.Add(template);
                await context.SaveChangesAsync();

                _logger.LogInfo("Email template created", "EmailTemplateCreated", new { TemplateId = template.Id, TemplateKey = template.TemplateKey });

                return template;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error creating email template", ex);
                throw;
            }
        }

        public async Task<EmailTemplate> UpdateAsync(EmailTemplate template)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync();
                
                template.UpdatedAt = DateTime.UtcNow;
                
                context.EmailTemplates.Update(template);
                await context.SaveChangesAsync();

                _logger.LogInfo("Email template updated", "EmailTemplateUpdated", new { TemplateId = template.Id });

                return template;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error updating email template", ex);
                throw;
            }
        }

        public async Task<EmailTemplate> CreateVersionAsync(Guid templateId)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync();
                
                var existingTemplate = await context.EmailTemplates
                    .AsNoTracking()
                    .FirstOrDefaultAsync(e => e.Id == templateId);

                if (existingTemplate == null)
                    throw new InvalidOperationException($"Template with ID {templateId} not found");

                // Get the highest version for this template key
                var maxVersion = await context.EmailTemplates
                    .Where(e => e.TemplateKey == existingTemplate.TemplateKey && 
                               e.ApplicationName == existingTemplate.ApplicationName)
                    .MaxAsync(e => (int?)e.Version) ?? 0;

                // Create new version
                var newTemplate = new EmailTemplate
                {
                    Id = Guid.NewGuid(),
                    TemplateKey = existingTemplate.TemplateKey,
                    ApplicationName = existingTemplate.ApplicationName,
                    Name = existingTemplate.Name,
                    Description = existingTemplate.Description,
                    SubjectTemplate = existingTemplate.SubjectTemplate,
                    HtmlBodyTemplate = existingTemplate.HtmlBodyTemplate,
                    TextBodyTemplate = existingTemplate.TextBodyTemplate,
                    AvailableVariables = existingTemplate.AvailableVariables,
                    CustomStyles = existingTemplate.CustomStyles,
                    Version = maxVersion + 1,
                    IsActive = false, // New versions start inactive
                    IsDefault = false,
                    SupportedAlertTypes = existingTemplate.SupportedAlertTypes,
                    SupportedSeverityLevels = existingTemplate.SupportedSeverityLevels,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    CreatedBy = existingTemplate.UpdatedBy // Carry forward who made the update
                };

                context.EmailTemplates.Add(newTemplate);
                await context.SaveChangesAsync();

                _logger.LogInfo("Email template version created", "EmailTemplateVersioned", 
                    new { OriginalId = templateId, NewId = newTemplate.Id, Version = newTemplate.Version });

                return newTemplate;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error creating email template version", ex);
                throw;
            }
        }

        public async Task<EmailTemplate> ActivateTemplateAsync(Guid templateId)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync();
                
                var template = await context.EmailTemplates
                    .FirstOrDefaultAsync(e => e.Id == templateId);

                if (template == null)
                    throw new InvalidOperationException($"Template with ID {templateId} not found");

                // Deactivate other versions of this template
                var otherVersions = await context.EmailTemplates
                    .Where(e => e.TemplateKey == template.TemplateKey && 
                               e.ApplicationName == template.ApplicationName &&
                               e.Id != templateId &&
                               e.IsActive)
                    .ToListAsync();

                foreach (var other in otherVersions)
                {
                    other.IsActive = false;
                    other.UpdatedAt = DateTime.UtcNow;
                }

                // Activate this template
                template.IsActive = true;
                template.UpdatedAt = DateTime.UtcNow;

                await context.SaveChangesAsync();

                _logger.LogInfo("Email template activated", "EmailTemplateActivated", 
                    new { TemplateId = templateId, DeactivatedCount = otherVersions.Count });

                return template;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error activating email template", ex);
                throw;
            }
        }

        public async Task<EmailTemplate> DeactivateTemplateAsync(Guid templateId)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync();
                
                var template = await context.EmailTemplates
                    .FirstOrDefaultAsync(e => e.Id == templateId);

                if (template == null)
                    throw new InvalidOperationException($"Template with ID {templateId} not found");

                template.IsActive = false;
                template.UpdatedAt = DateTime.UtcNow;

                await context.SaveChangesAsync();

                _logger.LogInfo("Email template deactivated", "EmailTemplateDeactivated", new { TemplateId = templateId });

                return template;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error deactivating email template", ex);
                throw;
            }
        }

        public async Task<EmailTemplate> SetAsDefaultAsync(Guid templateId)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync();
                
                var template = await context.EmailTemplates
                    .FirstOrDefaultAsync(e => e.Id == templateId);

                if (template == null)
                    throw new InvalidOperationException($"Template with ID {templateId} not found");

                // Remove default flag from other templates with same key
                var otherDefaults = await context.EmailTemplates
                    .Where(e => e.TemplateKey == template.TemplateKey && 
                               e.Id != templateId &&
                               e.IsDefault)
                    .ToListAsync();

                foreach (var other in otherDefaults)
                {
                    other.IsDefault = false;
                    other.UpdatedAt = DateTime.UtcNow;
                }

                // Set this as default
                template.IsDefault = true;
                template.UpdatedAt = DateTime.UtcNow;

                await context.SaveChangesAsync();

                _logger.LogInfo("Email template set as default", "EmailTemplateDefaultSet", new { TemplateId = templateId });

                return template;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error setting email template as default", ex);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(Guid templateId)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync();
                
                var template = await context.EmailTemplates
                    .FirstOrDefaultAsync(e => e.Id == templateId);

                if (template == null)
                    return false;

                // Soft delete by deactivating
                template.IsActive = false;
                template.UpdatedAt = DateTime.UtcNow;

                await context.SaveChangesAsync();

                _logger.LogInfo("Email template deleted (soft)", "EmailTemplateDeleted", new { TemplateId = templateId });

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error deleting email template", ex);
                throw;
            }
        }

        public async Task<IEnumerable<EmailTemplate>> GetVersionHistoryAsync(string templateKey, string? applicationName)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync();
                
                return await context.EmailTemplates
                    .Include(e => e.Application)
                    .Where(e => e.TemplateKey == templateKey && 
                               e.ApplicationName == applicationName)
                    .OrderByDescending(e => e.Version)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error getting email template version history", ex);
                throw;
            }
        }

        public async Task<bool> ExistsAsync(string templateKey, string? applicationName)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync();
                
                return await context.EmailTemplates
                    .AnyAsync(e => e.TemplateKey == templateKey && 
                                  e.ApplicationName == applicationName &&
                                  e.IsActive);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error checking if email template exists", ex);
                throw;
            }
        }

        public async Task<IEnumerable<string>> GetAllTemplateKeysAsync()
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync();
                
                return await context.EmailTemplates
                    .Where(e => e.IsActive)
                    .Select(e => e.TemplateKey)
                    .Distinct()
                    .OrderBy(k => k)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error getting all template keys", ex);
                throw;
            }
        }
    }
}