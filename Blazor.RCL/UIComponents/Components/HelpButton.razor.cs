using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Threading.Tasks;

namespace Blazor.RCL.UIComponents.Components
{
    /// <summary>
    /// Reusable help button component that toggles the help panel.
    /// </summary>
    public partial class HelpButton : ComponentBase
    {
        #region Parameters
        /// <summary>
        /// Gets or sets the icon to display for the help button.
        /// </summary>
        [Parameter]
        public string Icon { get; set; } = Icons.Material.Filled.Help;

        /// <summary>
        /// Gets or sets the color of the help button.
        /// </summary>
        [Parameter]
        public Color Color { get; set; } = Color.Primary;

        /// <summary>
        /// Gets or sets the size of the help button.
        /// </summary>
        [Parameter]
        public Size Size { get; set; } = Size.Large;

        /// <summary>
        /// Gets or sets the additional CSS class for the help button.
        /// </summary>
        [Parameter]
        public string Class { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the title (tooltip) for the help button.
        /// </summary>
        [Parameter]
        public string Title { get; set; } = "Help";

        /// <summary>
        /// Gets or sets the aria-label for accessibility.
        /// </summary>
        [Parameter]
        public string AriaLabel { get; set; } = "Open help documentation";

        /// <summary>
        /// Gets or sets whether to show a text label next to the icon.
        /// </summary>
        [Parameter]
        public bool ShowLabel { get; set; } = false;

        /// <summary>
        /// Gets or sets the text to display as the label.
        /// </summary>
        [Parameter]
        public string Label { get; set; } = "Help";
        #endregion

        #region Cascading Parameters
        /// <summary>
        /// Gets or sets the help system to interact with.
        /// </summary>
        [CascadingParameter]
        private HelpSystem? HelpSystem { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// Toggles the help panel open/closed state.
        /// </summary>
        private void ToggleHelpPanel()
        {
            if (HelpSystem == null)
            {
                Console.WriteLine("HelpButton: HelpSystem cascading parameter is null. Make sure the HelpButton is inside a HelpSystem component.");
                return;
            }
            
            HelpSystem.ToggleHelpPanel();
        }
        #endregion
    }
}