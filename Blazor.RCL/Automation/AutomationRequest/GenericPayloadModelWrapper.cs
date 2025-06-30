using Blazor.RCL.Automation.AutomationRequest.Interfaces;

namespace Blazor.RCL.Automation.AutomationRequest
{
    /// <summary>
    /// A wrapper class that implements IRequestPayloadModel to allow generic handling of request payloads.
    /// </summary>
    public class GenericPayloadModelWrapper : IRequestPayloadModel
    {
        /// <summary>
        /// Gets or sets the original data object.
        /// </summary>
        public object OriginalData { get; set; }
    }
}
