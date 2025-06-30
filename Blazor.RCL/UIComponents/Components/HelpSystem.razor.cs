using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Blazor.RCL.UIComponents.Components
{
    /// <summary>
    /// Represents the configuration for the help document.
    /// </summary>
    public class HelpDocumentInfo
    {
        /// <summary>
        /// Gets or sets the name of the SharePoint document library containing the help document.
        /// </summary>
        public string DocumentLibrary { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the name of the help document file.
        /// </summary>
        public string Document { get; set; } = string.Empty;
        
        public override bool Equals(object? obj)
        {
            if (obj is not HelpDocumentInfo other)
                return false;
                
            return DocumentLibrary == other.DocumentLibrary && 
                   Document == other.Document;
        }
        
        public override int GetHashCode()
        {
            return HashCode.Combine(DocumentLibrary, Document);
        }
        
        public static bool operator ==(HelpDocumentInfo? left, HelpDocumentInfo? right)
        {
            if (left is null && right is null)
                return true;
            if (left is null || right is null)
                return false;
                
            return left.Equals(right);
        }
        
        public static bool operator !=(HelpDocumentInfo? left, HelpDocumentInfo? right)
        {
            return !(left == right);
        }
    }

    /// <summary>
    /// A component that provides a help system including the help panel and any help buttons.
    /// This component should be placed at the root level of your application.
    /// </summary>
    public partial class HelpSystem : ComponentBase
    {
        #region Parameters
        /// <summary>
        /// Gets or sets the help document information.
        /// </summary>
        [Parameter]
        public HelpDocumentInfo? HelpDocument { get; set; }

        /// <summary>
        /// Gets or sets the child content of the component.
        /// </summary>
        [Parameter]
        public RenderFragment? ChildContent { get; set; }
        #endregion

        #region Injected Services
        [Inject] private ILogger<HelpSystem>? _logger { get; set; }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets whether the help panel is open.
        /// </summary>
        public bool IsHelpPanelOpen { get; set; }

        /// <summary>
        /// Gets whether help documentation is available.
        /// </summary>
        public bool IsHelpAvailable => HelpDocument != null && 
                                      !string.IsNullOrEmpty(HelpDocument.DocumentLibrary) && 
                                      !string.IsNullOrEmpty(HelpDocument.Document);
        #endregion

        #region Public Methods
        /// <summary>
        /// Opens the help panel.
        /// </summary>
        public void OpenHelpPanel()
        {
            IsHelpPanelOpen = true;
            StateHasChanged();
        }

        /// <summary>
        /// Closes the help panel.
        /// </summary>
        public void CloseHelpPanel()
        {
            IsHelpPanelOpen = false;
            StateHasChanged();
        }

        /// <summary>
        /// Toggles the help panel open/closed state.
        /// </summary>
        public void ToggleHelpPanel()
        {
            if (!IsHelpAvailable)
            {
                _logger?.LogWarning("Help button clicked but no documentation is available");
                return;
            }
            
            IsHelpPanelOpen = !IsHelpPanelOpen;
            _logger?.LogInformation($"Help panel toggled to {(IsHelpPanelOpen ? "open" : "closed")}");
            StateHasChanged();
        }

        /// <summary>
        /// Sets whether the help panel is open.
        /// </summary>
        public Task SetHelpPanelOpenState(bool isOpen)
        {
            IsHelpPanelOpen = isOpen;
            StateHasChanged();
            return Task.CompletedTask;
        }
        #endregion
    }
}