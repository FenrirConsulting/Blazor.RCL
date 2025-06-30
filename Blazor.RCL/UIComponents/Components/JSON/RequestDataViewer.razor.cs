using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Text.Json;
using System.Text.Json.Serialization;
using Blazor.RCL.Application.Interfaces;
using Blazor.RCL.NLog.LogService.Interface;
using System.Text.RegularExpressions;

namespace Blazor.RCL.UIComponents.Components.JSON
{
    /// <summary>
    /// Custom dialog component for viewing request data in a structured format.
    /// Supports both specific known request types and generic request structures.
    /// </summary>
    public partial class RequestDataViewer : ComponentBase
    {
        #region Parameters and Injected Services
        [CascadingParameter] IMudDialogInstance MudDialog { get; set; } = default!;
        [Inject] private ILogHelper Logger { get; set; } = default!;

        /// <summary>
        /// The original JSON request data to display.
        /// </summary>
        [Parameter] public string JsonContent { get; set; } = string.Empty;
        #endregion

        #region Private Fields
        private bool IsOriginalJsonVisible { get; set; } = false;
        private string OriginalJson => JsonContent;
        
        // Request type detection
        private bool IsReinstateRequest => JsonContent?.Contains("EnableAccounts") == true;
        private bool IsDisposeRequest => JsonContent?.Contains("DisableAccounts") == true;
        private string RequestType { get; set; } = "Generic Request";
        private Color RequestTypeColor => DetermineRequestTypeColor();
        
        // Common fields
        private string EmployeeId { get; set; } = string.Empty;
        private Dictionary<string, object> GenericProperties { get; set; } = new();
        
        // Structured property collections for better display
        private List<KeyValuePair<string, string>> MetadataProperties { get; set; } = new();
        private List<KeyValuePair<string, string>> SimpleProperties { get; set; } = new();
        private List<KeyValuePair<string, string>> ComplexProperties { get; set; } = new();
        private Dictionary<string, List<Dictionary<string, string>>> ArrayProperties { get; set; } = new();
        private Dictionary<string, List<Dictionary<string, string>>> ItemDataArrays { get; set; } = new();
        #endregion

