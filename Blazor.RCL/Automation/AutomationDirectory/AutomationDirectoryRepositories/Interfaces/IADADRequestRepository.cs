using Blazor.RCL.Automation.AutomationDirectory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazor.RCL.Automation.AutomationDirectory.AutomationDirectoryRepositories.Interfaces
{
    /// <summary>
    /// Defines the interface for ADADRequest repository operations.
    /// </summary>
    public interface IADADRequestRepository
    {
        /// <summary>
        /// Gets the ADADRequest entries by SAM account.
        /// </summary>
        /// <param name="samAccount">The SAM account to search for.</param>
        /// <returns>A collection of ADADRequest entries.</returns>
        Task<IEnumerable<ADADRequest>> GetBySAMAccount(string samAccount);

        /// <summary>
        /// Gets the ADADRequest entries by employee number (Company Resource ID).
        /// </summary>
        /// <param name="CompanyResourceId">The Company Resource ID to search for.</param>
        /// <returns>A collection of ADADRequest entries.</returns>
        Task<IEnumerable<ADADRequest>> GetByCompanyResourceId(string CompanyResourceId);

        /// <summary>
        /// Gets the ADADRequest entries by source ID.
        /// </summary>
        /// <param name="sourceId">The source ID to search for.</param>
        /// <returns>A collection of ADADRequest entries.</returns>
        Task<IEnumerable<ADADRequest>> GetBySourceId(string sourceId);

        /// <summary>
        /// Updates the Automation status and message for a request.
        /// </summary>
        /// <param name="requestId">The request ID to update.</param>
        /// <param name="status">The new status value.</param>
        /// <param name="statusMessage">The new status message.</param>
        /// <returns>The number of records updated.</returns>
        Task<int> UpdateAutomationStatusAsync(long requestId, string status, string statusMessage);

        /// <summary>
        /// Marks records as reinstated in the database.
        /// </summary>
        /// <param name="batchId">The batch ID of the original operation.</param>
        /// <returns>The number of records updated.</returns>
        Task<int> MarkRecordsAsReinstatedAsync(string batchId);
    }
}
