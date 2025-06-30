using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Blazor.RCL.Automation.AutomationTasks
{
    /// <summary>
    /// Represents the details of an Active Directory user.
    /// </summary>
    public class ADUserDetails
    {
        #region Request Properties
        /// <summary>
        /// Gets or sets the value indicating whether the request was successful.
        /// </summary>
        public bool SuccessCode { get; set; }

        /// <summary>
        /// Gets or sets the employee ID.
        /// </summary>
        public string EmployeeId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the employee ID.
        /// </summary>
        public string AccountName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the account domain.
        /// </summary>
        public string AccountDomain { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether to include member details.
        /// </summary>
        public bool MemberOfDetails { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to include manager details.
        /// </summary>
        public bool ManagerDetails { get; set; }

        /// <summary>
        /// Gets or sets the source ID from the original request (for tracking reinstated accounts).
        /// </summary>
        public string SourceId { get; set; } = string.Empty;
        #endregion

        #region Response Properties
        /// <summary>
        /// Gets or sets the task ID.
        /// </summary>
        [JsonPropertyName("TaskID")]
        public int TaskId { get; set; }

        /// <summary>
        /// Gets or sets the task status.
        /// </summary>
        [JsonPropertyName("TaskStatus")]
        public int TaskStatus { get; set; }

        /// <summary>
        /// Gets or sets the error code.
        /// </summary>
        [JsonPropertyName("ErrorCode")]
        public int ErrorCode { get; set; }

        /// <summary>
        /// Gets or sets the error description.
        /// </summary>
        [JsonPropertyName("ErrorDesc")]
        public string ErrorDesc { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the error trace.
        /// </summary>
        [JsonPropertyName("ErrorTrace")]
        public string ErrorTrace { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the success description.
        /// </summary>
        [JsonPropertyName("SuccessDesc")]
        public string SuccessDesc { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the response object.
        /// </summary>
        [JsonPropertyName("RObject")]
        public ResponseObject? Response { get; set; } = new ResponseObject();
        #endregion

    }

    #region Nested Classes
    /// <summary>
    /// Represents the response object containing user and manager details.
    /// </summary>
    public class ResponseObject
    {
        [JsonPropertyName("Users")]
        public List<UserDetails>? Users { get; set; }

        [JsonPropertyName("Manager")]
        public UserDetails? Manager { get; set; }
    }

    /// <summary>
    /// Represents the details of a user or manager.
    /// </summary>
    public class UserDetails
    {
        [JsonPropertyName("Domain")]
        public string Domain { get; set; } = string.Empty;

        [JsonPropertyName("Name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("DisplayName")]
        public string DisplayName { get; set; } = string.Empty;

        [JsonPropertyName("SamAccountName")]
        public string SamAccountName { get; set; } = string.Empty;

        [JsonPropertyName("UserPrincipalName")]
        public string UserPrincipalName { get; set; } = string.Empty;

        [JsonPropertyName("EmployeeNumber")]
        public string EmployeeNumber { get; set; } = string.Empty;

        [JsonPropertyName("EmployeeId")]
        public string EmployeeId { get; set; } = string.Empty;

        [JsonPropertyName("UserAccountControl")]
        public string UserAccountControl { get; set; } = string.Empty;

        [JsonPropertyName("MSExchremoteRecipientType")]
        public string MSExchangeRemoteRecipientType { get; set; } = string.Empty;

        [JsonPropertyName("Mail")]
        public string Mail { get; set; } = string.Empty;

        [JsonPropertyName("SN")]
        public string SN { get; set; } = string.Empty;

        [JsonPropertyName("GivenName")]
        public string GivenName { get; set; } = string.Empty;

        [JsonPropertyName("Title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("DistinguishedName")]
        public string DistinguishedName { get; set; } = string.Empty;

        [JsonPropertyName("ManagerDN")]
        public string ManagerDN { get; set; } = string.Empty;

        [JsonPropertyName("Description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("Comment ")]
        public string? Comment { get; set; }

        [JsonPropertyName("AccountExpires")]
        public string? AccountExpires { get; set; }

        [JsonPropertyName("PwdLastSet")]
        public string PwdLastSet { get; set; } = string.Empty;

        [JsonPropertyName("CreateTimestampValue")]
        public string CreateTimestampValue { get; set; } = string.Empty;

        [JsonPropertyName("LastLogontimestamp")]
        public string LastLogonTimestamp { get; set; } = string.Empty;

        [JsonPropertyName("LockOutTime")]
        public string LockOutTime { get; set; } = string.Empty;

        [JsonPropertyName("MSExchRecipientTypeDetails")]
        public string MSExchRecipientTypeDetails { get; set; } = string.Empty;

        [JsonPropertyName("HomeMdb")]
        public string HomeMdb { get; set; } = string.Empty;

        [JsonPropertyName("AdminDescription ")]
        public string AdminDescription { get; set; } = string.Empty;

        [JsonPropertyName("MSDSConsistencyGuid")]
        public string MSDSConsistencyGuid { get; set; } = string.Empty;

        [JsonPropertyName("ExtensionAttribute3")]
        public string ExtensionAttribute3 { get; set; } = string.Empty;

        [JsonPropertyName("ExtensionAttribute4")]
        public string ExtensionAttribute4 { get; set; } = string.Empty;

        [JsonPropertyName("ExtensionAttribute8")]
        public string ExtensionAttribute8 { get; set; } = string.Empty;

        [JsonPropertyName("ExtensionAttribute12")]
        public string ExtensionAttribute12 { get; set; } = string.Empty;

        [JsonPropertyName("MemberOf")]
        public List<string> MemberOf { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the previous request item identifier for tracking reinstated accounts.
        /// This field is used to establish a link between disable and reinstate operations.
        /// </summary>
        [JsonPropertyName("PreviousRequestItem")]
        public string PreviousRequestItem { get; set; } = string.Empty;
    }
    #endregion
}