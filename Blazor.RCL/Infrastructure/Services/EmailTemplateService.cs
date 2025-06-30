using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using HandlebarsDotNet;
using Blazor.RCL.Application.Interfaces;
using Blazor.RCL.Application.Models.Notifications;
using Blazor.RCL.Domain.Entities.Notifications;
using Blazor.RCL.NLog.LogService.Interface;
using Microsoft.Extensions.Caching.Memory;

namespace Blazor.RCL.Infrastructure.Services
{
    /// <summary>
    /// Service for managing and rendering email templates using Handlebars
    /// </summary>
    public class EmailTemplateService : IEmailTemplateService
    {
        private readonly IEmailTemplateRepository _templateRepository;
        private readonly IHandlebars _handlebars;
        private readonly ILogHelper _logger;
        private readonly IMemoryCache _templateCache;
        private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(5);

        public EmailTemplateService(
            IEmailTemplateRepository templateRepository,
            ILogHelper logger,
            IMemoryCache memoryCache)
        {
            _templateRepository = templateRepository;
            _logger = logger;
            _templateCache = memoryCache;
            
            // Configure Handlebars
            _handlebars = Handlebars.Create();
            RegisterHandlebarsHelpers();
        }

        private void RegisterHandlebarsHelpers()
        {
            // Register custom helpers for common formatting needs
            _handlebars.RegisterHelper("formatDate", (writer, context, parameters) =>
            {
                if (parameters.Length > 0 && parameters[0] is DateTime date)
                {
                    var format = parameters.Length > 1 ? parameters[1]?.ToString() ?? "yyyy-MM-dd HH:mm:ss" : "yyyy-MM-dd HH:mm:ss";
                    writer.Write(date.ToString(format));
                }
            });

            _handlebars.RegisterHelper("toLowerCase", (writer, context, parameters) =>
            {
                if (parameters.Length > 0 && parameters[0] != null)
                {
                    writer.Write(parameters[0].ToString()?.ToLowerInvariant());
                }
            });

            _handlebars.RegisterHelper("toUpperCase", (writer, context, parameters) =>
            {
                if (parameters.Length > 0 && parameters[0] != null)
                {
                    writer.Write(parameters[0].ToString()?.ToUpperInvariant());
                }
            });

            _handlebars.RegisterHelper("severityColor", (writer, context, parameters) =>
            {
                if (parameters.Length > 0 && Enum.TryParse<NotificationSeverity>(parameters[0]?.ToString(), true, out var severity))
                {
                    var color = severity switch
                    {
                        NotificationSeverity.Critical => "#DC3545",
                        NotificationSeverity.Error => "#DC3545",
                        NotificationSeverity.Warning => "#FFC107",
                        NotificationSeverity.Info => "#0D6EFD",
                        _ => "#6C757D"
                    };
                    writer.Write(color);
                }
            });
        }

