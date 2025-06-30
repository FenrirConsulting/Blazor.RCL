# Component-Based Architecture Implementation

## Overview

This document outlines the component-based architecture implementation approach used in the RCL project and related applications. This architecture replaced the previous monolithic design to improve maintainability, testability, and separation of concerns.

## Core Architecture Components

### 1. Service Layer

The service layer implements business logic and manages application state:

- **State Management Services**
  - Example: `EnableDisableStateService` in the EnableDisable project 
  - Purpose: Centrally manages application state and state transitions
  - Pattern: Observable state with change notifications

- **API Interaction Services**
  - Example: `AutomationApiService` 
  - Purpose: Provides a clean abstraction for backend API communication
  - Pattern: Repository pattern with async/await operations

- **Request Building Services**
  - Generic interfaces and base classes in RCL
  - Specific implementations in client applications
  - Purpose: Constructs properly formatted API requests
  - Pattern: Builder pattern with fluent interfaces

- **Response Processing Services**
  - Example: `ResponseProcessingService`
  - Purpose: Standardizes handling of API responses
  - Pattern: Strategy pattern for different response types

### 2. UI Component Layer

The UI layer is built as a collection of reusable components:

- **Container Components**
  - Purpose: Manage state and coordinate child components
  - Pattern: Container/Presentational pattern

- **Presentational Components**
  - Purpose: Render UI based on inputs and emit events
  - Pattern: Prop-driven rendering with event callbacks

- **Form Components**
  - Purpose: Handle user input and validation
  - Pattern: Controlled components with validation rules

- **Layout Components**
  - Purpose: Structure the UI and manage responsive behavior
  - Pattern: Composition pattern with content projection

### 3. Dependency Injection

All components are registered and resolved through dependency injection:

- Services registered in `DependencyInjection.cs`
- Scoped vs. Singleton lifetimes based on state requirements
- Interface-based design for testability and loose coupling

## Implementation Guidelines

### Service Implementation

1. Define an interface for each service in the application layer
2. Implement the service with appropriate state management
3. Register the service in the DI container
4. Inject the service where needed

Example:
```csharp
public interface IExampleService
{
    Task<r> ProcessItemAsync(Item item);
}

public class ExampleService : IExampleService
{
    private readonly IApiClient _apiClient;
    
    public ExampleService(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }
    
    public async Task<r> ProcessItemAsync(Item item)
    {
        // Implementation
    }
}
```

### UI Component Implementation

1. Create a Razor component with a backing C# class
2. Define parameters, callbacks, and local state
3. Implement rendering logic with event handling
4. Use cascading parameters for theme/global state

Example:
```csharp
@code {
    [Parameter] public Item Item { get; set; }
    [Parameter] public EventCallback<Item> OnItemChanged { get; set; }
    
    private async Task HandleChange(ChangeEventArgs e)
    {
        Item = Item with { Name = e.Value.ToString() };
        await OnItemChanged.InvokeAsync(Item);
    }
}
```

## Migration Approach

When refactoring from a monolithic design to this component-based architecture:

1. Identify and extract service boundaries
2. Convert UI sections to standalone components
3. Implement state management services
4. Replace direct API calls with service calls
5. Update dependency injection registration

## Benefits Realized

The RCL project and applications that use it have benefited from this architecture in several ways:

- **Improved maintainability** through smaller, focused components
- **Enhanced testability** with clear separation of concerns
- **Better reusability** of both UI components and services
- **Clearer responsibility boundaries** between different parts of the application
- **More consistent patterns** across different applications

## Related Documentation

- Request Builder Pattern details are now in the Architecture Overview document
- UI Components information is available in the UI Components Reference Guide
- See the main Architecture Overview document for the complete architectural picture
