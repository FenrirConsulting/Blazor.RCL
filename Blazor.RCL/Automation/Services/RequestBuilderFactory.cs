using Blazor.RCL.Automation.Services.Interfaces;
using Blazor.RCL.NLog.LogService.Interface;
using System;
using System.Collections.Generic;

namespace Blazor.RCL.Automation.Services
{
    /// <summary>
    /// Generic factory for creating request builder instances based on action type.
    /// </summary>
    public class RequestBuilderFactory : IRequestBuilderFactory
    {
        #region Fields

        protected readonly ILogHelper _logger;
        protected readonly Dictionary<string, Func<IRequestBuilder>> _builders;
        
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestBuilderFactory"/> class.
        /// </summary>
        /// <param name="logger">The logging service.</param>
        public RequestBuilderFactory(ILogHelper logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _builders = new Dictionary<string, Func<IRequestBuilder>>(StringComparer.OrdinalIgnoreCase);
        }
        
        #endregion

        #region Public Methods

        /// <summary>
        /// Registers a request builder with the factory.
        /// </summary>
        /// <param name="actionType">The action type to register.</param>
        /// <param name="builderCreator">A function that creates a new instance of the builder.</param>
        public void RegisterBuilder(string actionType, Func<IRequestBuilder> builderCreator)
        {
            if (string.IsNullOrEmpty(actionType))
                throw new ArgumentException("Action type cannot be null or empty.", nameof(actionType));
                
            if (builderCreator == null)
                throw new ArgumentNullException(nameof(builderCreator));

            _builders[actionType.ToLower()] = builderCreator;
        }

        /// <summary>
        /// Gets a request builder for the specified action type.
        /// </summary>
        /// <param name="actionType">The action type.</param>
        /// <returns>An <see cref="IRequestBuilder"/> instance.</returns>
        /// <exception cref="ArgumentException">Thrown when an unsupported action type is provided.</exception>
        public IRequestBuilder GetBuilder(string actionType)
        {
            if (string.IsNullOrEmpty(actionType))
                throw new ArgumentException("Action type cannot be null or empty.", nameof(actionType));

            if (_builders.TryGetValue(actionType.ToLower(), out var builderFunc))
            {
                return builderFunc();
            }

            throw new ArgumentException($"Unsupported action type: {actionType}");
        }

        #endregion
    }
}