        public async Task<RenderedEmailContent> RenderTemplateAsync(
            string templateKey,
            string applicationName,
            Dictionary<string, object> variables)
        {
            try
            {
                var template = await GetCachedTemplateAsync(templateKey, applicationName);
                if (template == null)
                {
                    throw new InvalidOperationException($"No active template found for key '{templateKey}' and application '{applicationName}'");
                }

                // Merge default variables with provided variables
                var context = BuildVariableContext(variables, template);

                // Compile and render templates
                var subjectTemplate = _handlebars.Compile(template.SubjectTemplate);
                var htmlTemplate = _handlebars.Compile(ApplyCustomStyles(template.HtmlBodyTemplate, template.CustomStyles));
                var textTemplate = _handlebars.Compile(template.TextBodyTemplate);

                return new RenderedEmailContent
                {
                    Subject = subjectTemplate(context),
                    HtmlBody = htmlTemplate(context),
                    TextBody = textTemplate(context),
                    Headers = new Dictionary<string, string>
                    {
                        ["X-Template-Key"] = templateKey,
                        ["X-Template-Version"] = template.Version.ToString()
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to render template '{templateKey}' for '{applicationName}'", ex);
                throw new InvalidOperationException($"Template rendering failed: {ex.Message}", ex);
            }
        }

        public async Task<RenderedEmailContent> RenderNotificationEmailAsync(
            NotificationMessage notification,
            Dictionary<string, object>? additionalVariables = null)
        {
            // Determine the appropriate template key based on alert type
            var templateKey = DetermineTemplateKey(notification);

            // Build variables from notification
            var variables = new Dictionary<string, object>
            {
                ["Title"] = notification.Title,
                ["Content"] = notification.Content,
                ["NotificationId"] = notification.Id.ToString(),
                ["ApplicationName"] = notification.SourceApplication,
                ["Severity"] = notification.Severity.ToString(),
                ["AlertType"] = ((AlertType)notification.AlertType).ToString(),
                ["Timestamp"] = notification.CreatedAt
            };

            // Parse metadata if available
            if (!string.IsNullOrEmpty(notification.Metadata))
            {
                try
                {
                    var metadata = JsonSerializer.Deserialize<Dictionary<string, object>>(notification.Metadata);
                    if (metadata != null)
                    {
                        foreach (var kvp in metadata)
                        {
                            variables[$"Metadata_{kvp.Key}"] = kvp.Value;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarn($"Failed to parse notification metadata: {ex.Message}");
                }
            }

            // Merge additional variables
            if (additionalVariables != null)
            {
                foreach (var kvp in additionalVariables)
                {
                    variables[kvp.Key] = kvp.Value;
                }
            }

            return await RenderTemplateAsync(templateKey, notification.SourceApplication, variables);
        }

        public async Task<IEnumerable<EmailTemplateViewModel>> GetApplicationTemplatesAsync(
            string applicationName,
            bool includeGlobal = true)
        {
            var templates = new List<EmailTemplate>();
            
            // Get application-specific templates
            var appTemplates = await _templateRepository.GetByApplicationAsync(applicationName, includeInactive: true);
            templates.AddRange(appTemplates);

            // Get global templates if requested
            if (includeGlobal)
            {
                var globalTemplates = await _templateRepository.GetByApplicationAsync(null, includeInactive: true);
                templates.AddRange(globalTemplates);
            }

            return templates.Select(MapToViewModel).OrderBy(t => t.TemplateKey).ThenBy(t => t.ApplicationName);
        }

        public async Task<EmailTemplateViewModel> CreateTemplateAsync(CreateEmailTemplateRequest request)
        {
            // Validate template syntax
            var template = new EmailTemplate
            {
                TemplateKey = request.TemplateKey,
                ApplicationName = request.ApplicationName,
                Name = request.Name,
                Description = request.Description,
                SubjectTemplate = request.SubjectTemplate,
                HtmlBodyTemplate = request.HtmlBodyTemplate,
                TextBodyTemplate = request.TextBodyTemplate,
                CustomStyles = request.CustomStyles,
                IsActive = request.IsActive,
                IsDefault = request.IsDefault,
                Version = 1
            };

            var validation = await ValidateTemplateAsync(template);
            if (!validation.IsValid)
            {
                throw new InvalidOperationException($"Template validation failed: {string.Join(", ", validation.Errors)}");
            }

            // Check if template key already exists
            if (await _templateRepository.ExistsAsync(request.TemplateKey, request.ApplicationName))
            {
                throw new InvalidOperationException($"Template with key '{request.TemplateKey}' already exists for this application");
            }

            // Set metadata
            template.Id = Guid.NewGuid();
            template.CreatedAt = DateTime.UtcNow;
            template.UpdatedAt = DateTime.UtcNow;
            template.AvailableVariables = SerializeVariables(request.Variables);
            template.SupportedAlertTypes = request.SupportedAlertTypes != null 
                ? JsonSerializer.Serialize(request.SupportedAlertTypes.Select(a => a.ToString())) 
                : null;
            template.SupportedSeverityLevels = request.SupportedSeverityLevels != null 
                ? JsonSerializer.Serialize(request.SupportedSeverityLevels.Select(s => s.ToString())) 
                : null;

            // Create template
            var created = await _templateRepository.CreateAsync(template);
            
            // Clear cache
            InvalidateCache(created.TemplateKey, created.ApplicationName);

            return MapToViewModel(created);
        }

        public async Task<EmailTemplateViewModel> UpdateTemplateAsync(Guid templateId, UpdateEmailTemplateRequest request)
        {
            var existing = await _templateRepository.GetByIdAsync(templateId);
            if (existing == null)
            {
                throw new InvalidOperationException($"Template with ID '{templateId}' not found");
            }

            EmailTemplate updated;
            
            if (request.CreateNewVersion)
            {
                // Create new version
                updated = await _templateRepository.CreateVersionAsync(templateId);
                updated.Name = request.Name;
                updated.Description = request.Description;
                updated.SubjectTemplate = request.SubjectTemplate;
                updated.HtmlBodyTemplate = request.HtmlBodyTemplate;
                updated.TextBodyTemplate = request.TextBodyTemplate;
                updated.CustomStyles = request.CustomStyles;
                updated.UpdatedAt = DateTime.UtcNow;
                updated.AvailableVariables = SerializeVariables(request.Variables);
                updated.SupportedAlertTypes = request.SupportedAlertTypes != null 
                    ? JsonSerializer.Serialize(request.SupportedAlertTypes.Select(a => a.ToString())) 
                    : null;
                updated.SupportedSeverityLevels = request.SupportedSeverityLevels != null 
                    ? JsonSerializer.Serialize(request.SupportedSeverityLevels.Select(s => s.ToString())) 
                    : null;
            }
            else
            {
                // Update existing
                existing.Name = request.Name;
                existing.Description = request.Description;
                existing.SubjectTemplate = request.SubjectTemplate;
                existing.HtmlBodyTemplate = request.HtmlBodyTemplate;
                existing.TextBodyTemplate = request.TextBodyTemplate;
                existing.CustomStyles = request.CustomStyles;
                existing.UpdatedAt = DateTime.UtcNow;
                existing.AvailableVariables = SerializeVariables(request.Variables);
                existing.SupportedAlertTypes = request.SupportedAlertTypes != null 
                    ? JsonSerializer.Serialize(request.SupportedAlertTypes.Select(a => a.ToString())) 
                    : null;
                existing.SupportedSeverityLevels = request.SupportedSeverityLevels != null 
                    ? JsonSerializer.Serialize(request.SupportedSeverityLevels.Select(s => s.ToString())) 
                    : null;
                updated = existing;
            }

            // Validate before saving
            var validation = await ValidateTemplateAsync(updated);
            if (!validation.IsValid)
            {
                throw new InvalidOperationException($"Template validation failed: {string.Join(", ", validation.Errors)}");
            }

            await _templateRepository.UpdateAsync(updated);
            
            // Clear cache
            InvalidateCache(updated.TemplateKey, updated.ApplicationName);

            return MapToViewModel(updated);
        }

        public async Task<EmailTemplateValidation> ValidateTemplateAsync(
            EmailTemplate template,
            Dictionary<string, object>? testVariables = null)
        {
            var validation = new EmailTemplateValidation { IsValid = true };

            // Create test variables if not provided
            var variables = testVariables ?? GetSampleVariables();

            try
            {
                // Validate subject template
                var subjectTemplate = _handlebars.Compile(template.SubjectTemplate);
                var renderedSubject = subjectTemplate(variables);
                
                if (string.IsNullOrWhiteSpace(renderedSubject))
                {
                    validation.Errors.Add("Subject template produces empty result");
                    validation.IsValid = false;
                }

                // Validate HTML body template
                var htmlTemplate = _handlebars.Compile(template.HtmlBodyTemplate);
                var renderedHtml = htmlTemplate(variables);
                
                if (string.IsNullOrWhiteSpace(renderedHtml))
                {
                    validation.Errors.Add("HTML body template produces empty result");
                    validation.IsValid = false;
                }

                // Validate text body template
                var textTemplate = _handlebars.Compile(template.TextBodyTemplate);
                var renderedText = textTemplate(variables);
                
                if (string.IsNullOrWhiteSpace(renderedText))
                {
                    validation.Errors.Add("Text body template produces empty result");
                    validation.IsValid = false;
                }

                // Check for missing variables
                var usedVariables = ExtractVariablesFromTemplate(template);
                var availableVariables = variables.Keys.ToHashSet();
                var missingVariables = usedVariables.Except(availableVariables).ToList();
                
                if (missingVariables.Any())
                {
                    validation.MissingVariables.AddRange(missingVariables);
                    validation.Warnings.Add($"Template uses variables that may not be available: {string.Join(", ", missingVariables)}");
                }

                // Check for unused variables
                var definedVariables = ParseVariables(template.AvailableVariables).Select(v => v.Name).ToHashSet();
                var unusedVariables = definedVariables.Except(usedVariables).ToList();
                
                if (unusedVariables.Any())
                {
                    validation.UnusedVariables.AddRange(unusedVariables);
                    validation.Warnings.Add($"Template defines variables that are not used: {string.Join(", ", unusedVariables)}");
                }

                // Add preview if validation passed
                if (validation.IsValid)
                {
                    validation.PreviewContent = new RenderedEmailContent
                    {
                        Subject = renderedSubject,
                        HtmlBody = ApplyCustomStyles(renderedHtml, template.CustomStyles),
                        TextBody = renderedText
                    };
                }
            }
            catch (Exception ex)
            {
                validation.IsValid = false;
                validation.Errors.Add($"Template compilation error: {ex.Message}");
            }

            return validation;
        }

        public async Task<RenderedEmailContent> PreviewTemplateAsync(
            Guid templateId,
            Dictionary<string, object>? sampleData = null)
        {
            var template = await _templateRepository.GetByIdAsync(templateId);
            if (template == null)
            {
                throw new InvalidOperationException($"Template with ID '{templateId}' not found");
            }

            var variables = sampleData ?? GetSampleVariables();
            var context = BuildVariableContext(variables, template);

            try
            {
                var subjectTemplate = _handlebars.Compile(template.SubjectTemplate);
                var htmlTemplate = _handlebars.Compile(ApplyCustomStyles(template.HtmlBodyTemplate, template.CustomStyles));
                var textTemplate = _handlebars.Compile(template.TextBodyTemplate);

                return new RenderedEmailContent
                {
                    Subject = subjectTemplate(context),
                    HtmlBody = htmlTemplate(context),
                    TextBody = textTemplate(context),
                    Headers = new Dictionary<string, string>
                    {
                        ["X-Template-Key"] = template.TemplateKey,
                        ["X-Template-Version"] = template.Version.ToString(),
                        ["X-Preview"] = "true"
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to preview template '{templateId}'", ex);
                throw new InvalidOperationException($"Template preview failed: {ex.Message}", ex);
            }
        }

        public async Task<EmailTemplateViewModel> CloneTemplateAsync(Guid templateId, string targetApplication)
        {
            var source = await _templateRepository.GetByIdAsync(templateId);
            if (source == null)
            {
                throw new InvalidOperationException($"Template with ID '{templateId}' not found");
            }

            // Check if template already exists in target application
            if (await _templateRepository.ExistsAsync(source.TemplateKey, targetApplication))
            {
                throw new InvalidOperationException($"Template with key '{source.TemplateKey}' already exists in '{targetApplication}'");
            }

            var clone = new EmailTemplate
            {
                Id = Guid.NewGuid(),
                TemplateKey = source.TemplateKey,
                ApplicationName = targetApplication,
                Name = source.Name,
                Description = source.Description,
                SubjectTemplate = source.SubjectTemplate,
                HtmlBodyTemplate = source.HtmlBodyTemplate,
                TextBodyTemplate = source.TextBodyTemplate,
                AvailableVariables = source.AvailableVariables,
                CustomStyles = source.CustomStyles,
                SupportedAlertTypes = source.SupportedAlertTypes,
                SupportedSeverityLevels = source.SupportedSeverityLevels,
                Version = 1,
                IsActive = true,
                IsDefault = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var created = await _templateRepository.CreateAsync(clone);
            
            return MapToViewModel(created);
        }

        public async Task<Dictionary<string, string>> GetDefaultVariablesAsync()
        {
            return await Task.FromResult(DefaultEmailTemplates.StandardVariables);
        }

        public async Task<Dictionary<string, string>> GetAlertTypeVariablesAsync(AlertType alertType)
        {
            var variables = new Dictionary<string, string>(DefaultEmailTemplates.StandardVariables);

            // Add alert type specific variables
            switch (alertType)
            {
                case AlertType.Security:
                    variables["SecurityThreatLevel"] = "Security threat level (Low/Medium/High/Critical)";
                    variables["AffectedResource"] = "Resource affected by security event";
                    variables["RemediationSteps"] = "Steps to remediate the security issue";
                    break;
                    
                case AlertType.Performance:
                    variables["MetricName"] = "Name of the performance metric";
                    variables["MetricValue"] = "Current value of the metric";
                    variables["Threshold"] = "Threshold that was exceeded";
                    variables["Duration"] = "How long the issue has persisted";
                    break;
                    
                case AlertType.Maintenance:
                    variables["MaintenanceWindow"] = "Scheduled maintenance window";
                    variables["ExpectedDuration"] = "Expected duration of maintenance";
                    variables["AffectedServices"] = "Services affected by maintenance";
                    variables["ContactInfo"] = "Contact information for questions";
                    break;
                    
                case AlertType.HealthCheck:
                    variables["HealthStatus"] = "Health check status";
                    variables["CheckDetails"] = "Details of health check";
                    variables["StackTrace"] = "Stack trace (if applicable)";
                    variables["FailedOperation"] = "Operation that failed";
                    break;
            }

            return await Task.FromResult(variables);
        }

        public async Task<EmailTemplateViewModel> ActivateTemplateAsync(Guid templateId)
        {
            var activated = await _templateRepository.ActivateTemplateAsync(templateId);
            InvalidateCache(activated.TemplateKey, activated.ApplicationName);
            
            return MapToViewModel(activated);
        }

        public async Task<EmailTemplateViewModel> DeactivateTemplateAsync(Guid templateId)
        {
            var deactivated = await _templateRepository.DeactivateTemplateAsync(templateId);
            InvalidateCache(deactivated.TemplateKey, deactivated.ApplicationName);
            
            return MapToViewModel(deactivated);
        }

        public async Task<IEnumerable<EmailTemplateViewModel>> GetTemplateHistoryAsync(
            string templateKey,
            string? applicationName)
        {
            var history = await _templateRepository.GetVersionHistoryAsync(templateKey, applicationName);
            return history.Select(MapToViewModel).OrderByDescending(t => t.Version);
        }

        public async Task<EmailTemplateImportResult> ImportTemplatesAsync(string json, bool overwrite = false)
        {
            var result = new EmailTemplateImportResult();

            try
            {
                var templates = JsonSerializer.Deserialize<List<CreateEmailTemplateRequest>>(json);
                if (templates == null)
                {
                    throw new InvalidOperationException("Invalid JSON format");
                }

                result.TotalTemplates = templates.Count;

                foreach (var templateRequest in templates)
                {
                    try
                    {
                        var exists = await _templateRepository.ExistsAsync(templateRequest.TemplateKey, templateRequest.ApplicationName);
                        
                        if (exists && !overwrite)
                        {
                            result.SkippedTemplates++;
                            continue;
                        }

                        if (exists)
                        {
                            // Find existing and update
                            var existing = await _templateRepository.GetByKeyAsync(templateRequest.TemplateKey, templateRequest.ApplicationName);
                            if (existing != null)
                            {
                                var updateRequest = new UpdateEmailTemplateRequest
                                {
                                    Name = templateRequest.Name,
                                    Description = templateRequest.Description,
                                    SubjectTemplate = templateRequest.SubjectTemplate,
                                    HtmlBodyTemplate = templateRequest.HtmlBodyTemplate,
                                    TextBodyTemplate = templateRequest.TextBodyTemplate,
                                    Variables = templateRequest.Variables,
                                    CustomStyles = templateRequest.CustomStyles,
                                    SupportedAlertTypes = templateRequest.SupportedAlertTypes,
                                    SupportedSeverityLevels = templateRequest.SupportedSeverityLevels,
                                    CreateNewVersion = true
                                };
                                await UpdateTemplateAsync(existing.Id, updateRequest);
                            }
                        }
                        else
                        {
                            await CreateTemplateAsync(templateRequest);
                        }

                        result.SuccessfulImports++;
                        result.ImportedTemplateKeys.Add($"{templateRequest.ApplicationName ?? "Global"}/{templateRequest.TemplateKey}");
                    }
                    catch (Exception ex)
                    {
                        result.FailedImports++;
                        result.Errors.Add($"Failed to import '{templateRequest.TemplateKey}': {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                result.Errors.Add($"Import failed: {ex.Message}");
            }

            return result;
        }

        public async Task<string> ExportTemplatesAsync(string? applicationName = null, bool includeInactive = false)
        {
            var templates = new List<EmailTemplate>();

            if (applicationName != null)
            {
                var appTemplates = await _templateRepository.GetByApplicationAsync(applicationName, includeInactive);
                templates.AddRange(appTemplates);
            }
            else
            {
                // Export all templates
                var allKeys = await _templateRepository.GetAllTemplateKeysAsync();
                foreach (var key in allKeys)
                {
                    var keyTemplates = await _templateRepository.GetVersionHistoryAsync(key, null);
                    templates.AddRange(includeInactive ? keyTemplates : keyTemplates.Where(t => t.IsActive));
                }
            }

            var exportData = templates.Select(t => new
            {
                t.TemplateKey,
                t.ApplicationName,
                t.Name,
                t.Description,
                t.SubjectTemplate,
                t.HtmlBodyTemplate,
                t.TextBodyTemplate,
                Variables = ParseVariables(t.AvailableVariables),
                t.CustomStyles,
                SupportedAlertTypes = ParseJsonArray<string>(t.SupportedAlertTypes),
                SupportedSeverityLevels = ParseJsonArray<string>(t.SupportedSeverityLevels),
                t.Version,
                t.IsActive,
                t.IsDefault
            });

            return JsonSerializer.Serialize(exportData, new JsonSerializerOptions { WriteIndented = true });
        }

        public async Task<int> SeedDefaultTemplatesAsync(string applicationName)
        {
            var seeded = 0;

            foreach (var templateDef in DefaultEmailTemplates.StandardTemplates)
            {
                if (!await _templateRepository.ExistsAsync(templateDef.Key, applicationName))
                {
                    var request = new CreateEmailTemplateRequest
                    {
                        TemplateKey = templateDef.Key,
                        ApplicationName = applicationName,
                        Name = templateDef.Value.name,
                        Description = templateDef.Value.description,
                        SubjectTemplate = GetDefaultSubjectTemplate(templateDef.Key),
                        HtmlBodyTemplate = GetDefaultHtmlTemplate(templateDef.Key),
                        TextBodyTemplate = GetDefaultTextTemplate(templateDef.Key),
                        Variables = GetDefaultVariablesList(),
                        IsActive = true,
                        IsDefault = true
                    };

                    await CreateTemplateAsync(request);
                    seeded++;
                }
            }

            return seeded;
        }

        #region Private Helper Methods

        private async Task<EmailTemplate?> GetCachedTemplateAsync(string templateKey, string applicationName)
        {
            var cacheKey = $"template:{applicationName ?? "global"}:{templateKey}";
            
            if (!_templateCache.TryGetValue<EmailTemplate>(cacheKey, out var template))
            {
                template = await _templateRepository.GetActiveTemplateAsync(templateKey, applicationName);
                
                if (template != null)
                {
                    _templateCache.Set(cacheKey, template, _cacheExpiration);
                }
            }

            return template;
        }

        private void InvalidateCache(string templateKey, string? applicationName)
        {
            var cacheKey = $"template:{applicationName ?? "global"}:{templateKey}";
            _templateCache.Remove(cacheKey);
        }

        private string DetermineTemplateKey(NotificationMessage notification)
        {
            // Map alert types to template keys
            var alertType = (AlertType)notification.AlertType;
            return alertType switch
            {
                AlertType.System => "SystemAlert",
                AlertType.Security => "SecurityAlert",
                AlertType.Performance => "PerformanceAlert",
                AlertType.Maintenance => "MaintenanceNotice",
                AlertType.HealthCheck => "HealthCheckNotification",
                _ => "SystemAlert"
            };
        }

        private Dictionary<string, object> BuildVariableContext(Dictionary<string, object> variables, EmailTemplate template)
        {
            var context = new Dictionary<string, object>(variables);

            // Add default values for missing variables
            var definedVariables = ParseVariables(template.AvailableVariables);
            foreach (var variable in definedVariables.Where(v => !string.IsNullOrEmpty(v.DefaultValue)))
            {
                if (!context.ContainsKey(variable.Name))
                {
                    context[variable.Name] = variable.DefaultValue!;
                }
            }

            // Ensure required standard variables have values
            if (!context.ContainsKey("Timestamp"))
                context["Timestamp"] = DateTime.UtcNow;
            
            if (!context.ContainsKey("Environment"))
                context["Environment"] = "Unknown";

            return context;
        }

        private string ApplyCustomStyles(string htmlBody, string? customStyles)
        {
            if (string.IsNullOrWhiteSpace(customStyles))
                return htmlBody;

            // Insert custom styles into the HTML head or at the beginning
            if (htmlBody.Contains("<head>", StringComparison.OrdinalIgnoreCase))
            {
                return htmlBody.Replace("</head>", $"<style>{customStyles}</style></head>", StringComparison.OrdinalIgnoreCase);
            }
            else
            {
                return $"<style>{customStyles}</style>{htmlBody}";
            }
        }

        private HashSet<string> ExtractVariablesFromTemplate(EmailTemplate template)
        {
            var variables = new HashSet<string>();
            
            // Extract from all template parts
            ExtractVariablesFromString(template.SubjectTemplate, variables);
            ExtractVariablesFromString(template.HtmlBodyTemplate, variables);
            ExtractVariablesFromString(template.TextBodyTemplate, variables);

            return variables;
        }

        private void ExtractVariablesFromString(string template, HashSet<string> variables)
        {
            // Simple regex to find {{variable}} patterns
            var pattern = @"\{\{([^}]+)\}\}";
            var matches = System.Text.RegularExpressions.Regex.Matches(template, pattern);
            
            foreach (System.Text.RegularExpressions.Match match in matches)
            {
                if (match.Groups.Count > 1)
                {
                    var variable = match.Groups[1].Value.Trim();
                    // Handle helpers and complex expressions
                    if (!variable.Contains(' ') && !variable.Contains('('))
                    {
                        variables.Add(variable);
                    }
                }
            }
        }

        private Dictionary<string, object> GetSampleVariables()
        {
            return new Dictionary<string, object>
            {
                ["Title"] = "Sample Notification Title",
                ["Content"] = "This is a sample notification content for template preview.",
                ["ApplicationName"] = "SampleApp",
                ["ApplicationDisplayName"] = "Sample Application",
                ["Severity"] = "Info",
                ["AlertType"] = "System",
                ["Timestamp"] = DateTime.UtcNow,
                ["Username"] = "sampleuser",
                ["UserDisplayName"] = "Sample User",
                ["UserEmail"] = "sample@example.com",
                ["ActionUrl"] = "https://example.com/action",
                ["EntityType"] = "SampleEntity",
                ["EntityId"] = "12345",
                ["Environment"] = "DEV",
                ["NotificationId"] = Guid.NewGuid().ToString()
            };
        }

        private List<TemplateVariable> GetDefaultVariablesList()
        {
            return DefaultEmailTemplates.StandardVariables.Select(kvp => new TemplateVariable
            {
                Name = kvp.Key,
                Description = kvp.Value,
                IsRequired = false,
                SampleValue = GetSampleVariables().GetValueOrDefault(kvp.Key)?.ToString()
            }).ToList();
        }

        private string SerializeVariables(List<TemplateVariable>? variables)
        {
            if (variables == null || !variables.Any())
                return JsonSerializer.Serialize(GetDefaultVariablesList());
            
            return JsonSerializer.Serialize(variables);
        }

        private List<TemplateVariable> ParseVariables(string? variablesJson)
        {
            if (string.IsNullOrWhiteSpace(variablesJson))
                return new List<TemplateVariable>();

            try
            {
                return JsonSerializer.Deserialize<List<TemplateVariable>>(variablesJson) ?? new List<TemplateVariable>();
            }
            catch
            {
                return new List<TemplateVariable>();
            }
        }

        private List<T> ParseJsonArray<T>(string? json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return new List<T>();

            try
            {
                return JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
            }
            catch
            {
                return new List<T>();
            }
        }

        private EmailTemplateViewModel MapToViewModel(EmailTemplate template)
        {
            return new EmailTemplateViewModel
            {
                Id = template.Id,
                TemplateKey = template.TemplateKey,
                ApplicationName = template.ApplicationName,
                Name = template.Name,
                Description = template.Description,
                SubjectTemplate = template.SubjectTemplate,
                HtmlBodyTemplate = template.HtmlBodyTemplate,
                TextBodyTemplate = template.TextBodyTemplate,
                Variables = ParseVariables(template.AvailableVariables),
                SupportedAlertTypes = ParseJsonArray<string>(template.SupportedAlertTypes),
                SupportedSeverityLevels = ParseJsonArray<string>(template.SupportedSeverityLevels),
                Version = template.Version,
                IsActive = template.IsActive,
                IsDefault = template.IsDefault,
                CreatedAt = template.CreatedAt,
                CreatedBy = template.CreatedBy,
                UpdatedAt = template.UpdatedAt,
                UpdatedBy = template.UpdatedBy
            };
        }

        private string GetDefaultSubjectTemplate(string templateKey)
        {
            return templateKey switch
            {
                "SystemAlert" => "[{{ApplicationDisplayName}}] System Alert: {{Title}}",
                "SecurityAlert" => "[SECURITY] {{ApplicationDisplayName}}: {{Title}}",
                "MaintenanceNotice" => "Scheduled Maintenance: {{ApplicationDisplayName}} - {{Title}}",
                "PerformanceAlert" => "[PERFORMANCE] {{ApplicationDisplayName}}: {{Title}}",
                "ErrorNotification" => "[ERROR] {{ApplicationDisplayName}}: {{Title}}",
                "UserAction" => "Action Required: {{Title}}",
                "StatusUpdate" => "Status Update: {{ApplicationDisplayName}} - {{Title}}",
                "DigestEmail" => "{{ApplicationDisplayName}} Notification Digest - {{formatDate Timestamp 'yyyy-MM-dd'}}",
                _ => "[{{ApplicationDisplayName}}] {{Title}}"
            };
        }

        private string GetDefaultHtmlTemplate(string templateKey)
        {
            var baseTemplate = @"<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>{{Title}}</title>
</head>
<body style=""font-family: Arial, sans-serif; margin: 0; padding: 0; background-color: #f5f5f5;"">
    <div style=""max-width: 600px; margin: 0 auto; background-color: #ffffff; padding: 20px;"">
        <div style=""background-color: {{severityColor Severity}}; color: white; padding: 15px; margin: -20px -20px 20px -20px;"">
            <h1 style=""margin: 0; font-size: 24px;"">{{Title}}</h1>
            <p style=""margin: 5px 0 0 0; font-size: 14px;"">{{ApplicationDisplayName}} - {{formatDate Timestamp}}</p>
        </div>
        
        <div style=""padding: 20px 0;"">
            <div style=""background-color: #f8f9fa; padding: 15px; border-radius: 5px; margin-bottom: 20px;"">
                <table style=""width: 100%; border-collapse: collapse;"">
                    <tr>
                        <td style=""padding: 5px 0;""><strong>Severity:</strong></td>
                        <td style=""padding: 5px 0;"">{{Severity}}</td>
                    </tr>
                    <tr>
                        <td style=""padding: 5px 0;""><strong>Type:</strong></td>
                        <td style=""padding: 5px 0;"">{{AlertType}}</td>
                    </tr>
                    <tr>
                        <td style=""padding: 5px 0;""><strong>Time:</strong></td>
                        <td style=""padding: 5px 0;"">{{formatDate Timestamp 'yyyy-MM-dd HH:mm:ss'}} UTC</td>
                    </tr>
                </table>
            </div>
            
            <div style=""margin: 20px 0;"">
                {{Content}}
            </div>
            
            {{#if ActionUrl}}
            <div style=""text-align: center; margin: 30px 0;"">
                <a href=""{{ActionUrl}}"" style=""display: inline-block; padding: 12px 30px; background-color: #007bff; color: white; text-decoration: none; border-radius: 5px;"">Take Action</a>
            </div>
            {{/if}}
        </div>
        
        <div style=""border-top: 1px solid #e0e0e0; padding-top: 20px; margin-top: 30px; font-size: 12px; color: #666;"">
            <p>This is an automated notification from {{ApplicationDisplayName}}. Please do not reply to this email.</p>
            <p>Notification ID: {{NotificationId}}</p>
        </div>
    </div>
</body>
</html>";

            return baseTemplate;
        }

        private string GetDefaultTextTemplate(string templateKey)
        {
            return @"{{Title}}
{{ApplicationDisplayName}} - {{formatDate Timestamp}}

Severity: {{Severity}}
Type: {{AlertType}}
Time: {{formatDate Timestamp 'yyyy-MM-dd HH:mm:ss'}} UTC

{{Content}}

{{#if ActionUrl}}
Take Action: {{ActionUrl}}
{{/if}}

---
This is an automated notification from {{ApplicationDisplayName}}.
Notification ID: {{NotificationId}}";
        }

        #endregion
    }
}