using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Blazor.RCL.Application.Interfaces;
using Blazor.RCL.Application.Models.Notifications;
using Blazor.RCL.Domain.Entities.Notifications;
using Blazor.RCL.NLog.LogService.Interface;
using Blazor.RCL.UIComponents.Components.Admin.Dialogs;

namespace Blazor.RCL.UIComponents.Components.Admin
{
    public partial class ApplicationProfileAdmin : ComponentBase
    {
        [Parameter] public string ApplicationName { get; set; } = default!;
        
        [Inject] private IApplicationNotificationProfileService ProfileService { get; set; } = default!;
        [Inject] private IEmailTemplateService TemplateService { get; set; } = default!;
        [Inject] private ISnackbar Snackbar { get; set; } = default!;
        [Inject] private ILogHelper Logger { get; set; } = default!;
        [Inject] private IJSRuntime JSRuntime { get; set; } = default!;
        [Inject] private IDialogService DialogService { get; set; } = default!;

        // Component state
        private bool IsLoading { get; set; }
        private ApplicationNotificationProfile? _currentApplication;
        private List<EmailTemplateViewModel> _emailTemplates = new();
        private Dictionary<string, string> _availableVariables = new();

        // Template grid state
        private string _searchText = string.Empty;
        private AlertType? _filterAlertType;
        private bool _showInactive = false;
        private bool _showGallery = false;

        // Import state
        private string _importJson = string.Empty;
        private bool _overwriteExisting = false;
        private List<ImportResult> _importResults = new();

        // Filtered templates property
        private IEnumerable<EmailTemplateViewModel> FilteredTemplates
        {
            get
            {
                var filtered = _emailTemplates.AsEnumerable();

                if (!string.IsNullOrWhiteSpace(_searchText))
                {
                    filtered = filtered.Where(t => 
                        t.Name.Contains(_searchText, StringComparison.OrdinalIgnoreCase) ||
                        t.TemplateKey.Contains(_searchText, StringComparison.OrdinalIgnoreCase) ||
                        (t.Description?.Contains(_searchText, StringComparison.OrdinalIgnoreCase) ?? false));
                }

                if (_filterAlertType.HasValue)
                {
                    filtered = filtered.Where(t => 
                        t.SupportedAlertTypes?.Contains(_filterAlertType.Value.ToString()) ?? false);
                }

                if (!_showInactive)
                {
                    filtered = filtered.Where(t => t.IsActive);
                }

                return filtered.OrderBy(t => t.Name);
            }
        }

        protected override async Task OnInitializedAsync()
        {
            await LoadData();
        }

