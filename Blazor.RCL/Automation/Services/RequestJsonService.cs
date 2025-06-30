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
    /// Service for retrieving request JSON data from the database
    /// </summary>
    public class RequestJsonService
    {
        private readonly IDbContextFactory<AutomationRequestDbContext> _dbContextFactory;
        private readonly ILogHelper _logger;

        public RequestJsonService(IDbContextFactory<AutomationRequestDbContext> dbContextFactory, ILogHelper logger)
        {
            _dbContextFactory = dbContextFactory;
            _logger = logger;
        }

        /// <summary>
        /// Gets the request JSON data for a specific request item by querying the database directly
        /// </summary>
        /// <param name="requestItem">The request item identifier</param>
        /// <returns>JSON formatted request data or null if not found</returns>
        public async Task<string> GetRequestJsonFromDatabaseAsync(string requestItem)
        {
            if (string.IsNullOrEmpty(requestItem))
            {
                return null;
            }

            try
            {
                // Create a DbContext instance when needed
                using var dbContext = await _dbContextFactory.CreateDbContextAsync();
                // Parameter for the RequestItem
                var requestItemParam = new SqlParameter("@RequestItemParam", requestItem);
                
                _logger.LogMessage($"Executing query to get request JSON for {requestItem}");
                
                // Execute direct query to get the RequestJson using joins to ensure we get the right data
                var sqlQuery = @"
                    SELECT RJ.RequestJson
                    FROM [dbo].[RequestLog] RL
                    JOIN [dbo].[RequestJson] RJ ON RL.RequestLogPK = RJ.RequestLogFK
                    WHERE RL.RequestItem = @RequestItemParam";
                
                // Use direct ADO.NET approach for more control
                using var command = dbContext.Database.GetDbConnection().CreateCommand();
                command.CommandText = sqlQuery;
                command.Parameters.Add(new SqlParameter("@RequestItemParam", requestItem));
                
                // Ensure connection is open
                if (dbContext.Database.GetDbConnection().State != ConnectionState.Open)
                    await dbContext.Database.GetDbConnection().OpenAsync();
                
                // Execute the command and get the JSON data
                using var reader = await command.ExecuteReaderAsync();
                string requestJsonValue = null;
                
                // If we have results
                if (await reader.ReadAsync())
                {
                    // Get the RequestJson value if it's not null
                    if (!reader.IsDBNull(0))
                    {
                        requestJsonValue = reader.GetString(0);
                        _logger.LogMessage($"Found request JSON data with length: {requestJsonValue.Length}");
                    }
                    else
                    {
                        _logger.LogMessage("Request JSON value is NULL in database");
                    }
                }
                else
                {
                    _logger.LogMessage($"No request JSON found for RequestItem: {requestItem}");
                }
                
                // Return null if no results
                if (string.IsNullOrEmpty(requestJsonValue))
                {
                    _logger.LogMessage("Returning null because requestJsonValue is null or empty");
                    return null;
                }

                // Format JSON for readability if needed
                try
                {
                    var jsonElement = JsonSerializer.Deserialize<JsonElement>(requestJsonValue);
                    return JsonSerializer.Serialize(jsonElement, new JsonSerializerOptions
                    {
                        WriteIndented = true
                    });
                }
                catch (JsonException ex)
                {
                    _logger.LogError("Error parsing RequestJson", ex);
                    // Return original JSON if parsing fails
                    return requestJsonValue;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving request data for RequestItem: {requestItem}", ex);
                return null;
            }
        }

        /// <summary>
        /// Data transfer object for mapping SQL query results
        /// </summary>
        private class RequestJsonDto
        {
            public string RequestJson { get; set; } = string.Empty;
        }
    }
}
