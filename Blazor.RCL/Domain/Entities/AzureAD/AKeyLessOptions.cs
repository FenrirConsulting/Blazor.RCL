using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazor.RCL.Domain.Entities.AzureAD
{
    /// <summary>
    /// Represents the AKeyLess configuration options.
    /// </summary>
    public class AKeyLessOptions
    {
        #region Properties

        /// <summary>
        /// Gets or sets the Access ID for AKeyLess authentication.
        /// </summary>
        public string? AccessID { get; set; }

        /// <summary>
        /// Gets or sets the Secret path for production environment.
        /// </summary>
        public string? SecPathProd { get; set; }

        /// <summary>
        /// Gets or sets the base URL for non-production environments.
        /// </summary>
        public string? BaseUrlNonProd { get; set; }

        /// <summary>
        /// Gets or sets the base URL for production environment.
        /// </summary>
        public string? BaseUrlProd { get; set; }

        /// <summary>
        /// Gets or sets the timeout value in seconds.
        /// </summary>
        public int Timeout { get; set; } = 30;

        /// <summary>
        /// Gets or sets the number of retry attempts.
        /// </summary>
        public int RetryCount { get; set; } = 2;

        /// <summary>
        /// Gets or sets the delay between retry attempts in milliseconds.
        /// </summary>
        public int RetryDelay { get; set; } = 1000;

        /// <summary>
        /// Gets or sets the User ID for AKeyLess authentication.
        /// Only used in development environments.
        /// </summary>
        public string? UiD { get; set; }

        /// <summary>
        /// Gets or sets the environment type (Prod or NonProd).
        /// </summary>
        public string? env { get; set; }

        /// <summary>
        /// Gets or sets the collection of AKeyLess secrets.
        /// </summary>
        public AKeyLessSecrets? Secrets { get; set; }

        #endregion
    }

    /// <summary>
    /// Represents the AKeyLess secrets configuration.
    /// </summary>
    public class AKeyLessSecrets
    {
        /// <summary>
        /// Gets or sets the Access ID for non-production environments.
        /// </summary>
        public string? NonProdAccessID { get; set; }

        /// <summary>
        /// Gets or sets the User ID for non-production environments.
        /// </summary>
        public string? NonProdUiD { get; set; }

        /// <summary>
        /// Gets or sets the Access ID for production environments.
        /// </summary>
        public string? ProdAccessID { get; set; }

        /// <summary>
        /// Gets or sets the User ID for production environments.
        /// </summary>
        public string? ProdUiD { get; set; }

        /// <summary>
        /// Gets or sets the mapping of configuration keys to secret paths.
        /// </summary>
        public Dictionary<string, string>? SecretMappings { get; set; }
    }
}
