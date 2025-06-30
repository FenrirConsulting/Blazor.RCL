# RCL Project Structure

This document provides a comprehensive overview of the RCL project structure, outlining the organization and purpose of each directory and key files.

## Top-Level Directory Structure

```
Blazor.RCL/
├── Application/             # Application-level services, interfaces, and models
├── CompileSettings/         # Configuration for compilation
├── Domain/                  # Core domain entities
├── Automation/                    # Automation-specific components and data models
├── Infrastructure/          # Infrastructure services and implementations
├── Metadata/                # Project documentation and metadata
├── NLog/                    # Logging configuration
├── UIComponents/            # Reusable UI components
├── wwwroot/                 # Static web assets
├── _Imports.razor           # Razor component imports
├── _Imports.razor.cs        # C# code for imports
├── Blazor.RCL.csproj
└── nuget.config             # NuGet package configuration
```

## Application Directory

Contains application-level logic, interfaces, and models that define the core application services and data transfer objects.

```
Application/
├── Common/
│   └── Configuration/
│       └── Interfaces/
│           └── IAppConfiguration.cs  # Interface for application configuration settings 
├── Interfaces/               # Application service interfaces
│   ├── IApplicationNotificationProfileRepository.cs  # Repository for app notification profiles
│   ├── IApplicationNotificationProfileService.cs     # Service for app notification profiles
│   ├── IDistributedCacheRepository.cs  # Interface for distributed cache operations
│   ├── IEmailNotificationQueueRepository.cs         # Repository for email queue
│   ├── IEmailNotificationService.cs                  # Service for email notifications
│   ├── IEmailTemplateRepository.cs                   # Repository for email templates
│   ├── IEmailTemplateService.cs                      # Service for email templates
│   ├── ILDAPServerRepository.cs        # Interface for LDAP server management
│   ├── INotificationDeliveryRepository.cs           # Repository for notification delivery
│   ├── INotificationMessageRepository.cs            # Repository for notification messages
│   ├── INotificationPublisher.cs                    # Publisher abstraction for notifications
│   ├── INotificationService.cs                      # Core notification service interface
│   ├── IRemoteScriptRepository.cs      # Interface for remote script execution
│   ├── IRequestStatusCodeRepository.cs # Interface for request status code management
│   ├── IServerHostRepository.cs        # Interface for server host management
│   ├── IServiceAccountRepository.cs    # Interface for service account management
│   ├── IToolsConfigurationRepository.cs # Interface for tools configuration
│   ├── IToolsRequestRepository.cs      # Interface for tools request data access
│   ├── IToolsRequestService.cs         # Interface for tools request business logic
│   ├── IUserApplicationNotificationSettingsRepository.cs  # Repository for user app settings
│   ├── IUserNotificationSettingsRepository.cs       # Repository for user notification settings
│   ├── IUserNotificationSettingsService.cs          # Service for user notification settings
│   └── IUserSettingsRepository.cs      # Interface for user settings management
└── Models/                  # View models and DTOs
    ├── Configuration/                  # Configuration models
    │   └── NotificationSettings.cs     # Notification system settings model
    ├── Notifications/                  # Notification-related models
    │   ├── CreateApplicationProfileRequest.cs  # Request to create app profile
    │   ├── CreateNotificationRequest.cs        # Request to create notification
    │   ├── EmailTemplateModels.cs              # Email template related models
    │   ├── UpdateApplicationProfileRequest.cs  # Request to update app profile
    │   ├── UpdateUserNotificationSettingsRequest.cs  # Request to update user settings
    │   ├── UserNotificationSettingsSummary.cs  # Summary of user notification settings
    │   └── UserNotificationViewModel.cs         # View model for user notifications
    ├── LoginModel.cs                   # Model for login form data
    ├── LoginResponse.cs                # Model for login response data
    ├── NavLinkInfo.cs                  # Model for navigation link information
    ├── RoleUpdateRequest.cs            # Model for role update requests
    ├── UserInfoResponse.cs             # Model for user information responses
    └── ViewModule.cs                   # Model for view module configuration
```

