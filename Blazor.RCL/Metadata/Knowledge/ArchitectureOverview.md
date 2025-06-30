# Blazor.RCL - Architecture Overview

This document provides a high-level overview of the Blazor.RCL architecture.

## Architecture Style

The project follows a Clean Architecture approach with clear separation of concerns across multiple layers:

- **Domain Layer**: Core business logic and entities
- **Application Layer**: Application services, DTOs, and interfaces
- **Infrastructure Layer**: Implementation of interfaces, external service integrations
- **UI Components Layer**: Reusable Blazor components and layouts

## System Components

### Core Components

- **UIComponents**: Blazor components, pages, and layouts that provide reusable UI elements
- **Automation Services**: Implementation of core Automation functionality
  - Directory Services
  - Mail Services
  - Request Processing
  - Task Management
  - Batch Processing
- **Infrastructure**: Cross-cutting concerns
  - Logging (NLog)
  - Data Access (Entity Framework Core)
  - Identity Services
- **Domain**: Core business entities and logic
- **Application**: Service interfaces and DTOs

## Technology Stack

### Backend
- .NET 8.0
- Entity Framework Core 8.0
- NLog 5.3.2
- Microsoft Identity
- System.DirectoryServices

### Frontend
- Blazor
- MudBlazor 8.2.0
- MudBlazor.Extensions 8.0.1
- MudBlazor.ThemeManager 3.0.0

### Infrastructure
- SQL Server (via Entity Framework Core)
- Redis Cache (optional)
- LDAP Directory Services

## Component Relationships

The architecture follows a dependency inversion principle where all dependencies point inward. The domain layer is at the core and has no dependencies. The application layer depends on the domain layer. The infrastructure and UI components depend on the application layer.

## Data Flow

1. UI Components initiate requests via service interfaces
2. Application services process these requests
3. Infrastructure implementations handle external interactions
4. Data flows back up through the layers to the UI

## Key Design Decisions

1. Use of Blazor for reusable UI components
2. Implementation of MudBlazor for consistent UI design
3. Clean Architecture approach for separation of concerns
4. Use of Entity Framework Core for data access
5. Integration with multiple authentication methods

### Request Builder Pattern

A key architectural decision in the RCL project is the implementation of a generic request builder system. This approach provides:

- **Common interfaces and base functionality** in the RCL project
- **Specific request builder implementations** remain in client applications
- **Consistent pattern** across all Automation Tool applications
- **Loose coupling** between the utility library and client applications

The hybrid approach for request builders was chosen over fully centralized or decentralized approaches to balance:

1. **Standardization** of request building patterns
2. **Flexibility** for client applications to define their own specific request builders
3. **Code reuse** while maintaining loose coupling
4. **Type-safe request construction** across applications

## Non-Functional Requirements

- **Scalability**: Designed to be used across multiple applications
- **Performance**: Optimized for rapid UI rendering and efficient data access
- **Security**: Strong authentication and authorization support
- **Reliability**: Robust error handling and logging

## Future Considerations

- Enhanced monitoring and telemetry
- Expanded UI component library
- Additional authentication providers 