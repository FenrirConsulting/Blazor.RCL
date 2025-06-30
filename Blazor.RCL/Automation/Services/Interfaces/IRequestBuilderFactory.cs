using System;

namespace Blazor.RCL.Automation.Services.Interfaces
{
    /// <summary>
    /// Defines the factory for creating request builder instances.
    /// </summary>
    public interface IRequestBuilderFactory
    {
        #region Methods

        /// <summary>
        /// Registers a request builder with the factory.
        /// </summary>
        /// <param name="actionType">The action type to register.</param>
        /// <param name="builderCreator">A function that creates a new instance of the builder.</param>
        void RegisterBuilder(string actionType, Func<IRequestBuilder> builderCreator);

        /// <summary>
        /// Gets a request builder for the specified action type.
        /// </summary>
        /// <param name="actionType">The action type (e.g., "enable", "disable").</param>
        /// <returns>An <see cref="IRequestBuilder"/> instance.</returns>
        IRequestBuilder GetBuilder(string actionType);

        #endregion
    }
}