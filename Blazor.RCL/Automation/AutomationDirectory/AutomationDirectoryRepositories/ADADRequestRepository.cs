using Blazor.RCL.Automation.Data;
using Blazor.RCL.Automation.AutomationDirectory;
using Blazor.RCL.Automation.AutomationDirectory.AutomationDirectoryRepositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blazor.RCL.Automation.AutomationDirectory.AutomationDirectoryRepositories
{
    /// <summary>
    /// Implements the ADADRequest repository operations.
    /// </summary>
    public class ADADRequestRepository : Interfaces.IADADRequestRepository
    {
        #region Private Fields

        private readonly AutomationDirectoryDbContext _context;
        private readonly ILogger<ADADRequestRepository> _logger;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the ADADRequestRepository class.
        /// </summary>
        /// <param name="context">The database context.</param>
        /// <param name="logger">The logger.</param>
        public ADADRequestRepository(AutomationDirectoryDbContext context, ILogger<ADADRequestRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        #endregion

        #region Public Methods

        /// <inheritdoc/>
        public async Task<IEnumerable<ADADRequest>> GetBySAMAccount(string samAccount)
        {
            try
            {
                if (string.IsNullOrEmpty(samAccount))
                {
                    _logger.LogWarning("Cannot get ADADRequest by SAM account: samAccount is null or empty");
                    return Enumerable.Empty<ADADRequest>();
                }

                // Use explicit projection to avoid issues with entity-database mismatches
                var query = _context.ADADRequest
                    .Where(a => a.SamAccountName == samAccount)
                    .Select(a => new ADADRequest
                    {
                        ADADRequestPK = a.ADADRequestPK,
                        Domain = a.Domain,
                        CompanyResourceId = a.CompanyResourceId,
                        HRStatus = a.HRStatus,
                        SamAccountName = a.SamAccountName,
                        UPN = a.UPN,
                        UserAccountControl = a.UserAccountControl,
                        DistinguishedName = a.DistinguishedName,
                        SN = a.SN,
                        GivenName = a.GivenName,
                        DisplayName = a.DisplayName,
                        ADDescription = a.ADDescription,
                        EA3 = a.EA3,
                        EA4 = a.EA4,
                        EA8 = a.EA8,
                        MemberOf = a.MemberOf,
                        Mail = a.Mail,
                        Manager = a.Manager,
                        ManagerMail = a.ManagerMail,
                        MdbuseDefaults = a.MdbuseDefaults,
                        MSExchangeRemoteRecipientType = a.MSExchangeRemoteRecipientType,
                        MSExchRecipientTypeDetails = a.MSExchRecipientTypeDetails,
                        MsExchHideFromAddressLists = a.MsExchHideFromAddressLists,
                        FacsimileTelephoneNumber = a.FacsimileTelephoneNumber,
                        UserEnabled = a.UserEnabled,
                        PwdLastSetDate = a.PwdLastSetDate,
                        PwdLastSetDays = a.PwdLastSetDays,
                        LastLogonDate = a.LastLogonDate,
                        LastLogonDays = a.LastLogonDays,
                        ADWhenCreated = a.ADWhenCreated,
                        Action = a.Action,
                        ActionComment = a.ActionComment,
                        ActionDate = a.ActionDate,
                        AuditUpdateUserName = a.AuditUpdateUserName,
                        AuditUpdateDate = a.AuditUpdateDate,
                        ToolsResult = a.ToolsResult,
                        SourceId = a.SourceId,
                        AutomationGuid = a.AutomationGuid,
                        AutomationStatus = a.AutomationStatus,
                        AutomationStatusMessage = a.AutomationStatusMessage
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
        public async Task<IEnumerable<ADADRequest>> GetByCompanyResourceId(string CompanyResourceId)
        {
            try
            {
                if (string.IsNullOrEmpty(CompanyResourceId))
                {
                    _logger.LogWarning("Cannot get ADADRequest by Company Resource ID: CompanyResourceId is null or empty");
                    return Enumerable.Empty<ADADRequest>();
                }

                // Use explicit projection to avoid issues with entity-database mismatches
                var query = _context.ADADRequest
                    .Where(a => a.CompanyResourceId == CompanyResourceId)
                    .Select(a => new ADADRequest
                    {
                        ADADRequestPK = a.ADADRequestPK,
                        Domain = a.Domain,
                        CompanyResourceId = a.CompanyResourceId,
                        HRStatus = a.HRStatus,
                        SamAccountName = a.SamAccountName,
                        UPN = a.UPN,
                        UserAccountControl = a.UserAccountControl,
                        DistinguishedName = a.DistinguishedName,
                        SN = a.SN,
                        GivenName = a.GivenName,
                        DisplayName = a.DisplayName,
                        ADDescription = a.ADDescription,
                        EA3 = a.EA3,
                        EA4 = a.EA4,
                        EA8 = a.EA8,
                        MemberOf = a.MemberOf,
                        Mail = a.Mail,
                        Manager = a.Manager,
                        ManagerMail = a.ManagerMail,
                        MdbuseDefaults = a.MdbuseDefaults,
                        MSExchangeRemoteRecipientType = a.MSExchangeRemoteRecipientType,
                        MSExchRecipientTypeDetails = a.MSExchRecipientTypeDetails,
                        MsExchHideFromAddressLists = a.MsExchHideFromAddressLists,
                        FacsimileTelephoneNumber = a.FacsimileTelephoneNumber,
                        UserEnabled = a.UserEnabled,
                        PwdLastSetDate = a.PwdLastSetDate,
                        PwdLastSetDays = a.PwdLastSetDays,
                        LastLogonDate = a.LastLogonDate,
                        LastLogonDays = a.LastLogonDays,
                        ADWhenCreated = a.ADWhenCreated,
                        Action = a.Action,
                        ActionComment = a.ActionComment,
                        ActionDate = a.ActionDate,
                        AuditUpdateUserName = a.AuditUpdateUserName,
                        AuditUpdateDate = a.AuditUpdateDate,
                        ToolsResult = a.ToolsResult,
                        SourceId = a.SourceId,
                        AutomationGuid = a.AutomationGuid,
                        AutomationStatus = a.AutomationStatus,
                        AutomationStatusMessage = a.AutomationStatusMessage
                    });

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                // Log any errors but let the caller handle them
                _logger.LogError(ex, "Error in GetByCompanyResourceId");
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<ADADRequest>> GetBySourceId(string sourceId)
        {
            try
            {
                if (string.IsNullOrEmpty(sourceId))
                {
                    _logger.LogWarning("Cannot get ADADRequest by Source ID: sourceId is null or empty");
                    return Enumerable.Empty<ADADRequest>();
                }

                // Use explicit projection to avoid issues with entity-database mismatches
                var query = _context.ADADRequest
                    .Where(a => a.SourceId == sourceId)
                    .Select(a => new ADADRequest
                    {
                        ADADRequestPK = a.ADADRequestPK,
                        Domain = a.Domain,
                        CompanyResourceId = a.CompanyResourceId,
                        HRStatus = a.HRStatus,
                        SamAccountName = a.SamAccountName,
                        UPN = a.UPN,
                        UserAccountControl = a.UserAccountControl,
                        DistinguishedName = a.DistinguishedName,
                        SN = a.SN,
                        GivenName = a.GivenName,
                        DisplayName = a.DisplayName,
                        ADDescription = a.ADDescription,
                        EA3 = a.EA3,
                        EA4 = a.EA4,
                        EA8 = a.EA8,
                        MemberOf = a.MemberOf,
                        Mail = a.Mail,
                        Manager = a.Manager,
                        ManagerMail = a.ManagerMail,
                        MdbuseDefaults = a.MdbuseDefaults,
                        MSExchangeRemoteRecipientType = a.MSExchangeRemoteRecipientType,
                        MSExchRecipientTypeDetails = a.MSExchRecipientTypeDetails,
                        MsExchHideFromAddressLists = a.MsExchHideFromAddressLists,
                        FacsimileTelephoneNumber = a.FacsimileTelephoneNumber,
                        UserEnabled = a.UserEnabled,
                        PwdLastSetDate = a.PwdLastSetDate,
                        PwdLastSetDays = a.PwdLastSetDays,
                        LastLogonDate = a.LastLogonDate,
                        LastLogonDays = a.LastLogonDays,
                        ADWhenCreated = a.ADWhenCreated,
                        Action = a.Action,
                        ActionComment = a.ActionComment,
                        ActionDate = a.ActionDate,
                        AuditUpdateUserName = a.AuditUpdateUserName,
                        AuditUpdateDate = a.AuditUpdateDate,
                        ToolsResult = a.ToolsResult,
                        SourceId = a.SourceId,
                        AutomationGuid = a.AutomationGuid,
                        AutomationStatus = a.AutomationStatus,
                        AutomationStatusMessage = a.AutomationStatusMessage
                    });

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                // Log any errors but let the caller handle them
                _logger.LogError(ex, "Error in GetBySourceId");
                throw;
            }
        }

        /// <inheritdoc/>
        public async Task<int> UpdateAutomationStatusAsync(long requestId, string status, string statusMessage)
        {
            try
            {
                if (requestId <= 0)
                {
                    _logger.LogWarning("Cannot update Automation status: requestId is invalid");
                    return 0;
                }

                if (string.IsNullOrEmpty(status))
                {
                    _logger.LogWarning("Cannot update Automation status: status is null or empty");
                    return 0;
                }

                // Find the request
                var request = await _context.ADADRequest.FindAsync(requestId);
                if (request == null)
                {
                    _logger.LogWarning($"No request found with ID {requestId}");
                    return 0;
                }

                // Update the status fields
                request.AutomationStatus = status;
                request.AutomationStatusMessage = statusMessage;
                request.AuditUpdateDate = DateTime.UtcNow;

                // Save changes
                return await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log any errors but let the caller handle them
                _logger.LogError(ex, $"Error in UpdateAutomationStatusAsync for requestId {requestId}");
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

                // First, get the original record to get the SAM account name using safe SQL parameters
                var samAccountQuery = "SELECT SamAccountName FROM ADADRequest WHERE SourceId = @batchId";
                var parameter = new Microsoft.Data.SqlClient.SqlParameter("@batchId", batchId);
                
                var samAccountResult = await _context.ADADRequest
                    .FromSqlRaw(samAccountQuery, parameter)
                    .Select(a => a.SamAccountName)
                    .FirstOrDefaultAsync();

                if (string.IsNullOrEmpty(samAccountResult))
                {
                    _logger.LogWarning($"No records found with SourceId {batchId} or SAM account is empty");
                    return 0;
                }

                // Use direct SQL update to avoid issues with audit fields
                string samAccount = samAccountResult.Replace("'", "''"); // Sanitize the samAccount
                
                // Use parameterized query for the update to prevent SQL injection
                string updateSql = "UPDATE ADADRequest SET ToolsResult = @reinstateValue " +
                                  "WHERE SamAccountName = @samAccount AND Action = @action " +
                                  "AND (ToolsResult IS NULL OR ToolsResult != @reinstateValue)";
                
                var parameters = new[]
                {
                    new Microsoft.Data.SqlClient.SqlParameter("@reinstateValue", "Reinstated"),
                    new Microsoft.Data.SqlClient.SqlParameter("@samAccount", samAccount),
                    new Microsoft.Data.SqlClient.SqlParameter("@action", "Dispose-ADADDSPS")
                };
                
                // Execute the update
                int result = await _context.Database.ExecuteSqlRawAsync(updateSql, parameters);
                
                _logger.LogInformation($"Successfully marked {result} records as reinstated for SAM account {samAccount}");
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
