using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Blazor.RCL.Domain.Entities.Notifications;
using Blazor.RCL.Application.Models.Notifications;

namespace Blazor.RCL.UIComponents.Components.Admin.Dialogs
{
    public partial class EditTemplateDialog : ComponentBase
    {
        [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = default!;
        [Inject] private IJSRuntime JSRuntime { get; set; } = default!;
        [Inject] private ISnackbar Snackbar { get; set; } = default!;
        
        [Parameter] public EmailTemplateModel Model { get; set; } = new();
        [Parameter] public bool IsNew { get; set; }
        [Parameter] public Func<EmailTemplate, Task<RenderedEmailContent?>>? OnPreviewRequested { get; set; }
        [Parameter] public Dictionary<string, string> AvailableVariables { get; set; } = new();
        
        private RenderedEmailContent? PreviewContent { get; set; }
        
        private void Submit() => MudDialog.Close(DialogResult.Ok(Model));
        private void Cancel() => MudDialog.Cancel();
        
        private async Task GeneratePreview()
        {
            if (OnPreviewRequested != null)
            {
                var tempTemplate = new EmailTemplate
                {
                    SubjectTemplate = Model.SubjectTemplate,
                    HtmlBodyTemplate = Model.HtmlBodyTemplate,
                    TextBodyTemplate = Model.TextBodyTemplate,
                    CustomStyles = Model.CustomStyles
                };
                
                PreviewContent = await OnPreviewRequested(tempTemplate);
            }
        }
        
        private async Task CopyVariable(string variable)
        {
            var text = $"{{{{{variable}}}}}";
            await JSRuntime.InvokeVoidAsync("navigator.clipboard.writeText", text);
            Snackbar.Add($"Copied {text} to clipboard", Severity.Info);
        }
        
        private async Task CopyHelper(string helper)
        {
            await JSRuntime.InvokeVoidAsync("navigator.clipboard.writeText", helper);
            Snackbar.Add("Helper copied to clipboard!", Severity.Success);
        }
        
        private List<HelperInfo> GetHelpers()
        {
            return new List<HelperInfo>
            {
                new HelperInfo { Name = "if", Description = "Conditional rendering", Example = "{{#if condition}}...{{/if}}" },
                new HelperInfo { Name = "unless", Description = "Inverse conditional rendering", Example = "{{#unless condition}}...{{/unless}}" },
                new HelperInfo { Name = "each", Description = "Iterate over arrays", Example = "{{#each items}}...{{/each}}" },
                new HelperInfo { Name = "with", Description = "Change context", Example = "{{#with object}}...{{/with}}" },
                new HelperInfo { Name = "lookup", Description = "Dynamic property access", Example = "{{lookup object property}}" },
                new HelperInfo { Name = "eq", Description = "Equality comparison", Example = "{{#if (eq value 'test')}}...{{/if}}" },
                new HelperInfo { Name = "ne", Description = "Not equal comparison", Example = "{{#if (ne value 'test')}}...{{/if}}" },
                new HelperInfo { Name = "gt", Description = "Greater than comparison", Example = "{{#if (gt value 5)}}...{{/if}}" },
                new HelperInfo { Name = "lt", Description = "Less than comparison", Example = "{{#if (lt value 5)}}...{{/if}}" },
                new HelperInfo { Name = "and", Description = "Logical AND", Example = "{{#if (and condition1 condition2)}}...{{/if}}" },
                new HelperInfo { Name = "or", Description = "Logical OR", Example = "{{#if (or condition1 condition2)}}...{{/if}}" }
            };
        }
        
        public class EmailTemplateModel
        {
            public string TemplateKey { get; set; } = string.Empty;
            public string? ApplicationName { get; set; }
            public string Name { get; set; } = string.Empty;
            public string? Description { get; set; }
            public string SubjectTemplate { get; set; } = string.Empty;
            public string HtmlBodyTemplate { get; set; } = string.Empty;
            public string TextBodyTemplate { get; set; } = string.Empty;
            public string? CustomStyles { get; set; }
            public IEnumerable<AlertType> SupportedAlertTypes { get; set; } = new HashSet<AlertType>();
            public bool IsActive { get; set; }
            public bool IsDefault { get; set; }
            public bool CreateNewVersion { get; set; }
        }

        public class HelperInfo
        {
            public string Name { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public string Example { get; set; } = string.Empty;
        }
    }
}