### Key Components in Application

- **Interfaces**: Unified contracts for services with consolidated functionality
- **Models**: Data transfer objects (DTOs) for transferring data between layers
- **Common**: Shared application-level components and configurations

## Domain Directory

Contains core domain entities and business logic that represent the core business concepts.

```
Domain/
└── Entities/               # Core domain entities
    ├── Authentication/          # Authentication-related entities
    │   ├── ApplicationUser.cs      # Custom user entity
    │   ├── DistributedCacheEntry.cs # Distributed cache entry entity
    │   └── LdapServer.cs          # LDAP server configuration (Note: Also exists as LDAPServer.cs)
    ├── AzureAD/                # Azure AD integration entities
    │   ├── AKeyLessOptions.cs     # Options for AKeyless authentication
    │   ├── ApiPermission.cs       # API permission definitions
    │   └── AzureAdOptions.cs      # Azure AD configuration options
    ├── Configuration/          # Configuration entities
    │   ├── LDAPServer.cs          # LDAP server configuration
    │   ├── SmtpSettings.cs        # SMTP email configuration
    │   ├── TermedUserOU.cs        # Terminated user organizational unit config
    │   ├── ToolsConfiguration.cs  # Tools configuration settings
    │   └── UserSettings.cs        # User preferences and settings
    ├── RemoteScript.cs         # Remote script definition
    ├── RequestStatusCode.cs    # Status codes for requests
    ├── ServerHost.cs           # Server host definition
    ├── ServerHostConfig.cs     # Server host configuration
    ├── Notifications/           # Notification system entities
    │   ├── ApplicationNotificationProfile.cs  # Application notification configuration
    │   ├── EmailNotificationQueue.cs          # Email queue entity
    │   ├── EmailTemplate.cs                   # Email template entity
    │   ├── NotificationDelivery.cs            # Notification delivery tracking
    │   ├── NotificationEnums.cs               # Notification enumerations
    │   ├── NotificationMessage.cs             # Core notification message entity
    │   ├── UserApplicationNotificationSettings.cs  # Per-app user preferences
    │   └── UserNotificationSettings.cs        # Global user notification preferences
    ├── ServiceAccount.cs       # Service account definition
    └── ToolsRequest.cs         # Tools request data
```

### Key Components in Domain

#### Notification System
- **NotificationEnums**: Enums for notification types, severity, delivery methods, etc.
- **NotificationMessage**: Core notification entity with routing and metadata
- **NotificationDelivery**: Tracks delivery status per user
- **EmailNotificationQueue**: Queue for email processing with concurrency control
- **EmailTemplate**: Handlebars email templates with versioning
- **UserNotificationSettings**: Global user preferences (quiet hours, frequency)
- **ApplicationNotificationProfile**: Per-application configuration
- **UserApplicationNotificationSettings**: User preferences per application

## Automation Directory

Automation-specific components, data models, and services for Identity and Access Provisioning Framework functionality.

