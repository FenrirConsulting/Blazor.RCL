using Microsoft.AspNetCore.Components;
using Microsoft.Graph.Models;
using Blazor.RCL.Infrastructure.Services.Interfaces;
using System.Text;
using Blazor.RCL.Application.Interfaces;
using Blazor.RCL.Application.Common.Configuration.Interfaces;
using System.Text.Json;
using System.IO;
using Microsoft.Extensions.Logging;
using MudBlazor;

namespace Blazor.RCL.UIComponents.Components
{
    /// <summary>
    /// Represents a sliding help panel component that displays markdown documentation.
    /// </summary>
    public partial class HelpPanel : ComponentBase, IDisposable
    {
        #region Parameters
        /// <summary>
        /// Gets or sets the help document information.
        /// </summary>
        [Parameter]
        public HelpDocumentInfo? HelpDocument { get; set; }

        /// <summary>
        /// Gets or sets whether the help panel is open.
        /// </summary>
        [Parameter]
        public bool IsOpen { get; set; }

        /// <summary>
        /// Gets or sets the callback when IsOpen changes.
        /// </summary>
        [Parameter]
        public EventCallback<bool> IsOpenChanged { get; set; }
        #endregion

        #region Injected Services
        [Inject] private ISharePointService? _sharePointService { get; set; }
        [Inject] private IToolsConfigurationRepository? _toolsRepository { get; set; }
        [Inject] private IAppConfiguration? _appConfiguration { get; set; }
        [Inject] private ILogger<HelpPanel>? _logger { get; set; }
        #endregion

        #region Private Fields
        private DriveItem? _document;
        private string _markdownContent = string.Empty;
        private bool _isLoading = true;
        private bool _hasError = false;
        #endregion

        #region Properties
        /// <summary>
        /// Gets whether the help document is available.
        /// </summary>
        private bool IsHelpAvailable => HelpDocument != null && 
                                       !string.IsNullOrEmpty(HelpDocument.DocumentLibrary) && 
                                       !string.IsNullOrEmpty(HelpDocument.Document);
        #endregion

        #region Lifecycle Methods
        /// <summary>
        /// Initializes the component and retrieves the help document when parameters are set.
        /// </summary>
        protected override async Task OnParametersSetAsync()
        {
            try 
            {
                // Load content when panel is opened or when HelpDocument info changes
                if (IsOpen && (string.IsNullOrEmpty(_markdownContent) || _previousHelpDocument != HelpDocument))
                {
                    _previousHelpDocument = HelpDocument;
                    _isLoading = true;
                    StateHasChanged();
                    await LoadHelpDocument();
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error in OnParametersSetAsync");
            }
        }
        
        private HelpDocumentInfo? _previousHelpDocument;
        #endregion

        #region Public Methods
        /// <summary>
        /// Closes the help panel drawer.
        /// </summary>
        public async Task CloseDrawer()
        {
            if (IsOpen)
            {
                IsOpen = false;
                StateHasChanged();
                await IsOpenChanged.InvokeAsync(IsOpen);
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Loads the help document from SharePoint or local file.
        /// </summary>
        private async Task LoadHelpDocument()
        {
            try
            {
                _isLoading = true;
                _hasError = false;
                _markdownContent = string.Empty;
                StateHasChanged();

                // Try to load from local file first for development environments
                if (TryLoadLocalMarkdownFile())
                {
                    _logger?.LogInformation("Loaded help documentation from local file");
                    _isLoading = false;
                    StateHasChanged();
                    return;
                }

                // Try to use directly provided HelpDocument parameter
                if (HelpDocument != null && 
                    !string.IsNullOrEmpty(HelpDocument.DocumentLibrary) && 
                    !string.IsNullOrEmpty(HelpDocument.Document))
                {
                    _logger?.LogInformation($"Loading documentation from SharePoint: {HelpDocument.DocumentLibrary}/{HelpDocument.Document}");
                    await LoadFromSharePoint(HelpDocument.DocumentLibrary, HelpDocument.Document);
                }
                // Fall back to configuration if parameter not provided
                else
                {
                    _logger?.LogInformation("No HelpDocument parameter provided, trying to load from configuration");
                    var helpDocumentConfig = await _toolsRepository!.GetValueAsync(_appConfiguration!.AppName, "HelpDocument");

                    if (string.IsNullOrEmpty(helpDocumentConfig))
                    {
                        _logger?.LogWarning("Help document configuration not found");
                        _isLoading = false;
                        StateHasChanged();
                        return;
                    }

                    try
                    {
                        var helpDocumentInfo = JsonSerializer.Deserialize<HelpDocumentInfo>(helpDocumentConfig);

                        if (helpDocumentInfo == null)
                        {
                            _logger?.LogWarning("Invalid help document configuration JSON");
                            _isLoading = false;
                            StateHasChanged();
                            return;
                        }

                        _logger?.LogInformation($"Loaded from config: {helpDocumentInfo.DocumentLibrary}/{helpDocumentInfo.Document}");
                        await LoadFromSharePoint(helpDocumentInfo.DocumentLibrary, helpDocumentInfo.Document);
                    }
                    catch (JsonException ex)
                    {
                        _logger?.LogError(ex, "Error parsing help document configuration JSON");
                        _hasError = true;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error loading help document");
                _hasError = true;
            }
            finally
            {
                _isLoading = false;
                StateHasChanged();
            }
        }

        /// <summary>
        /// Loads the document from SharePoint.
        /// </summary>
        private async Task LoadFromSharePoint(string libraryName, string documentName)
        {
            if (string.IsNullOrEmpty(libraryName) || string.IsNullOrEmpty(documentName))
            {
                _logger?.LogWarning("Library name or document name is empty");
                return;
            }

            var documents = await _sharePointService!.GetDocumentsAsync(libraryName);
            _document = documents.FirstOrDefault(d => d.Name!.Equals(documentName, StringComparison.OrdinalIgnoreCase));

            if (_document != null)
            {
                using var stream = await _sharePointService.DownloadDocumentAsync(_document.Id!);
                if (stream != null)
                {
                    using var reader = new StreamReader(stream);
                    _markdownContent = await reader.ReadToEndAsync();
                }
                else
                {
                    _logger?.LogWarning("Document stream is null");
                }
            }
            else
            {
                _logger?.LogWarning("Document not found in SharePoint library");
            }
        }

        /// <summary>
        /// Attempts to load a local markdown file for development environments.
        /// </summary>
        /// <returns>True if the local file was successfully loaded, false otherwise.</returns>
        private bool TryLoadLocalMarkdownFile()
        {
            try
            {
                // Check for local markdown file (useful for development)
                string localFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EnableDisableDocumentation.md");
                
                if (File.Exists(localFilePath))
                {
                    _markdownContent = File.ReadAllText(localFilePath);
                    return true;
                }

                // Try one directory up (project root)
                localFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "EnableDisableDocumentation.md");
                if (File.Exists(localFilePath))
                {
                    _markdownContent = File.ReadAllText(localFilePath);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger?.LogWarning(ex, "Error loading local markdown file, falling back to SharePoint");
                return false;
            }
        }
        #endregion

        #region IDisposable Implementation
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            // Dispose of managed resources
            _document = null;
            _markdownContent = string.Empty;
        }
        #endregion
    }

}