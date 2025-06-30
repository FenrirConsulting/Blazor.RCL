using Blazor.RCL.Automation.AutomationRequest.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blazor.RCL.Automation.Services.Interfaces
{
    /// <summary>
    /// Defines the contract for building Automation request objects.
    /// </summary>
    public interface IRequestBuilder
    {
        #region Properties

        /// <summary>
        /// Gets the batch identifier for the request.
        /// </summary>
        string BatchId { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Builds a list of request objects from the provided items.
        /// </summary>
        /// <param name="items">The list of request build items.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of request objects.</returns>
        Task<List<object>> BuildRequests(List<IRequestBuildItem> items);

        #endregion
    }
}