```
Automation/
├── Data/                    # Database contexts
│   ├── AutomationAppDbContext.cs         # DbContext for Automation application data
│   ├── AutomationBatchDbContext.cs       # DbContext for batch processing
│   ├── AutomationDirectoryDBContext.cs   # DbContext for directory services
│   ├── AutomationMailDbContext.cs        # DbContext for mail functionality
│   └── AutomationRequestDbContext.cs     # DbContext for request processing
├── AutomationApp/                 # App-specific entities
│   ├── AppDbConnectionInfo.cs      # Connection information for app database
│   ├── ExceptionLog.cs             # Entity for logging exceptions
│   ├── LineOfBusinessCode.cs       # Entity for line of business codes
│   ├── RequiredActionCode.cs       # Entity for required action codes
│   ├── RequiredActionEnvironment.cs # Entity for action environments
│   ├── TerminatedEmployee.cs       # Entity for terminated employee data
│   └── TerminatedEmployee_Action.cs # Actions for terminated employees
├── AutomationBatch/               # Batch processing entities
│   ├── AccessSubTypeCode_DNU.cs    # Access subtype codes (Do Not Use)
│   ├── AccessTypeCode_DBU.cs       # Access type codes (Database Use)
│   ├── BatchConfiguration.cs       # Configuration for batch processing
│   ├── BatchExecutionLog.cs        # Log for batch execution
│   ├── BatchFolderStructure.cs     # Folder structure for batch processing
│   ├── BatchRequest.cs             # Entity for batch requests
│   ├── BatchRequestDetail.cs       # Details for batch requests
│   ├── BatchRequestReprocess.cs    # Batch request reprocessing entity
│   ├── BatchRequestTypeCode.cs     # Batch request type codes
│   ├── BuildEnvironmentLookup.cs   # Build environment lookup data
│   ├── DetailDataTypeCode.cs       # Detail data type codes
│   ├── ExceptionLog.cs             # Exception logging for batch processing
│   ├── ExecutionStatusCode.cs      # Execution status codes
│   ├── RequestKeyDataCode.cs       # Request key data codes
│   └── RequestStatusCode.cs        # Status codes for requests
├── AutomationDirectory/           # Directory-specific repositories
│   ├── ADADRequest.cs              # AD directory requests
│   ├── ADAcctDisposition_Action.cs # AD account disposition actions
│   ├── ADAcctDisposition_Configs.cs # AD account disposition configurations
│   ├── ADAcctDisposition_Domains.cs # AD account disposition domains
│   ├── ADAcctDisposition_RevalidatedActions.cs # Revalidated AD actions
│   ├── ADAcctDisposition_Staging.cs # AD account disposition staging
│   ├── ADAcctDisposition_Users.cs  # AD account disposition users
│   ├── ADAcctDisposition_tempAction.cs # Temporary AD actions
│   ├── AzureId_Staging.cs          # Azure ID staging data
│   ├── ELDAPDARequest.cs           # LDAP directory access requests
│   ├── RACFUserInformation.cs      # RACF user information
│   ├── RRTELDAP_Action.cs          # RRT LDAP actions
│   ├── RemoteAccessBirthright.cs   # Remote access birthright data
│   ├── RemoteAccessBirthright_BU1.cs # Remote access birthright BU1
│   ├── TermedEmployeesGlide_Staging.cs # Termed employees staging
│   └── AutomationDirectoryRepositories/  # Repository implementations
│       ├── ADADRequestRepository.cs # AD request repository
│       ├── ADAcctDispositionActionRepository.cs # AD disposition repository
│       └── Interfaces/             # Repository interfaces
│           └── IADAcctDispositionActionRepository.cs # AD disposition interface
├── AutomationMail/                # Mail-related entities
│   ├── AetnaExchangeRequest.cs     # Aetna exchange mail requests
│   ├── AetnaRequest.cs             # Aetna mail requests
│   ├── BatchExecutionLog.cs        # Batch execution logging for mail
│   ├── BatchTypeCode.cs            # Batch type codes for mail
│   ├── EmailLookup.cs              # Email lookup functionality
│   ├── EmailLookupRaw.cs           # Raw email lookup data
│   ├── ExceptionLog.cs             # Exception logging for mail
│   ├── ExchangeRequest.cs          # Exchange mail requests
│   ├── ExchangeRequestDetail_DNU.cs # Exchange request details (Do Not Use)
│   ├── LookupData_DNU.cs           # Lookup data (Do Not Use)
│   ├── LookupKeyCode_DNU.cs        # Lookup key codes (Do Not Use)
│   └── MEUUser.cs                  # MEU user entity
├── AutomationRequest/             # Request processing
│   ├── AccessSubTypeCode.cs        # Access subtype codes
│   ├── AccessTypeCode.cs           # Access type codes
│   ├── AutomationTaskRepo.cs       # Automation task repository
│   ├── BatchRequestJson.cs         # Batch request JSON handling
│   ├── CatalogItemCode.cs          # Catalog item codes
│   ├── CatalogItemMap.cs           # Catalog item mapping
│   ├── Configuration.cs            # Request configuration
│   ├── DataTypeCode.cs             # Data type codes
│   ├── ErrorCode.cs                # Error codes
│   ├── ExceptionLog.cs             # Exception logging
│   ├── GenericPayloadModelWrapper.cs # Generic payload wrapper
│   ├── AutomationResponse.cs             # Automation response model
│   ├── Interfaces/                 # Request processing interfaces
│   │   ├── IRequestBuildItem.cs    # Interface for request build items
│   │   ├── IRequestBuilder.cs      # Interface for request builders
│   │   ├── IRequestLogRepository.cs # Interface for request log repository
│   │   └── IRequestPayloadModel.cs # Interface for request payload models
│   ├── LookupData.cs               # Lookup data
│   ├── LookupKeyCode.cs            # Lookup key codes
│   ├── POQNXTLookupAdd.cs          # POQNXT lookup add
│   ├── POQNXTLookupDelete.cs       # POQNXT lookup delete
│   ├── POQNXTLookupModify.cs       # POQNXT lookup modify
│   ├── QueryWebConfiguration.cs     # Query web configuration
│   ├── Repositories/               # Request repositories
│   │   ├── RequestLogRepository.cs # Request log repository
│   │   └── RequestRepository.cs    # Repository for request data
│   ├── RequestCountsOutsideAutomation.cs # Request counts outside Automation
│   ├── RequestDisableAETHPayloadModel.cs # Disable AETH payload
│   ├── RequestDisableLOAPayloadModel.cs # Disable LOA payload
│   ├── RequestDisableNeverLogonPayloadModel.cs # Disable never logon payload
│   ├── RequestDisablePayloadModel.cs # Disable payload model
│   ├── RequestDisposePayloadModel.cs # Dispose payload model
│   ├── RequestErrorLog.cs          # Request error logging
│   ├── RequestGetADAcctDispositionConfigPayloadModel.cs # Get AD config payload
│   ├── RequestJson.cs              # Request JSON handling
│   ├── RequestJsonParsed.cs        # Parsed request JSON
│   ├── RequestJsonParsedProcessed.cs # Processed parsed request JSON
│   ├── RequestLog.cs               # Request logging
│   ├── RequestMetadata.cs          # Request metadata
│   ├── RequestModel.cs             # Model for requests
│   ├── RequestReinstatePayloadModel.cs # Reinstate payload model
│   ├── RequestSchemaData.cs        # Request schema data
│   ├── RequestSchemaTemplate.cs    # Request schema template
│   ├── RequestSendMailAccountDisablementPayloadModel.cs # Mail disablement payload
│   ├── RequestSendMailPwdExpiresPayloadModel.cs # Password expiry mail payload
│   ├── RequestSourceCode.cs        # Request source codes
│   ├── RequestStatusCode.cs        # Status codes for requests
│   ├── RequestTask.cs              # Request task entity
│   ├── RequestTaskMetadata.cs      # Request task metadata
│   ├── RequestTask_Demo.cs         # Demo request tasks
│   ├── RequestUpdateADAcctDispositionActionPayloadModel.cs # Update AD action payload
│   ├── RequestsToArchive.cs        # Requests to archive
│   ├── SourceCatalogItemMap.cs     # Source catalog item mapping
│   ├── SourceInformation.cs        # Source information
│   ├── TaskStatusCode.cs           # Task status codes
│   └── ValidAccessTypeCode.cs      # Valid access type codes
├── AutomationTasks/               # Task-related entities
│   └── ADUserDetails.cs            # AD user details entity
└── Services/                # Automation services
    ├── APIServices.cs              # API service implementations
    ├── Interfaces/                 # Service interfaces
    │   ├── IRequestBuilder.cs      # Interface for request builder
    │   └── IRequestBuilderFactory.cs # Interface for request builder factory
    ├── RequestBuilderFactory.cs    # Factory for creating request builders
    ├── RequestJsonService.cs       # Service for handling request JSON
    ├── RequestResponseService.cs   # Service for request/response handling
    └── ResponseProcessingService.cs # Service for processing responses
```

