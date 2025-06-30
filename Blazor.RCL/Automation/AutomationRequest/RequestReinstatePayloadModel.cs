using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Blazor.RCL.Automation.AutomationRequest.Interfaces;

namespace Blazor.RCL.Automation.AutomationRequest
{
    /// <summary>
    /// Represents the payload model for a reinstate request.
    /// </summary>
    public class RequestReinstatePayloadModel : IRequestPayloadModel
    {
        #region Properties

        /// <summary>
        /// Gets or sets the employee identifier.
        /// </summary>
        [JsonPropertyName("EmployeeId")]
        public string EmployeeId { get; set; } = "";

        /// <summary>
        /// Gets or sets the list of accounts to enable.
        /// </summary>
        [JsonPropertyName("EnableAccounts")]
        public List<EnableAccount> EnableAccounts { get; set; } = new List<EnableAccount>();

        /// <summary>
        /// Gets or sets the list of attributes to set.
        /// </summary>
        [JsonPropertyName("SetAttributes")]
        public List<SetAttribute> SetAttributes { get; set; } = new List<SetAttribute>();

        /// <summary>
        /// Gets or sets the list of accounts to be unhidden from address lists.
        /// </summary>
        [JsonPropertyName("HiddenFromAddressListsEnabled")]
        public List<HiddenFromAddressListDisabled> HiddenFromAddressListsEnabled { get; set; } = new List<HiddenFromAddressListDisabled>();

        /// <summary>
        /// Gets or sets the list of members to add to groups.
        /// </summary>
        [JsonPropertyName("AddMembers")]
        public List<AddMember> AddMembers { get; set; } = new List<AddMember>();

        /// <summary>
        /// Gets or sets the list of accounts to move.
        /// </summary>
        [JsonPropertyName("MoveAccounts")]
        public List<MoveAccount> MoveAccounts { get; set; } = new List<MoveAccount>();

        #endregion

        #region Nested Classes

        /// <summary>
        /// Represents an account to be enabled.
        /// </summary>
        public class EnableAccount
        {
            /// <summary>
            /// Gets or sets the account name.
            /// </summary>
            [JsonPropertyName("AccountName")]
            public string AccountName { get; set; } = "";

            /// <summary>
            /// Gets or sets the account domain.
            /// </summary>
            [JsonPropertyName("AccountDomain")]
            public string AccountDomain { get; set; } = "";
        }

        /// <summary>
        /// Represents an attribute to be set.
        /// </summary>
        public class SetAttribute : EnableAccount
        {
            /// <summary>
            /// Gets or sets the attribute name.
            /// </summary>
            [JsonPropertyName("AttributeName")]
            public string AttributeName { get; set; } = "";

            /// <summary>
            /// Gets or sets the attribute value.
            /// </summary>
            [JsonPropertyName("AttributeValue")]
            public string AttributeValue { get; set; } = "";

            /// <summary>
            /// Initializes a new instance of the <see cref="SetAttribute"/> class.
            /// </summary>
            public SetAttribute()
            {
                if (AttributeName == "Description")
                {
                    AttributeValue = $"{DateTime.Now:yyyy-MM-dd} - Reinstated - previous value from ADAcctDisposition_Actions table";
                }
                else if (AttributeName == "msExchHideFromAddressList")
                {
                    AttributeValue = "FALSE";
                }
                else
                {
                    AttributeValue = "previous value from ADAcctDisposition_Actions table";
                }
            }
        }

        /// <summary>
        /// Represents an account to be unhidden from address lists.
        /// </summary>
        public class HiddenFromAddressListDisabled : EnableAccount { }

        /// <summary>
        /// Represents a member to be added to a group.
        /// </summary>
        public class AddMember
        {
            /// <summary>
            /// Gets or sets the group name.
            /// </summary>
            [JsonPropertyName("GroupName")]
            public string GroupName { get; set; } = "";

            /// <summary>
            /// Gets or sets the group domain.
            /// </summary>
            [JsonPropertyName("GroupDomain")]
            public string GroupDomain { get; set; } = "";

            /// <summary>
            /// Gets or sets the account name.
            /// </summary>
            [JsonPropertyName("AccountName")]
            public string AccountName { get; set; } = "";

            /// <summary>
            /// Gets or sets the account domain.
            /// </summary>
            [JsonPropertyName("AccountDomain")]
            public string AccountDomain { get; set; } = "";
        }

        /// <summary>
        /// Represents an account to be moved.
        /// </summary>
        public class MoveAccount : EnableAccount
        {
            /// <summary>
            /// Gets or sets the target OU.
            /// </summary>
            [JsonPropertyName("TargetOU")]
            public string TargetOU { get; set; } = "previous value from ADAcctDisposition_Actions table";
        }

        #endregion
    }
}