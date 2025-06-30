using Blazor.RCL.Automation.Data;
using Blazor.RCL.Automation.AutomationDirectory;
using Blazor.RCL.Automation.AutomationDirectory.AutomationDirectoryRepositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazor.RCL.Automation.AutomationDirectory.AutomationDirectoryRepositories
{
    /// <summary>
    /// Implements the AutomationDirectoryADAcctDispositionAction repository operations.
    /// </summary>
    public class ADAcctDispositionActionRepository : Interfaces.IADAcctDispositionActionRepository
    {
        #region Private Fields

        private readonly AutomationDirectoryDbContext _context;
        private readonly ILogger<ADAcctDispositionActionRepository> _logger;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the AutomationDirectoryADAcctDispositionActionRepository class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger.</param>
        public ADAcctDispositionActionRepository(AutomationDirectoryDbContext context, ILogger<ADAcctDispositionActionRepository> logger)
        {
            _context = context;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #endregion

        #region Public Methods

        /// <inheritdoc/>
        public async Task<IEnumerable<ADAcctDisposition_Action>> GetByBatchIds(IEnumerable<string> batchIds)
        {
            try
            {
                // Use explicit projection to avoid issues with entity-database mismatches
                var query = _context.ADAcctDisposition_Action
                    .Where(a => batchIds.Contains(a.BatchId))
                    .Where(a => a.Disabled && a.Action == "Dispose")
                    .Select(a => new ADAcctDisposition_Action
                    {
                        ActionID = a.ActionID,
                        Action = a.Action,
                        FinalAction = a.FinalAction,
                        ActionType = a.ActionType,
                        ActionValue = a.ActionValue,
                        ActionComment = a.ActionComment,
                        ActionResult = a.ActionResult,
                        ActionResultComment = a.ActionResultComment,
                        UserLastLogon = a.UserLastLogon,
                        DateOfDisable = a.DateOfDisable,
                        SourceDomain = a.SourceDomain,
                        Disabled = a.Disabled,
                        SourceSamaccountname = a.SourceSamaccountname,
                        SourceOriginalOU = a.SourceOriginalOU,
                        ExtensionAttribute8 = a.ExtensionAttribute8,
                        Description = a.Description,
                        SourceManager = a.SourceManager,
                        SourceMemberOf = a.SourceMemberOf,
                        EmployeeNumber = a.EmployeeNumber,
                        mDBUseDefaults = a.mDBUseDefaults,
                        msExchHideFromAddressLists = a.msExchHideFromAddressLists,
                        LastLogonDate = a.LastLogonDate,
                        WhenCreated = a.WhenCreated,
                        PwdLastSet = a.PwdLastSet,
                        BatchId = a.BatchId,
                        BatchRequestFK = a.BatchRequestFK,
                        ToolsResult = a.ToolsResult
                        // Deliberately omitting audit fields that don't exist in the DB
                    });

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                // Log any errors but let the caller handle them
                _logger.LogError(ex, "Error in GetByBatchIds");
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<ADAcctDisposition_Action>> GetBySAMAccount(string samAccount)
        {
            try
            {
                // Use explicit projection to avoid issues with entity-database mismatches
                var query = _context.ADAcctDisposition_Action
                    .Where(a => a.SourceSamaccountname == samAccount)
                    .Where(a => a.Disabled && a.Action == "Dispose" && (a.ToolsResult != "Reinstated" || a.ToolsResult == null))
                    .Select(a => new ADAcctDisposition_Action
                    {
                        ActionID = a.ActionID,
                        Action = a.Action,
                        FinalAction = a.FinalAction,
                        ActionType = a.ActionType,
                        ActionValue = a.ActionValue,
                        ActionComment = a.ActionComment,
                        ActionResult = a.ActionResult,
                        ActionResultComment = a.ActionResultComment,
                        UserLastLogon = a.UserLastLogon,
                        DateOfDisable = a.DateOfDisable,
                        SourceDomain = a.SourceDomain,
                        Disabled = a.Disabled,
                        SourceSamaccountname = a.SourceSamaccountname,
                        SourceOriginalOU = a.SourceOriginalOU,
                        ExtensionAttribute8 = a.ExtensionAttribute8,
                        Description = a.Description,
                        SourceManager = a.SourceManager,
                        SourceMemberOf = a.SourceMemberOf,
                        EmployeeNumber = a.EmployeeNumber,
                        mDBUseDefaults = a.mDBUseDefaults,
                        msExchHideFromAddressLists = a.msExchHideFromAddressLists,
                        LastLogonDate = a.LastLogonDate,
                        WhenCreated = a.WhenCreated,
                        PwdLastSet = a.PwdLastSet,
                        BatchId = a.BatchId,
                        BatchRequestFK = a.BatchRequestFK,
                        ToolsResult = a.ToolsResult
                        // Deliberately omitting audit fields that don't exist in the DB
                    });

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                // Log any errors but let the caller handle them
                _logger.LogError(ex, "Error in GetBySAMAccount");
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<ADAcctDisposition_Action>> GetByEmployeeNumber(string employeeNumber)
        {
            try
            {
                // Use explicit projection to avoid issues with entity-database mismatches
                var query = _context.ADAcctDisposition_Action
                    .Where(a => a.EmployeeNumber == employeeNumber)
                    .Where(a => a.Disabled && a.Action == "Dispose" && (a.ToolsResult != "Reinstated" || a.ToolsResult == null))
                    .Select(a => new ADAcctDisposition_Action
                    {
                        ActionID = a.ActionID,
                        Action = a.Action,
                        FinalAction = a.FinalAction,
                        ActionType = a.ActionType,
                        ActionValue = a.ActionValue,
                        ActionComment = a.ActionComment,
                        ActionResult = a.ActionResult,
                        ActionResultComment = a.ActionResultComment,
                        UserLastLogon = a.UserLastLogon,
                        DateOfDisable = a.DateOfDisable,
                        SourceDomain = a.SourceDomain,
                        Disabled = a.Disabled,
                        SourceSamaccountname = a.SourceSamaccountname,
                        SourceOriginalOU = a.SourceOriginalOU,
                        ExtensionAttribute8 = a.ExtensionAttribute8,
                        Description = a.Description,
                        SourceManager = a.SourceManager,
                        SourceMemberOf = a.SourceMemberOf,
                        EmployeeNumber = a.EmployeeNumber,
                        mDBUseDefaults = a.mDBUseDefaults,
                        msExchHideFromAddressLists = a.msExchHideFromAddressLists,
                        LastLogonDate = a.LastLogonDate,
                        WhenCreated = a.WhenCreated,
                        PwdLastSet = a.PwdLastSet,
                        BatchId = a.BatchId,
                        BatchRequestFK = a.BatchRequestFK,
                        ToolsResult = a.ToolsResult
                        // Deliberately omitting audit fields that don't exist in the DB
                    });

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                // Log any errors but let the caller handle them
                _logger.LogError(ex, "Error in GetByEmployeeNumber");
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<int> MarkRecordsAsReinstatedAsync(string batchId)
        {
            try
            {
                if (string.IsNullOrEmpty(batchId))
                {
                    _logger.LogWarning("Cannot mark records as reinstated: batchId is null or empty");
                    return 0;
                }

                // Sanitize input to avoid SQL injection
                batchId = batchId.Replace("'", "''");

                // First, get the original batch record to get the SAM account name using safe SQL parameters
                var samAccountQuery = "SELECT SourceSamaccountname FROM ADAcctDisposition_Action WHERE BatchId = @batchId";
                var parameter = new SqlParameter("@batchId", batchId);
                
                var samAccountResult = await _context.ADAcctDisposition_Action
                    .FromSqlRaw(samAccountQuery, parameter)
                    .Select(a => a.SourceSamaccountname)
                    .FirstOrDefaultAsync();

                if (string.IsNullOrEmpty(samAccountResult))
                {
                    _logger.LogWarning($"No records found with BatchId {batchId} or SAM account is empty");
                    return 0;
                }

                // Use direct SQL update to avoid issues with audit fields
                string samAccount = samAccountResult.Replace("'", "''"); // Sanitize the samAccount
                
                // Use parameterized query for the update to prevent SQL injection
                string updateSql = "UPDATE ADAcctDisposition_Action SET ToolsResult = @reinstateValue " +
                                  "WHERE SourceSamaccountname = @samAccount AND Disabled = 1 AND Action = 'Dispose'";
                
                var parameters = new[]
                {
                    new SqlParameter("@reinstateValue", "Reinstated"),
                    new SqlParameter("@samAccount", samAccount)
                };
                
                // Execute the update
                int result = await _context.Database.ExecuteSqlRawAsync(updateSql, parameters);
                
                _logger.LogInformation($"Successfully marked {result} dispose records as reinstated for SAM account {samAccount}");
                return result;
            }
            catch (Exception ex)
            {
                // Log any errors but let the caller handle them
                _logger.LogError(ex, $"Error in MarkRecordsAsReinstatedAsync for batchId {batchId}");
                throw;
            }
        }

        #endregion
    }
}