### Key Components in Automation

- **Data Contexts**: Entity Framework contexts for different Automation subsystems
- **AutomationApp**: Core application entities for the Automation system
- **AutomationBatch**: Batch processing functionality for automated tasks
- **AutomationDirectory**: Directory service integration (AD, LDAP, RACF)
- **AutomationMail**: Email and Exchange integration services
- **AutomationRequest**: Request processing framework with builder pattern support
- **AutomationTasks**: Task scheduling and execution framework
- **Services**: Core Automation business services

## Infrastructure Directory

Infrastructure services, implementations, and cross-cutting concerns that support the application.

```
Infrastructure/
├── Authentication/          # Authentication services
│   ├── CookieHandler.cs              # Handles cookie-based authentication
│   ├── CustomAuthenticationStateProvider.cs # Custom auth state provider for Blazor
│   ├── CustomClaimsTransform.cs      # Custom claims transformation
│   ├── DomainUserGroupService.cs     # Service for domain user group management
│   ├── Interfaces/                   # Authentication interfaces
│   │   ├── ICookieHandler.cs         # Interface for cookie handling
│   │   ├── IDomainUserGroupService.cs # Interface for domain user groups
│   │   └── ILdapAuthenticationService.cs # Interface for LDAP authentication
│   └── LdapAuthenticationService.cs  # LDAP authentication implementation
├── Authorization/           # Authorization services
│   └── ClaimsPrincipalExtensions.cs  # Extensions for ClaimsPrincipal
├── BackgroundServices/      # Background services
│   ├── BackgroundRequestService.cs   # Service for background request processing
│   ├── EmailProcessingWorker.cs      # Worker for email queue processing
│   ├── EmailProcessingServiceExtensions.cs  # Extensions for email processing
│   ├── Interfaces/                   # Background service interfaces
│   │   └── IRequestPollingService.cs # Interface for request polling
│   ├── RequestPollingService.cs      # Implementation of request polling
│   ├── RequestProcessingServiceExtensions.cs # Extensions for request processing
│   └── RequestProcessingWorker.cs    # Worker service for request processing (enhanced for email)
├── Data/                    # Data access layer
│   └── Repositories/
├── Common/                  # Common infrastructure components
│   ├── Configuration/               # Configuration components
│   │   ├── ConfigurationInitializationService.cs # Service for config initialization
│   │   ├── LdapRoleMappingConfig.cs  # LDAP role mapping configuration
│   │   ├── LdapServerList.cs         # List of LDAP servers
│   │   ├── NavLinksInfoList.cs       # Navigation links configuration
│   │   ├── RemoteScriptList.cs       # List of remote scripts
│   │   ├── ServerAppConfiguration.cs # Server-side application configuration
│   │   ├── ServerHostList.cs         # List of server hosts
│   │   ├── ServiceAccountList.cs     # List of service accounts
│   │   └── UserGroupsCache.cs        # Cache for user groups
│   ├── Extensions/                  # Extension methods
│   │   ├── ConfigurationExtensions.cs # Extensions for configuration
│   │   ├── ServiceCollectionExtensions.cs # DI container extensions
│   │   └── StringExtensions.cs      # String utility extensions
│   ├── Logging/                    # Logging infrastructure
│   │   ├── LoggingExtensions.cs     # Extensions for logging
│   │   └── NLogConfiguration.cs    # NLog configuration
│   ├── Utilities/                  # Utility classes
│   │   ├── DateTimeProvider.cs      # DateTime abstraction
│   │   ├── FileSystem.cs           # File system operations
│   │   └── JsonSerializer.cs       # JSON serialization
│   └── CustomDistributedCache.cs   # Custom distributed cache implementation
├── Data/                    # Data access infrastructure
│   ├── AppDbContext.cs           # Entity Framework context (includes notification entities)
│   └── Repositories/             # Repository implementations
│       ├── ApplicationNotificationProfileRepository.cs  # App notification profile repository
│       ├── DistributedCacheRepository.cs # Distributed cache repository
│       ├── EmailNotificationQueueRepository.cs  # Email queue repository
│       ├── EmailTemplateRepository.cs           # Email template repository
│       ├── LdapServerRepository.cs      # LDAP server repository
│       ├── NotificationDeliveryRepository.cs    # Notification delivery repository
│       ├── NotificationMessageRepository.cs     # Notification message repository
│       ├── RemoteScriptRepository.cs    # Remote script repository
│       ├── RequestStatusCodeRepository.cs # Request status code repository
│       ├── ServerHostRepository.cs      # Server host repository
│       ├── ServiceAccountRepository.cs  # Service account repository
│       ├── ToolsConfigurationRepository.cs # Tools configuration repository
│       ├── ToolsRequestRepository.cs    # Tools request repository
│       ├── UserApplicationNotificationSettingsRepository.cs  # User app settings repository
│       ├── UserNotificationSettingsRepository.cs  # User notification settings repository
│       └── UserSettingsRepository.cs    # User settings repository (enhanced with roles)
├── Extensions/              # Infrastructure extensions
│   └── ServiceCollectionExtensions.cs # DI container extensions (includes notification services)
├── Hubs/                    # SignalR hubs
│   └── NotificationHub.cs         # SignalR hub for real-time notifications
├── Navigation/              # Navigation infrastructure
│   └── IdentityRedirectManager.cs # Identity redirect management
├── Services/                # Infrastructure services
│   ├── AKeylessManager.cs         # AKeyless secret management
│   ├── ApplicationNotificationProfileService.cs  # App notification profile service
│   ├── AzureAdOptionsService.cs   # Azure AD options service
│   ├── ClaimsUpdateService.cs     # Claims update service
│   ├── CustomCircuitHandler.cs    # Custom Blazor circuit handler
│   ├── EmailNotificationService.cs # Email notification service
│   ├── EmailTemplateService.cs     # Email template service
│   ├── ErrorHandlerService.cs     # Error handling service
│   ├── InitializationService.cs   # Application initialization service
│   ├── Interfaces/                # Service interfaces
│   │   ├── IAKeylessManager.cs    # Interface for AKeyless manager
│   │   ├── IAzureAdOptions.cs     # Interface for Azure AD options
│   │   ├── IClaimsUpdateService.cs # Interface for claims update
│   │   ├── IInitilizationService.cs # Interface for initialization
│   │   ├── INotificationManager.cs # Interface for notification manager
│   │   ├── IRegistryHelperService.cs # Interface for registry helper
│   │   ├── IRequestRefresh.cs     # Interface for request refresh
│   │   ├── ISharePointService.cs  # Interface for SharePoint
│   │   ├── IThemeService.cs       # Interface for theme service
│   │   └── IUserSettingsService.cs # Interface for user settings
│   ├── LocalNotificationPublisher.cs  # Local notification publisher (polling mode)
│   ├── NotificationManager.cs     # Notification management service
│   ├── NotificationService.cs     # Core notification service
│   ├── RedisNotificationPublisher.cs  # Redis notification publisher (real-time)
│   ├── RegistryHelperService.cs   # Registry helper service
│   ├── RequestRefresh.cs          # Request refresh service
│   ├── SharePointService.cs       # SharePoint integration service
│   ├── ThemeService.cs            # Theme management service
│   ├── ToolsRequestService.cs     # Tools request service
│   ├── UserNotificationSettingsService.cs  # User notification settings service
│   └── UserSettingsService.cs     # User settings service (enhanced with role capture)
└── Themes/                  # Theme infrastructure
    └── PaletteThemes.cs          # Theme palette definitions


## Metadata Directory

Project documentation and metadata for knowledge management.

```
Metadata/
├── Decisions/               # Architectural decision records
│   └── DecisionLog.md             # Log of architectural decisions
├── Knowledge/               # Project knowledge base
│   ├── ArchitectureOverview.md    # Overview of system architecture
│   ├── ProjectStructure.md        # This document - project structure
│   └── UIComponentsCheatSheet.md  # Reference for UI components
├── CurrentState.md          # Current project state
├── MetadataGuide.md         # Guide for metadata
├── ProjectOverview.md       # Overview of the project
├── PromptTemplate.md        # Templates for documentation
└── SessionIndex.md          # Index of development sessions
```

### Key Components in Metadata

- **Decisions**: Documentation of architectural decisions and their rationale
- **Knowledge**: Technical documentation and knowledge base
- **Project Documentation**: Overall project information and guides

## NLog Directory

Logging configuration and services using NLog framework.

```
NLog/
└── LogService/              # Logging services
    ├── Interface/
    │   └── ILogService.cs          # Interface for logging services
    ├── LogConfiguration.cs         # Configuration for NLog
    ├── LogService.cs               # Implementation of logging service
    └── NLogConfiguration.xml       # XML configuration for NLog
