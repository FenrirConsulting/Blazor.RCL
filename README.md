# Blazor.RCL

## Overview
Blazor.RCL is a Razor Class Library that serves as the foundational framework for the Blazor Enterprise Architecture. It provides shared components, services, and configurations that streamline application development and ensure consistency across the ecosystem.

## Key Features
- 🔄 Shared dependency management
- 🔐 Authentication service integration
- 🗄️ Database connectivity setup
- 📦 Common component library
- ⚙️ Configuration management
- 🔌 Service registration helpers

## Core Components

### Authentication
```csharp
services.AddBlazorAuthentication(options => {
    options.AuthenticationEndpoint = "your-auth-endpoint";
    options.TokenValidation = true;
    options.UseAzureAD = true;
});
```

### Database Configuration
```csharp
services.AddDatabaseConfiguration(options => {
    options.ConnectionString = configuration.GetConnectionString("Default");
    options.EnableRetryOnFailure = true;
    options.UseAzureKeyVault = true;
});
```

### Service Registration
```csharp
services.AddRclServices(options => {
    options.IncludeAuthentication = true;
    options.IncludeLogging = true;
    options.IncludeMonitoring = true;
});
```

## Shared Components

### UI Components
- Navigation menu
- Loading spinners
- Error boundaries
- Modal dialogs
- Toast notifications
- Data grids

### Service Components
- HTTP interceptors
- Auth handlers
- Error handlers
- Logging services
- State management

## Integration Guide

### 1. Install Package
```xml
<PackageReference Include="Blazor.RCL" Version="1.0.0" />
```

### 2. Configure Services
```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddRclServices(Configuration);
    services.AddRclAuthentication(Configuration);
    services.AddRclDatabase(Configuration);
}
```

### 3. Import Components
```razor
@using Blazor.RCL.Components
@using Blazor.RCL.Services
```

## Configuration Options

### Authentication
- Azure AD settings
- Cookie policies
- Token validation
- Session management

### Database
- Connection strings
- Retry policies
- Migration settings
- Query logging

### Logging
- Application Insights
- Log levels
- Event filtering
- Performance tracking

## Best Practices
1. Use provided base classes
2. Implement interface contracts
3. Follow naming conventions
4. Use dependency injection
5. Handle errors appropriately

## Security Features
- Secure configuration management
- Authentication integration
- Authorization policies
- Data protection
- Audit logging

## Development Workflow
1. Install NuGet package
2. Configure services
3. Import namespaces
4. Use components and services
5. Implement custom logic

## Contributing
1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## License
This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Author
Christopher Olson - Senior Security Engineer