        #region Lifecycle Methods
        /// <summary>
        /// Initialize component and parse the JSON data.
        /// </summary>
        protected override void OnInitialized()
        {
            try
            {
                if (!string.IsNullOrEmpty(JsonContent))
                {
                    ParseRequestData();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Error parsing request data JSON", ex);
            }
        }
        #endregion

        #region UI Event Handlers
        /// <summary>
        /// Close the dialog.
        /// </summary>
        private void Close() => MudDialog.Close();

        /// <summary>
        /// Toggle the visibility of the original JSON section.
        /// </summary>
        private void ToggleOriginalJson(bool value)
        {
            IsOriginalJsonVisible = value;
            StateHasChanged();
        }
        #endregion

        #region JSON Parsing Methods
        /// <summary>
        /// Parse the JSON request data into the structured model.
        /// </summary>
        private void ParseRequestData()
        {
            try
            {
                // Clear all collections
                MetadataProperties.Clear();
                SimpleProperties.Clear();
                ComplexProperties.Clear();
                ArrayProperties.Clear();
                ItemDataArrays.Clear();
                
                // Use System.Text.Json for parsing
                using var document = JsonDocument.Parse(JsonContent);
                var root = document.RootElement;

                // Log the entire JSON for debugging
                Logger.LogMessage($"Processing JSON content of length: {JsonContent.Length}");

                // Determine request type based on JSON content
                DetermineRequestType(root);

                // Extract standard metadata properties from the root level
                ExtractStandardMetadata(root);
                
                // Extract ItemData properties (both simple properties and arrays)
                if (root.TryGetProperty("ItemData", out var itemData) && 
                    itemData.ValueKind == JsonValueKind.Object)
                {
                    // Process simple properties in ItemData
                    ExtractItemDataProperties(itemData);
                    
                    // Process arrays in ItemData separately for table display
                    ProcessArraysInItemData(itemData);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Error parsing request data", ex);
            }
        }

        /// <summary>
        /// Determine the request type based on JSON content.
        /// </summary>
        private void DetermineRequestType(JsonElement root)
        {
            try
            {
                // Check for known request types
                if (root.TryGetProperty("EnableAccounts", out _))
                {
                    RequestType = "Reinstate Request";
                }
                else if (root.TryGetProperty("DisableAccounts", out _))
                {
                    RequestType = "Dispose Request";
                }
                // Check for AccessType and AccessSubType
                else if (root.TryGetProperty("AccessType", out var accessTypeElement) && 
                         root.TryGetProperty("AccessSubType", out var accessSubTypeElement))
                {
                    var accessType = accessTypeElement.GetString() ?? "";
                    var accessSubType = accessSubTypeElement.GetString() ?? "";
                    RequestType = $"{accessSubType} {accessType} Request";
                }
                // Check for generic properties that might indicate type
                else if (root.TryGetProperty("ItemData", out var itemData))
                {
                    if (itemData.TryGetProperty("RequestType", out var requestTypeElement))
                    {
                        RequestType = requestTypeElement.GetString() ?? "Generic Request";
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Error determining request type", ex);
            }
        }

        /// <summary>
        /// Extract the Employee ID from various possible locations in the JSON
        /// </summary>
        private void ExtractEmployeeId(JsonElement root)
        {
            try
            {
                // Check root level first
                if (root.TryGetProperty("EmployeeId", out var employeeIdElement))
                {
                    EmployeeId = employeeIdElement.GetString() ?? string.Empty;
                    if (!string.IsNullOrEmpty(EmployeeId))
                    {
                        MetadataProperties.Add(new KeyValuePair<string, string>("EmployeeId", EmployeeId));
                        return;
                    }
                }
                
                // Check ItemData if not found at root level
                if (root.TryGetProperty("ItemData", out var itemData) && 
                    itemData.TryGetProperty("EmployeeId", out employeeIdElement))
                {
                    EmployeeId = employeeIdElement.GetString() ?? string.Empty;
                    if (!string.IsNullOrEmpty(EmployeeId))
                    {
                        MetadataProperties.Add(new KeyValuePair<string, string>("EmployeeId", EmployeeId));
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Error extracting EmployeeId", ex);
            }
        }

        /// <summary>
        /// Extract simple properties from ItemData for display.
        /// </summary>
        private void ExtractItemDataProperties(JsonElement itemData)
        {
            try
            {
                Logger.LogMessage("Processing simple properties in ItemData");
                
                // Process all simple properties in ItemData (excluding arrays which are handled separately)
                foreach (var property in itemData.EnumerateObject())
                {
                    // Skip arrays as they're handled separately in ProcessArraysInItemData
                    if (property.Value.ValueKind == JsonValueKind.Array)
                        continue;
                    
                    Logger.LogMessage($"Processing ItemData property: {property.Name} with ValueKind: {property.Value.ValueKind}");
                    
                    // Process based on property type
                    switch (property.Value.ValueKind)
                    {
                        case JsonValueKind.Object:
                            // Process nested objects
                            ProcessObjectProperty(property.Name, property.Value);
                            break;
                            
                        case JsonValueKind.String:
                            // Handle string values (including EmployeeId)
                            var stringValue = property.Value.GetString() ?? string.Empty;
                            if (!string.IsNullOrEmpty(stringValue))
                            {
                                SimpleProperties.Add(new KeyValuePair<string, string>(property.Name, stringValue));
                            }
                            break;
                            
                        case JsonValueKind.Number:
                            // Handle numeric values
                            var numberValue = property.Value.GetRawText();
                            SimpleProperties.Add(new KeyValuePair<string, string>(property.Name, numberValue));
                            break;
                            
                        case JsonValueKind.True:
                        case JsonValueKind.False:
                            // Handle boolean values
                            var boolValue = property.Value.GetBoolean().ToString();
                            SimpleProperties.Add(new KeyValuePair<string, string>(property.Name, boolValue));
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Error extracting ItemData properties", ex);
            }
        }
        
        /// <summary>
        /// Process an object property for structured display.
        /// </summary>
        private void ProcessObjectProperty(string propertyName, JsonElement objectElement)
        {
            // Format as JSON for display
            var formattedJson = JsonSerializer.Serialize(objectElement, new JsonSerializerOptions { WriteIndented = true });
            GenericProperties[propertyName] = formattedJson;
            ComplexProperties.Add(new KeyValuePair<string, string>(propertyName, formattedJson));
            
            // For "AdditionalInfo" objects, also extract as simple properties for better display
            if (propertyName.Contains("Info", StringComparison.OrdinalIgnoreCase))
            {
                foreach (var subProp in objectElement.EnumerateObject())
                {
                    if (subProp.Value.ValueKind == JsonValueKind.String || 
                        subProp.Value.ValueKind == JsonValueKind.Number ||
                        subProp.Value.ValueKind == JsonValueKind.True ||
                        subProp.Value.ValueKind == JsonValueKind.False)
                    {
                        string value = subProp.Value.ValueKind == JsonValueKind.String ?
                            subProp.Value.GetString() ?? string.Empty :
                            subProp.Value.GetRawText();
                            
                        SimpleProperties.Add(new KeyValuePair<string, string>(
                            $"{propertyName}.{subProp.Name}", value));
                    }
                }
            }
        }
        
        /// <summary>
        /// Extract the standard metadata properties that should always appear at the top level.
        /// </summary>
        private void ExtractStandardMetadata(JsonElement root)
        {
            try
            {
                // Define the metadata properties we want to extract (in order)
                string[] metadataProps = new[] 
                { 
                    "SourceId", "Source", "Request", "RequestItem", 
                    "CatalogItem", "AccessType", "AccessSubType" 
                };
                
                // Extract each metadata property from the root level
                foreach (string propName in metadataProps)
                {
                    if (root.TryGetProperty(propName, out var propValue) && 
                        propValue.ValueKind == JsonValueKind.String)
                    {
                        string value = propValue.GetString() ?? string.Empty;
                        if (!string.IsNullOrEmpty(value))
                        {
                            MetadataProperties.Add(new KeyValuePair<string, string>(propName, value));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Error extracting standard metadata properties", ex);
            }
        }
        
        /// <summary>
        /// Process the arrays inside ItemData to display as tables.
        /// </summary>
        private void ProcessArraysInItemData(JsonElement itemData)
        {
            try
            {
                Logger.LogMessage("Processing arrays inside ItemData");
                
                // Look for array properties inside ItemData
                foreach (var property in itemData.EnumerateObject())
                {
                    if (property.Value.ValueKind == JsonValueKind.Array)
                    {
                        var arrayName = property.Name;
                        Logger.LogMessage($"Found array in ItemData: {arrayName} with {property.Value.GetArrayLength()} items");
                        
                        // Skip processing empty arrays
                        if (property.Value.GetArrayLength() == 0)
                        {
                            Logger.LogMessage($"Array {arrayName} is empty, skipping");
                            continue;
                        }
                        
                        // Process array into structured data
                        ProcessArrayContent(arrayName, property.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Error processing arrays in ItemData", ex);
            }
        }
        
        /// <summary>
        /// Process any array properties in the JSON element.
        /// </summary>
        private void ProcessArrayProperties(JsonElement element)
        {
            try
            {
                Logger.LogMessage("Processing array properties in root element");
                
                // Find all array properties at the root level
                foreach (var property in element.EnumerateObject())
                {
                    if (property.Value.ValueKind == JsonValueKind.Array)
                    {
                        var arrayName = property.Name;
                        Logger.LogMessage($"Found array property: {arrayName} with {property.Value.GetArrayLength()} items");
                        
                        // Special handling for known array types
                        if (arrayName == "EnableAccounts" || arrayName == "DisableAccounts" ||
                            arrayName == "SetAttributes" || arrayName == "HiddenFromAddressListsEnabled" ||
                            arrayName == "HiddenFromAddressListsDisabled" || arrayName == "AddMembers" ||
                            arrayName == "RemoveMembers" || arrayName == "MoveAccounts")
                        {
                            Logger.LogMessage($"Processing known array type: {arrayName}");
                            ProcessArrayContent(arrayName, property.Value);
                        }
                        else
                        {
                            // Handle generic arrays
                            ProcessArrayContent(arrayName, property.Value);
                        }
                    }
                }
                
                // Now check if we have ItemData with arrays inside
                if (element.TryGetProperty("ItemData", out var itemData) && itemData.ValueKind == JsonValueKind.Object)
                {
                    Logger.LogMessage("Processing arrays inside ItemData");
                    foreach (var property in itemData.EnumerateObject())
                    {
                        if (property.Value.ValueKind == JsonValueKind.Array)
                        {
                            var arrayName = property.Name;
                            Logger.LogMessage($"Found array in ItemData: {arrayName} with {property.Value.GetArrayLength()} items");
                            
                            // Process array in ItemData with the same naming convention as root arrays
                            ProcessArrayContent(arrayName, property.Value);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError("Error processing array properties", ex);
            }
        }
        
        /// <summary>
        /// Process an array content and build structured data representation.
        /// </summary>
        private void ProcessArrayContent(string arrayName, JsonElement arrayElement)
        {
            try
            {
                Logger.LogMessage($"Processing array content for {arrayName} with {arrayElement.GetArrayLength()} items");
                
                // Skip empty arrays
                if (arrayElement.GetArrayLength() == 0)
                {
                    Logger.LogMessage($"Array {arrayName} is empty, skipping");
                    return;
                }
                
                // Extract as structured data for table display
                var items = new List<Dictionary<string, string>>();
                var allKeys = new HashSet<string>();
                
                // Add Action column for specific array types
                if (arrayName == "EnableAccounts")
                    allKeys.Add("Action");
                else if (arrayName == "DisableAccounts")
                    allKeys.Add("Action");
                else if (arrayName == "HiddenFromAddressListsEnabled")
                    allKeys.Add("Action");
                else if (arrayName == "HiddenFromAddressListsDisabled")
                    allKeys.Add("Action");
                    
                // First pass - collect all possible keys from all items
                foreach (var item in arrayElement.EnumerateArray())
                {
                    if (item.ValueKind == JsonValueKind.Object)
                    {
                        foreach (var prop in item.EnumerateObject())
                        {
                            allKeys.Add(prop.Name);
                        }
                    }
                }
                
                Logger.LogMessage($"Collected keys for {arrayName}: {string.Join(", ", allKeys)}");
                
                // Second pass - build dictionaries with all keys
                foreach (var item in arrayElement.EnumerateArray())
                {
                    if (item.ValueKind == JsonValueKind.Object)
                    {
                        var dict = new Dictionary<string, string>();
                        
                        // Initialize all keys with empty values
                        foreach (var key in allKeys)
                        {
                            dict[key] = string.Empty;
                        }
                        
                        // Add action value for known array types
                        if (arrayName == "EnableAccounts")
                            dict["Action"] = "Enable";
                        else if (arrayName == "DisableAccounts")
                            dict["Action"] = "Disable";
                        else if (arrayName == "HiddenFromAddressListsEnabled")
                            dict["Action"] = "Hide from Address Lists";
                        else if (arrayName == "HiddenFromAddressListsDisabled")
                            dict["Action"] = "Show in Address Lists";
                        else if (arrayName == "AddMembers")
                            dict["Action"] = "Add to Group";
                        else if (arrayName == "RemoveMembers")
                            dict["Action"] = "Remove from Group";
                        
                        // Fill in actual values
                        foreach (var prop in item.EnumerateObject())
                        {
                            if (prop.Value.ValueKind == JsonValueKind.String)
                            {
                                dict[prop.Name] = prop.Value.GetString() ?? string.Empty;
                            }
                            else if (prop.Value.ValueKind == JsonValueKind.Object)
                            {
                                // Handle special case for group member objects
                                if (prop.Name == "Account" && arrayName.Contains("Members"))
                                {
                                    if (prop.Value.TryGetProperty("AccountName", out var accountNameElement))
                                        dict["AccountName"] = accountNameElement.GetString() ?? string.Empty;
                                    if (prop.Value.TryGetProperty("AccountDomain", out var accountDomainElement))
                                        dict["AccountDomain"] = accountDomainElement.GetString() ?? string.Empty;
                                }
                                else
                                {
                                    // For other objects, show a formatted representation
                                    dict[prop.Name] = JsonSerializer.Serialize(prop.Value, new JsonSerializerOptions { WriteIndented = false });
                                }
                            }
                            else if (prop.Value.ValueKind == JsonValueKind.Array)
                            {
                                // For arrays, show count
                                dict[prop.Name] = $"[{prop.Value.GetArrayLength()} items]";
                            }
                            else
                            {
                                dict[prop.Name] = prop.Value.GetRawText();
                            }
                        }
                        items.Add(dict);
                    }
                }
                
                if (items.Any())
                {
                    Logger.LogMessage($"Added {items.Count} items for array {arrayName}");
                    ItemDataArrays[arrayName] = items;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error processing array {arrayName}", ex);
            }
        }
        
        /// <summary>
        /// Process an array property for structured display.
        /// </summary>
        private void ProcessArrayProperty(string propertyName, JsonElement arrayElement)
        {
            // Format as JSON for display
            var formattedJson = JsonSerializer.Serialize(arrayElement, new JsonSerializerOptions { WriteIndented = true });
            GenericProperties[propertyName] = formattedJson;
            ComplexProperties.Add(new KeyValuePair<string, string>(propertyName, formattedJson));
            
            // Also extract as structured data for tabled display
            var items = new List<Dictionary<string, string>>();
            
            foreach (var item in arrayElement.EnumerateArray())
            {
                if (item.ValueKind == JsonValueKind.Object)
                {
                    var dict = new Dictionary<string, string>();
                    foreach (var prop in item.EnumerateObject())
                    {
                        if (prop.Value.ValueKind == JsonValueKind.String)
                        {
                            dict[prop.Name] = prop.Value.GetString() ?? string.Empty;
                        }
                        else
                        {
                            dict[prop.Name] = prop.Value.GetRawText();
                        }
                    }
                    items.Add(dict);
                }
            }
            
            if (items.Any())
            {
                ArrayProperties[propertyName] = items;
            }
        }
        
        /// <summary>
        /// Determine the color to use for the request type header.
        /// </summary>
        private Color DetermineRequestTypeColor()
        {
            if (IsReinstateRequest) return Color.Success;
            if (IsDisposeRequest) return Color.Error;
            return Color.Primary;
        }
        
        /// <summary>
        /// Get a friendly display name for a property or array name
        /// </summary>
        private string GetDisplayName(string name)
        {
            // Handle special array names
            if (name.EndsWith("Accounts", StringComparison.OrdinalIgnoreCase))
            {
                string action = name.StartsWith("Enable", StringComparison.OrdinalIgnoreCase) ? "Enable" : "Disable";
                return $"{action} Account Actions";
            }
            
            if (name.Equals("SetAttributes", StringComparison.OrdinalIgnoreCase))
                return "Attribute Changes";
                
            if (name.Contains("HiddenFromAddressLists", StringComparison.OrdinalIgnoreCase))
                return "Address List Visibility Changes";
                
            if (name.Contains("Member", StringComparison.OrdinalIgnoreCase))
                return "Group Membership Changes";
                
            if (name.Equals("MoveAccounts", StringComparison.OrdinalIgnoreCase))
                return "Account Location Changes";
                
            // Format camelCase or PascalCase names
            if (string.IsNullOrEmpty(name))
                return string.Empty;
                
            var result = Regex.Replace(name, "([A-Z])", " $1").Trim();
            return char.ToUpper(result[0]) + result.Substring(1);
        }
        
        /// <summary>
        /// Get appropriate icon for an array type based on its name
        /// </summary>
        private string GetIconForArrayType(string arrayName)
        {
            if (arrayName.EndsWith("Accounts", StringComparison.OrdinalIgnoreCase))
                return Icons.Material.Filled.AccountCircle;
                
            if (arrayName.Equals("SetAttributes", StringComparison.OrdinalIgnoreCase))
                return Icons.Material.Filled.Edit;
                
            if (arrayName.Contains("HiddenFromAddressLists", StringComparison.OrdinalIgnoreCase))
                return Icons.Material.Filled.Visibility;
                
            if (arrayName.Contains("Member", StringComparison.OrdinalIgnoreCase))
                return Icons.Material.Filled.Group;
                
            if (arrayName.Equals("MoveAccounts", StringComparison.OrdinalIgnoreCase))
                return Icons.Material.Filled.DriveFileMove;
                
            // Default icon
            return Icons.Material.Filled.List;
        }

        #endregion
    }
}