```

### Key Components in NLog

- **LogService**: Implementation of logging services using NLog
- **Configuration**: NLog configuration and setup

## UIComponents Directory

Reusable UI components for Blazor applications built with this library.

```
UIComponents/
└── Components/              # UI component library
    ├── Admin/                      # Administrative UI components
    │   ├── AdminActionButton.razor # Button for admin actions
    │   ├── AdminDataGrid.razor     # Data grid for admin interfaces
    │   ├── AdminDataTable.razor    # Data table for admin interfaces
    │   ├── AdminDialog.razor       # Dialog for admin interfaces
    │   ├── AdminExpandableSection.razor # Expandable section for admin UIs
    │   ├── AdminFilterBar.razor    # Filter bar for admin interfaces
    │   ├── AdminSection.razor      # Section container for admin UIs
    │   └── AdminStatusBadge.razor  # Status badge for admin interfaces
    ├── Auth/                       # Authentication UI components
    │   ├── LoginDisplay.razor      # Component for login display
    │   ├── LoginForm.razor         # Form for user login
    │   └── UserProfile.razor       # Component for user profile display
    ├── Common/                     # Common UI components
    │   ├── Alert.razor             # Alert component
    │   ├── Breadcrumbs.razor       # Breadcrumbs navigation
    │   ├── ConfirmDialog.razor     # Confirmation dialog
    │   ├── LoadingIndicator.razor  # Loading indicator/spinner
    │   ├── PageTitle.razor         # Page title component
    │   └── Tooltip.razor           # Tooltip component
    ├── Data/                       # Data display components
    │   ├── DataGrid.razor          # Data grid component
    │   ├── DataTable.razor         # Data table component
    │   ├── Pagination.razor        # Pagination control
    │   └── SearchBar.razor         # Search bar component
    ├── Forms/                      # Form components
    │   ├── AutoComplete.razor      # Auto-complete input
    │   ├── Checkbox.razor          # Checkbox input
    │   ├── DatePicker.razor        # Date picker component
    │   ├── DropDown.razor          # Dropdown/select component
    │   ├── FormField.razor         # Form field container
    │   ├── InputMask.razor         # Input with mask
    │   ├── RadioButton.razor       # Radio button input
    │   ├── TextArea.razor          # Text area input
    │   └── TextField.razor         # Text field input
    ├── JSON/                       # JSON handling components
    │   ├── JsonArray.razor         # Component for JSON array visualization
    │   ├── JsonDialog.razor        # Dialog for JSON display
    │   ├── JsonEditor.razor        # Component for editing JSON
    │   ├── JsonNode.razor          # Component for JSON node display
    │   ├── JsonObject.razor        # Component for JSON object visualization
    │   ├── JsonValue.razor         # Component for JSON value display
    │   ├── RequestDataViewer.razor # Dialog for viewing request JSON data
    │   └── ResponseDataViewer.razor # Dialog for viewing response JSON data
    ├── Layout/                     # Layout components
    │   ├── MainLayout.razor        # Main layout template
    │   ├── PageContainer.razor     # Page container component
    │   ├── Panel.razor             # Panel component
    │   ├── SideNav.razor           # Side navigation component
    │   └── Tabs.razor              # Tabs component
    ├── Navigation/                 # Navigation components
    │   ├── MainNav.razor           # Main navigation component
    │   ├── NavMenu.razor           # Navigation menu component
    │   └── SideMenu.razor          # Side menu component
    ├── AppBar.razor                # Application bar component
    ├── HelpButton.razor            # Help button component
    ├── HelpPanel.razor             # Help panel component
    ├── HelpSystem.razor            # Help system component
    ├── MenuBar.razor               # Menu bar component
    ├── MenuBar.razor.cs            # Menu bar code-behind
    ├── MenuBar.razor.css           # Menu bar styles
    ├── ScriptVariablesEditor.razor # Script variables editor
    ├── ScriptVariablesEditor.razor.cs # Script variables editor code-behind
    └── ScriptVariablesEditor.razor.css # Script variables editor styles