        private async Task LoadData()
        {
            IsLoading = true;
            try
            {
                // Load current application profile and templates in parallel
                var profileTask = LoadCurrentApplicationProfile();
                var templateTask = LoadTemplates();
                var variablesTask = LoadVariables();

                await Task.WhenAll(profileTask, templateTask, variablesTask);
            }
            catch (Exception ex)
            {
                Logger.LogError("Failed to load data", ex);
                Snackbar.Add("Failed to load data", Severity.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task LoadCurrentApplicationProfile()
        {
            try
            {
                _currentApplication = await ProfileService.GetApplicationProfileAsync(ApplicationName);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Failed to load application profile for {ApplicationName}", ex);
            }
        }

        private async Task LoadTemplates()
        {
            try
            {
                // Load templates for the current application only
                var templates = await TemplateService.GetApplicationTemplatesAsync(ApplicationName, includeGlobal: true);
                _emailTemplates = templates.ToList();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Failed to load templates for {ApplicationName}", ex);
            }
        }

        private async Task LoadVariables()
        {
            _availableVariables = await TemplateService.GetDefaultVariablesAsync();
        }

        #region Template Management

        private async Task OpenTemplateDialog()
        {
            var model = new EditTemplateDialog.EmailTemplateModel
            {
                ApplicationName = ApplicationName,
                IsActive = true,
                SupportedAlertTypes = new List<AlertType>()
            };

            var parameters = new DialogParameters<EditTemplateDialog>
            {
                { x => x.Model, model },
                { x => x.IsNew, true },
                { x => x.AvailableVariables, _availableVariables },
                { x => x.OnPreviewRequested, async (template) => await GenerateTemplatePreview(template) }
            };

            var options = new DialogOptions 
            { 
                MaxWidth = MaxWidth.Large, 
                FullWidth = true,
                CloseOnEscapeKey = true 
            };

            var dialog = await DialogService.ShowAsync<EditTemplateDialog>("Create New Template", parameters, options);
            var result = await dialog.Result;

            if (!result.Canceled && result.Data != null)
            {
                var templateModel = (EditTemplateDialog.EmailTemplateModel)result.Data;
                await CreateTemplate(templateModel);
            }
        }

        private async Task EditTemplate(EmailTemplateViewModel template)
        {
            Logger.LogMessage($"EditTemplate called for: {template.Name}");
            
            var model = new EditTemplateDialog.EmailTemplateModel
            {
                TemplateKey = template.TemplateKey,
                ApplicationName = template.ApplicationName,
                Name = template.Name,
                Description = template.Description,
                SubjectTemplate = template.SubjectTemplate,
                HtmlBodyTemplate = template.HtmlBodyTemplate,
                TextBodyTemplate = template.TextBodyTemplate,
                CustomStyles = string.Empty,
                SupportedAlertTypes = template.SupportedAlertTypes?.Select(s => Enum.Parse<AlertType>(s)).ToList() ?? new List<AlertType>(),
                IsActive = template.IsActive,
                IsDefault = template.IsDefault,
                CreateNewVersion = false
            };

            var parameters = new DialogParameters<EditTemplateDialog>
            {
                { x => x.Model, model },
                { x => x.IsNew, false },
                { x => x.AvailableVariables, _availableVariables },
                { x => x.OnPreviewRequested, async (t) => await GenerateTemplatePreview(t) }
            };

            var options = new DialogOptions 
            { 
                MaxWidth = MaxWidth.Large, 
                FullWidth = true,
                CloseOnEscapeKey = true 
            };

            var dialog = await DialogService.ShowAsync<EditTemplateDialog>($"Edit Template: {template.Name}", parameters, options);
            var result = await dialog.Result;

            if (!result.Canceled && result.Data != null)
            {
                var templateModel = (EditTemplateDialog.EmailTemplateModel)result.Data;
                await UpdateTemplate(template.Id, templateModel);
            }
        }

        private async Task CreateTemplate(EditTemplateDialog.EmailTemplateModel model)
        {
            try
            {
                var request = new CreateEmailTemplateRequest
                {
                    TemplateKey = model.TemplateKey,
                    ApplicationName = ApplicationName,
                    Name = model.Name,
                    Description = model.Description,
                    SubjectTemplate = model.SubjectTemplate,
                    HtmlBodyTemplate = model.HtmlBodyTemplate,
                    TextBodyTemplate = model.TextBodyTemplate,
                    CustomStyles = model.CustomStyles,
                    SupportedAlertTypes = model.SupportedAlertTypes.ToList(),
                    IsActive = model.IsActive,
                    IsDefault = model.IsDefault
                };

                await TemplateService.CreateTemplateAsync(request);
                Snackbar.Add("Email template created successfully", Severity.Success);
                await LoadTemplates();
            }
            catch (Exception ex)
            {
                Logger.LogError("Failed to create template", ex);
                Snackbar.Add($"Failed to create template: {ex.Message}", Severity.Error);
            }
        }

        private async Task UpdateTemplate(Guid templateId, EditTemplateDialog.EmailTemplateModel model)
        {
            try
            {
                var request = new UpdateEmailTemplateRequest
                {
                    Name = model.Name,
                    Description = model.Description,
                    SubjectTemplate = model.SubjectTemplate,
                    HtmlBodyTemplate = model.HtmlBodyTemplate,
                    TextBodyTemplate = model.TextBodyTemplate,
                    CustomStyles = model.CustomStyles,
                    SupportedAlertTypes = model.SupportedAlertTypes.ToList(),
                    CreateNewVersion = model.CreateNewVersion
                };

                await TemplateService.UpdateTemplateAsync(templateId, request);
                Snackbar.Add("Email template updated successfully", Severity.Success);
                await LoadTemplates();
            }
            catch (Exception ex)
            {
                Logger.LogError("Failed to update template", ex);
                Snackbar.Add($"Failed to update template: {ex.Message}", Severity.Error);
            }
        }

        private async Task<RenderedEmailContent?> GenerateTemplatePreview(EmailTemplate template)
        {
            try
            {
                var validation = await TemplateService.ValidateTemplateAsync(template);
                
                if (validation.IsValid && validation.PreviewContent != null)
                {
                    return validation.PreviewContent;
                }
                else
                {
                    var errors = string.Join(", ", validation.Errors);
                    Snackbar.Add($"Template validation failed: {errors}", Severity.Error);
                    return null;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Failed to generate preview", ex);
                Snackbar.Add($"Failed to generate preview: {ex.Message}", Severity.Error);
                return null;
            }
        }


        private async Task PreviewTemplate(EmailTemplateViewModel template)
        {
            try
            {
                RenderedEmailContent? previewContent;
                
                // Try to get preview content
                try
                {
                    previewContent = await TemplateService.PreviewTemplateAsync(template.Id);
                }
                catch
                {
                    // If preview fails, create a simple preview from the template data
                    previewContent = new RenderedEmailContent
                    {
                        Subject = template.SubjectTemplate,
                        HtmlBody = template.HtmlBodyTemplate,
                        TextBody = template.TextBodyTemplate
                    };
                }

                var parameters = new DialogParameters<PreviewTemplateDialog>
                {
                    { x => x.TemplateName, template.Name },
                    { x => x.PreviewContent, previewContent },
                    { x => x.OnSendTestEmail, async () => await SendTestEmail() }
                };

                var options = new DialogOptions 
                { 
                    MaxWidth = MaxWidth.Large, 
                    FullWidth = true,
                    CloseOnEscapeKey = true 
                };

                await DialogService.ShowAsync<PreviewTemplateDialog>("Template Preview", parameters, options);
            }
            catch (Exception ex)
            {
                Logger.LogError("Failed to preview template", ex);
                Snackbar.Add($"Failed to preview template: {ex.Message}", Severity.Error);
            }
        }


        private async Task CloneTemplate(EmailTemplateViewModel template)
        {
            var model = new EditTemplateDialog.EmailTemplateModel
            {
                TemplateKey = $"{template.TemplateKey}_Copy",
                ApplicationName = template.ApplicationName,
                Name = $"{template.Name} (Copy)",
                Description = template.Description,
                SubjectTemplate = template.SubjectTemplate,
                HtmlBodyTemplate = template.HtmlBodyTemplate,
                TextBodyTemplate = template.TextBodyTemplate,
                CustomStyles = string.Empty,
                SupportedAlertTypes = template.SupportedAlertTypes?.Select(s => Enum.Parse<AlertType>(s)).ToList() ?? new List<AlertType>(),
                IsActive = true,
                IsDefault = false,
                CreateNewVersion = false
            };

            var parameters = new DialogParameters<EditTemplateDialog>
            {
                { x => x.Model, model },
                { x => x.IsNew, true },
                { x => x.AvailableVariables, _availableVariables },
                { x => x.OnPreviewRequested, async (t) => await GenerateTemplatePreview(t) }
            };

            var options = new DialogOptions 
            { 
                MaxWidth = MaxWidth.Large, 
                FullWidth = true,
                CloseOnEscapeKey = true 
            };

            var dialog = await DialogService.ShowAsync<EditTemplateDialog>("Create Template Copy", parameters, options);
            var result = await dialog.Result;

            if (!result.Canceled && result.Data != null)
            {
                var templateModel = (EditTemplateDialog.EmailTemplateModel)result.Data;
                await CreateTemplate(templateModel);
            }
        }

        // Delete functionality not available in current service interface
        /*
        private async Task DeleteTemplate(EmailTemplateViewModel template)
        {
            try
            {
                await TemplateService.DeleteTemplateAsync(template.Id);
                Snackbar.Add($"Template '{template.Name}' deleted successfully", Severity.Success);
                await LoadTemplates();
            }
            catch (Exception ex)
            {
                Logger.LogError("Failed to delete template", ex);
                Snackbar.Add($"Failed to delete template: {ex.Message}", Severity.Error);
            }
        }
        */

        private async Task ShowTemplateHistory(EmailTemplateViewModel template)
        {
            try
            {
                var historyTemplateKey = $"{template.TemplateKey} ({template.ApplicationName ?? "Global"})";
                var templateHistory = (await TemplateService.GetTemplateHistoryAsync(template.TemplateKey, template.ApplicationName)).ToList();
                
                // For now, show history in a simple message
                // TODO: Create a dedicated history dialog component
                Snackbar.Add($"Template '{historyTemplateKey}' has {templateHistory.Count} versions", Severity.Info);
            }
            catch (Exception ex)
            {
                Logger.LogError("Failed to load template history", ex);
                Snackbar.Add($"Failed to load template history: {ex.Message}", Severity.Error);
            }
        }

        private async Task ActivateTemplate(EmailTemplateViewModel template)
        {
            try
            {
                await TemplateService.ActivateTemplateAsync(template.Id);
                Snackbar.Add($"Template '{template.Name}' activated", Severity.Success);
                await LoadTemplates();
            }
            catch (Exception ex)
            {
                Logger.LogError("Failed to activate template", ex);
                Snackbar.Add($"Failed to activate template: {ex.Message}", Severity.Error);
            }
        }

        private async Task HandleTemplateStatusChange(EmailTemplateViewModel template)
        {
            if (template.IsActive)
            {
                await DeactivateTemplate(template);
            }
            else
            {
                await ActivateTemplate(template);
            }
        }

        private async Task DeactivateTemplate(EmailTemplateViewModel template)
        {
            try
            {
                await TemplateService.DeactivateTemplateAsync(template.Id);
                Snackbar.Add($"Template '{template.Name}' deactivated", Severity.Info);
                await LoadTemplates();
            }
            catch (Exception ex)
            {
                Logger.LogError("Failed to deactivate template", ex);
                Snackbar.Add($"Failed to deactivate template: {ex.Message}", Severity.Error);
            }
        }

        private async Task SendTestEmail()
        {
            // TODO: Implement test email functionality
            Snackbar.Add("Test email functionality not yet implemented", Severity.Info);
        }

        private async Task CopyVariable(string variable)
        {
            var text = $"{{{{{variable}}}}}";
            await JSRuntime.InvokeVoidAsync("navigator.clipboard.writeText", text);
            Snackbar.Add($"Copied {text} to clipboard", Severity.Info);
        }

        private void ClearImportResults()
        {
            _importResults.Clear();
            StateHasChanged();
        }

        private void ShowTemplateGallery()
        {
            _showGallery = true;
            StateHasChanged();
        }

        private void HideTemplateGallery()
        {
            _showGallery = false;
            StateHasChanged();
        }

        private async Task UseStarterTemplate(StarterTemplates.StarterTemplate starterTemplate)
        {
            var model = new EditTemplateDialog.EmailTemplateModel
            {
                TemplateKey = $"{starterTemplate.Key}_{ApplicationName.ToUpper().Replace(" ", "_")}",
                ApplicationName = ApplicationName,
                Name = starterTemplate.Name,
                Description = starterTemplate.Description,
                SubjectTemplate = starterTemplate.SubjectTemplate,
                HtmlBodyTemplate = starterTemplate.HtmlBodyTemplate,
                TextBodyTemplate = starterTemplate.TextBodyTemplate,
                CustomStyles = string.Empty,
                SupportedAlertTypes = starterTemplate.SupportedAlertTypes,
                IsActive = true,
                IsDefault = false,
                CreateNewVersion = false
            };

            var parameters = new DialogParameters<EditTemplateDialog>
            {
                { x => x.Model, model },
                { x => x.IsNew, true },
                { x => x.AvailableVariables, _availableVariables },
                { x => x.OnPreviewRequested, async (template) => await GenerateTemplatePreview(template) }
            };

            var options = new DialogOptions 
            { 
                MaxWidth = MaxWidth.Large, 
                FullWidth = true,
                CloseOnEscapeKey = true 
            };

            var dialog = await DialogService.ShowAsync<EditTemplateDialog>($"Customize Template: {starterTemplate.Name}", parameters, options);
            var result = await dialog.Result;

            if (!result.Canceled && result.Data != null)
            {
                var templateModel = (EditTemplateDialog.EmailTemplateModel)result.Data;
                await CreateTemplate(templateModel);
                _showGallery = false;
            }
        }

        private string GetIconValue(string iconPath)
        {
            // Convert icon path like "Material.Filled.Security" to actual icon value
            var parts = iconPath.Split('.');
            if (parts.Length == 3 && parts[0] == "Material" && parts[1] == "Filled")
            {
                var iconName = parts[2];
                var iconType = typeof(Icons.Material.Filled);
                var property = iconType.GetProperty(iconName);
                if (property != null)
                {
                    return property.GetValue(null)?.ToString() ?? Icons.Material.Filled.Email;
                }
            }
            return Icons.Material.Filled.Email;
        }

        #endregion

        #region Import/Export

        private async Task HandleFileSelected(InputFileChangeEventArgs e)
        {
            try
            {
                var file = e.File;
                if (file != null)
                {
                    using var stream = file.OpenReadStream();
                    using var reader = new StreamReader(stream);
                    _importJson = await reader.ReadToEndAsync();
                    StateHasChanged();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Failed to read import file", ex);
                Snackbar.Add($"Failed to read file: {ex.Message}", Severity.Error);
            }
        }

        private async Task ImportTemplates()
        {
            if (string.IsNullOrWhiteSpace(_importJson))
            {
                Snackbar.Add("Please provide JSON content to import", Severity.Warning);
                return;
            }

            _importResults.Clear();

            try
            {
                var templates = JsonSerializer.Deserialize<List<EmailTemplateImportModel>>(_importJson);
                if (templates == null || !templates.Any())
                {
                    Snackbar.Add("No valid templates found in JSON", Severity.Warning);
                    return;
                }

                foreach (var template in templates)
                {
                    var result = new ImportResult { TemplateName = template.Name };

                    try
                    {
                        // Check if template exists
                        var existing = _emailTemplates.FirstOrDefault(t => 
                            t.TemplateKey == template.TemplateKey && 
                            t.ApplicationName == ApplicationName);

                        if (existing != null && !_overwriteExisting)
                        {
                            result.Success = false;
                            result.Message = "Template already exists (enable overwrite to update)";
                        }
                        else
                        {
                            if (existing != null)
                            {
                                // Update existing
                                var updateRequest = new UpdateEmailTemplateRequest
                                {
                                    Name = template.Name,
                                    Description = template.Description,
                                    SubjectTemplate = template.SubjectTemplate,
                                    HtmlBodyTemplate = template.HtmlBodyTemplate,
                                    TextBodyTemplate = template.TextBodyTemplate,
                                    CustomStyles = template.CustomStyles,
                                    SupportedAlertTypes = template.SupportedAlertTypes,
                                    CreateNewVersion = true
                                };
                                await TemplateService.UpdateTemplateAsync(existing.Id, updateRequest);
                                result.Success = true;
                                result.Message = "Template updated successfully";
                            }
                            else
                            {
                                // Create new
                                var createRequest = new CreateEmailTemplateRequest
                                {
                                    TemplateKey = template.TemplateKey,
                                    ApplicationName = ApplicationName,
                                    Name = template.Name,
                                    Description = template.Description,
                                    SubjectTemplate = template.SubjectTemplate,
                                    HtmlBodyTemplate = template.HtmlBodyTemplate,
                                    TextBodyTemplate = template.TextBodyTemplate,
                                    CustomStyles = template.CustomStyles,
                                    SupportedAlertTypes = template.SupportedAlertTypes,
                                    IsActive = template.IsActive,
                                    IsDefault = template.IsDefault
                                };
                                await TemplateService.CreateTemplateAsync(createRequest);
                                result.Success = true;
                                result.Message = "Template created successfully";
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        result.Success = false;
                        result.Message = ex.Message;
                    }

                    _importResults.Add(result);
                }

                await LoadTemplates();
                Snackbar.Add($"Import completed: {_importResults.Count(r => r.Success)} succeeded, {_importResults.Count(r => !r.Success)} failed", 
                    _importResults.Any(r => !r.Success) ? Severity.Warning : Severity.Success);
            }
            catch (Exception ex)
            {
                Logger.LogError("Failed to import templates", ex);
                Snackbar.Add($"Failed to import templates: {ex.Message}", Severity.Error);
            }
        }

        #endregion

        #region Models

        private class EmailTemplateImportModel
        {
            public string TemplateKey { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
            public string? Description { get; set; }
            public string SubjectTemplate { get; set; } = string.Empty;
            public string HtmlBodyTemplate { get; set; } = string.Empty;
            public string TextBodyTemplate { get; set; } = string.Empty;
            public string? CustomStyles { get; set; }
            public List<AlertType> SupportedAlertTypes { get; set; } = new();
            public bool IsActive { get; set; }
            public bool IsDefault { get; set; }
        }

        private class ImportResult
        {
            public string TemplateName { get; set; } = string.Empty;
            public bool Success { get; set; }
            public string Message { get; set; } = string.Empty;
        }

        #endregion
    }
}