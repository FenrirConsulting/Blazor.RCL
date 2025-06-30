using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Blazor.RCL.Automation.AutomationRequest.Interfaces;

namespace Blazor.RCL.Automation.AutomationRequest
{
    /// <summary>
    /// Represents the payload model for a dispose request.
    /// </summary>
    public class RequestDisposePayloadModel : IRequestPayloadModel
    {
        #region Properties

        /// <summary>
        /// Gets or sets the employee identifier.
        /// </summary>
        [JsonPropertyName("EmployeeId")]
        public string EmployeeId { get; set; } = "";

        /// <summary>
        /// Gets or sets the list of accounts to disable.
        /// </summary>
        [JsonPropertyName("DisableAccounts")]
        public List<DisableAccount> DisableAccounts { get; set; } = new List<DisableAccount>();

        /// <summary>
        /// Gets or sets the list of attributes to set.
        /// </summary>
        [JsonPropertyName("SetAttributes")]
        public List<SetAttribute> SetAttributes { get; set; } = new List<SetAttribute>();

        /// <summary>
        /// Gets or sets the list of accounts to be hidden from address lists.
        /// </summary>
        [JsonPropertyName("HiddenFromAddressListsEnabled")]
        public List<HiddenFromAddressListEnabled> HiddenFromAddressListsEnabled { get; set; } = new List<HiddenFromAddressListEnabled>();

        /// <summary>
        /// Gets or sets the list of members to remove from groups.
        /// </summary>
        [JsonPropertyName("RemoveMembers")]
        public List<RemoveMember> RemoveMembers { get; set; } = new List<RemoveMember>();

        /// <summary>
        /// Gets or sets the list of accounts to move.
        /// </summary>
        [JsonPropertyName("MoveAccounts")]
        public List<MoveAccount> MoveAccounts { get; set; } = new List<MoveAccount>();

        #endregion

        #region Nested Classes

        /// <summary>
        /// Represents an account to be disabled.
        /// </summary>
        public class DisableAccount
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
        public class SetAttribute : DisableAccount
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
                    AttributeValue = $"{DateTime.Now:yyyy-MM-dd} - Disabled per Inactivity Policy(90 days) - olddesc";
                }
                else if (AttributeName == "extensionAttribute8" || AttributeName == "extensionattribute8")
                {
                    AttributeValue = "DS";
                }
                else if (AttributeName == "mDBOverHardQuotaLimit")
                {
                    AttributeValue = "1024";
                }
                else if (AttributeName == "mDBOverQuotaLimit")
                {
                    AttributeValue = "1024";
                }
                else if (AttributeName == "mDBStorageQuota")
                {
                    AttributeValue = "1024";
                }
                else if (AttributeName == "mDBUseDefaults")
                {
                    AttributeValue = "FALSE";
                }
                else if (AttributeName == "msExchHideFromAddressList")
                {
                    AttributeValue = "TRUE";
                }
            }
        }

        /// <summary>
        /// Represents an account to be hidden from address lists.
        /// </summary>
        public class HiddenFromAddressListEnabled : DisableAccount { }

        /// <summary>
        /// Represents a member to be removed from a group.
        /// </summary>
        public class RemoveMember
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
            /// Gets or sets the account name to be removed.
            /// </summary>
            [JsonPropertyName("AccountName")]
            public string AccountName { get; set; } = "";

            /// <summary>
            /// Gets or sets the account domain to be removed.
            /// </summary>
            [JsonPropertyName("AccountDomain")]
            public string AccountDomain { get; set; } = "";

            /// <summary>
            /// Gets or sets the account to be removed.
            /// </summary>
            [JsonPropertyName("Account")]
            public DisableAccount Account { get; set; } = new DisableAccount();
        }

        /// <summary>
        /// Represents an account to be moved.
        /// </summary>
        public class MoveAccount : DisableAccount
        {
            /// <summary>
            /// Gets or sets the target OU.
            /// </summary>
            [JsonPropertyName("TargetOU")]
            public string TargetOU { get; set; } = "";
        }

        #endregion
    }
}