├── Layouts/                 # Layout components
│   ├── MainLayout.razor            # Main application layout
│   ├── MainLayout.razor.cs         # Main layout code-behind
│   ├── MainLayout.razor.css        # Main layout styles
│   ├── ModuleLayout.razor          # Module-specific layout
│   ├── ModuleLayout.razor.cs       # Module layout code-behind
│   ├── ModuleLayout.razor.css      # Module layout styles
│   ├── PublicLayout.razor          # Public pages layout
│   ├── PublicLayout.razor.cs       # Public layout code-behind
│   └── PublicLayout.razor.css      # Public layout styles
└── Pages/                   # Page components
    ├── Admin/                      # Administrative pages
    │   ├── AdminBase.razor         # Base admin page component
    │   ├── AdminBase.razor.cs      # Admin base code-behind
    │   ├── AdminBase.razor.css     # Admin base styles
    │   └── BaseAdminComponent.cs   # Base class for admin components
    ├── Error.razor                 # Error page component
    └── Error.razor.cs              # Error page code-behind
```

### Key Components in UIComponents

- **Admin Components**: UI components specifically designed for administrative interfaces
- **Auth Components**: Components for authentication and user profile management
- **Common Components**: General-purpose UI components used throughout the application
- **Data Components**: Components for displaying and interacting with data
- **Form Components**: Input and form-related components
- **Layout Components**: Components for page and application layout
- **Navigation Components**: Components for navigation and menus
- **Help System**: Components for the integrated help system

## wwwroot Directory

Static web assets such as CSS, JavaScript, and images.

```
wwwroot/
├── css/                     # Stylesheets
│   ├── app.css              # Main application styles
│   ├── bootstrap/           # Bootstrap CSS framework
│   └── themes/              # Theme-specific stylesheets
├── fonts/                   # Font files
│   └── icons/               # Icon fonts
├── images/                  # Image assets
│   ├── icons/               # Icon images
│   └── logos/               # Logo images
└── js/                      # JavaScript files
    ├── app.js               # Main application script
    ├── interop/             # JavaScript interop files
    └── vendor/              # Third-party JavaScript libraries
```

### Key Components in wwwroot

- **CSS**: Stylesheets for the application, including themes
- **Fonts**: Font files used by the application
- **Images**: Image assets, including icons and logos
- **JavaScript**: Client-side scripts and interop files
- **TypeScript**: Strongly-typed clients for services

## Key Configuration Files

- **Blazor.RCL.csproj**: Project configuration file defining dependencies and build settings
- **_Imports.razor**: Razor component imports for the library, providing common using statements
- **CompileSettings/**: Contains compilation configuration files for the build process


## Documentation Files

Additional documentation files for specific components or issues:
