# Blazor.RCL Overview

## Project Summary
A utility library for Company Health Identity Automation .NET Core web applications. This library provides core functionality and components that can be reused across different web applications in the Identity and Access Management (Identity) and Identity and Access Provisioning Framework (Automation) ecosystem.

## Core Components
- **UIComponents**: Reusable Blazor UI components, layouts, and pages
- **Automation Services**: Core services for Automation functionality (Directory, Mail, Requests, Tasks, Batch processing)
- **Infrastructure**: Cross-cutting concerns including authentication, logging, data access, background services, and AKeyless integration
- **Domain**: Core business logic, entities, and value objects
- **Application**: Application services, interfaces, and DTOs

## Technology Stack
- **.NET 8.0**: Core framework for the library
- **Blazor**: Web UI framework
- **Entity Framework Core**: Data access and ORM
- **MudBlazor**: UI component library
- **NLog**: Logging framework
- **Microsoft Identity**: Authentication and authorization
- **Microsoft Graph**: Integration with Microsoft services
- **LDAP**: Directory services integration
- **AKeyless**: Secure secret management
- **RabbitMQ**: Message queue for async operations

## Metadata Structure
```
/Metadata/
├── README.md                  # Project overview + structure explanation
├── CurrentStatus.md           # Consolidated project status, goals, and implementation schedule
├── SessionIndex.md            # Chronological record of LLM sessions
└── Knowledge/                 # Simplified knowledge base
    └── ProjectStructure.md    # Comprehensive file structure documentation
```

## Using This Metadata

When working with Claude or other LLMs:

1. Always include the core context files for a complete understanding of the project:
   - README.md - Project summary and objectives
   - CurrentStatus.md - Current status and focus areas
   - SessionIndex.md - Conversation history and threads

2. Add Knowledge files when working on specific aspects of the project:
   - ProjectStructure.md - Comprehensive documentation of all project files and directories

This metadata system is designed to maintain context across LLM conversations while focusing on essential elements for effective project tracking and documentation.
