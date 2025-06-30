using System;
using System.Data;
using System.Threading.Tasks;
using System.Text.Json;
using Blazor.RCL.Automation.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Blazor.RCL.NLog.LogService.Interface;

namespace Blazor.RCL.Automation.Services
{
    /// <summary>
    /// Service for retrieving request response data from the database
    /// </summary>
    public class RequestResponseService
    {
        private readonly IDbContextFactory<AutomationRequestDbContext> _dbContextFactory;
        private readonly ILogHelper _logger;

        public RequestResponseService(IDbContextFactory<AutomationRequestDbContext> dbContextFactory, ILogHelper logger)
        {
            _dbContextFactory = dbContextFactory;
            _logger = logger;
        }

        /// <summary>
        /// Gets the response data for a specific request item by querying the database directly
        /// </summary>
        /// <param name="requestItem">The request item identifier</param>
        /// <returns>JSON formatted response data or null if not found</returns>
        public async Task<string> GetResponseDataFromDatabaseAsync(string requestItem)
        {
            if (string.IsNullOrEmpty(requestItem))
            {
                return null;
            }

            try
            {
                // Create a DbContext instance when needed
                using var dbContext = await _dbContextFactory.CreateDbContextAsync();
                // Create parameters
                var requestItemParam = new SqlParameter("@RequestItemParam", requestItem);
                var requestLogPkParam = new SqlParameter
                {
                    ParameterName = "@RequestLogPK",
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Output
                };
                var requestStatusCodeFkParam = new SqlParameter
                {
                    ParameterName = "@RequestStatusCodeFK",
                    SqlDbType = SqlDbType.Int,
                    Direction = ParameterDirection.Output
                };

                // Execute first query to get RequestLogPK and RequestStatusCodeFK
                await dbContext.Database.ExecuteSqlRawAsync(
                    @"SELECT @RequestLogPK = [RequestLogPK], @RequestStatusCodeFK = [RequestStatusCodeFK]
                      FROM [dbo].[RequestLog]
                      WHERE [RequestItem] = @RequestItemParam",
                    requestItemParam, requestLogPkParam, requestStatusCodeFkParam);

                // Check if we found the request
                if (requestLogPkParam.Value == DBNull.Value)
                {
                    return null;
                }
                
                // Create a new input parameter for the second query using the value from the first query
                int requestLogPkValue = Convert.ToInt32(requestLogPkParam.Value);
                var requestLogPkInputParam = new SqlParameter("@RequestLogPK", requestLogPkValue);
                
                // Use the exact same SQL query format that's known to work when executed manually
                var sqlQuery = @"SELECT 
                      RL.[guid] AS GuId
                      ,ISNULL(RL.[SourceId], '') AS SourceId
                      ,ISNULL(RL.[Request], '') AS Request
                      ,ISNULL(RL.[RequestItem], '') AS RequestItem
                      ,ISNULL(RL.[AccessType], '') AS AccessType
                      ,ISNULL(RL.[AccessSubtype], '') AS AccessSubtype
                      ,RL.RequestStatusCodeFK AS RequestStatusCode
                      ,ISNULL(RSC.RequestStatusDesc, '') AS RequestStatusDesc
                      ,ISNULL(RSC.DisplayStatusText, '') AS StatusComments
                      ,ISNULL((SELECT dbo.[V1.GetRequestTaskStatusJson](@RequestLogPK)), '[]') AS TaskDetails
                      FROM dbo.RequestLog RL
                      INNER JOIN [dbo].[RequestStatusCode] RSC ON RL.RequestStatusCodeFK = RSC.RequestStatusCodePK
                      WHERE RL.[RequestItem] = @RequestItemParam";
                
                // Get the full response data
                var result = await dbContext.Database.SqlQueryRaw<ResponseDataDto>(
                    sqlQuery,
                    requestItemParam, requestLogPkInputParam).FirstOrDefaultAsync();

                if (result == null)
                    return null;

                if (result == null)
                    return null;
                
                // Try to parse TaskDetails, with fallback handling
                object taskDetailsObj = null;
                try
                {
                    if (!string.IsNullOrEmpty(result.TaskDetails))
                    {
                        // Configure JSON options for better parsing
                        var options = new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true,
                            AllowTrailingCommas = true,
                            ReadCommentHandling = JsonCommentHandling.Skip
                        };
                        
                        // Try to parse as JSON array (which is what TaskDetails should be)
                        taskDetailsObj = JsonSerializer.Deserialize<JsonElement>(result.TaskDetails, options);
                    }
                }
                catch (JsonException jsonEx)
                {
                    _logger.LogError("Error parsing TaskDetails JSON", jsonEx);

                    // If parsing fails, set to an empty array so the UI doesn't break
                    taskDetailsObj = JsonSerializer.Deserialize<JsonElement>("[]");
                }
                
                // Convert to standard JSON format matching the expected structure
                var responseData = new
                {
                    result.GuId,
                    result.SourceId,
                    result.Request,
                    result.RequestItem,
                    result.AccessType,
                    result.AccessSubtype,
                    result.RequestStatusCode,
                    result.RequestStatusDesc,
                    StatusComments = result.StatusComments,
                    TaskDetails = taskDetailsObj,
                    ErrorDetails = (object)null,
                    AutomationId = result.GuId.ToString()
                };

                var jsonResult = JsonSerializer.Serialize(responseData, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
                
                return jsonResult;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving response data for RequestItem: {requestItem}", ex);
                return null;
            }
        }

        /// <summary>
        /// Data transfer object for mapping SQL query results
        /// </summary>
        private class ResponseDataDto
        {
            public Guid GuId { get; set; }
            public string SourceId { get; set; } = string.Empty;
            public string Request { get; set; } = string.Empty;
            public string RequestItem { get; set; } = string.Empty;
            public string AccessType { get; set; } = string.Empty;
            public string AccessSubtype { get; set; } = string.Empty;
            public short RequestStatusCode { get; set; } // Changed from int to short (Int16) to match database type
            public string RequestStatusDesc { get; set; } = string.Empty;
            public string StatusComments { get; set; } = string.Empty;
            public string TaskDetails { get; set; } = string.Empty;
        }
    }
}
