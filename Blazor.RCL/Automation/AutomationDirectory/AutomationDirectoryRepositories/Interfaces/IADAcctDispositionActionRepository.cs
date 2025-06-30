using Blazor.RCL.Automation.AutomationDirectory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazor.RCL.Automation.AutomationDirectory.AutomationDirectoryRepositories.Interfaces
{
    /// <summary>
    /// Defines the interface for AutomationDirectoryADAcctDispositionAction repository operations.
    /// </summary>
    public interface IADAcctDispositionActionRepository
    {
        /// <summary>
        /// Gets the AutomationDirectoryADAcctDisposition_Action entries by batch IDs.
        /// </summary>
        /// <param name="batchIds">The batch IDs to search for.</param>
        /// <returns>A collection of AutomationDirectoryADAcctDisposition_Action entries.</returns>
        Task<IEnumerable<ADAcctDisposition_Action>> GetByBatchIds(IEnumerable<string> batchIds);

        /// <summary>
        /// Gets the AutomationDirectoryADAcctDisposition_Action entries by SAM account.
        /// </summary>
        /// <param name="samAccount">The SAM account to search for.</param>
        /// <returns>A collection of AutomationDirectoryADAcctDisposition_Action entries.</returns>
        Task<IEnumerable<ADAcctDisposition_Action>> GetBySAMAccount(string samAccount);

        /// <summary>
        /// Gets the AutomationDirectoryADAcctDisposition_Action entries by employee number.
        /// </summary>
        /// <param name="employeeNumber">The employee number to search for.</param>
        /// <returns>A collection of AutomationDirectoryADAcctDisposition_Action entries.</returns>
        Task<IEnumerable<ADAcctDisposition_Action>> GetByEmployeeNumber(string employeeNumber);

        /// <summary>
        /// Marks dispose records as reinstated in the external database.
        /// </summary>
        /// <param name="batchId">The batch ID of the original dispose operation.</param>
        /// <returns>The number of records updated.</returns>
        Task<int> MarkRecordsAsReinstatedAsync(string batchId);
    }
}
