# UI Components Reference Guide

This document provides a quick reference guide for the UI components in the Blazor.RCL library.

## Component Categories

The UI components are organized into the following categories:

1. **Pages**: Complete page templates
2. **Layouts**: Page layout components
3. **Components**: Individual UI elements

## Common Components

### Navigation Components

- **MainNav**: Primary navigation component
  - Usage: `<MainNav Items="@navItems" />`
  - Properties:
    - `Items`: List of navigation items
    - `OnItemSelected`: Event triggered when an item is selected

### Data Display Components

- **DataTable**: Displays tabular data with sorting, filtering, and pagination
  - Usage: `<DataTable Data="@items" Columns="@columns" />`
  - Properties:
    - `Data`: The data to display
    - `Columns`: Column definitions
    - `Pageable`: Enable pagination
    - `Sortable`: Enable sorting
    - `Filterable`: Enable filtering

### Form Components

- **FormField**: Standard form field with label and validation
  - Usage: `<FormField Label="Username" @bind-Value="@username" />`
  - Properties:
    - `Label`: Field label
    - `Value`: Bound value
    - `Required`: Whether the field is required
    - `Validation`: Validation rules

### Authentication Components

- **LoginForm**: Standard login form
  - Usage: `<LoginForm OnLogin="@HandleLogin" />`
  - Properties:
    - `OnLogin`: Event triggered on successful login
    - `ErrorMessage`: Error message to display

## Page Templates

- **StandardPage**: Base page template with header, footer, and navigation
  - Usage: `<StandardPage Title="My Page">Content</StandardPage>`
  - Properties:
    - `Title`: Page title
    - `ShowNav`: Whether to show navigation

## Layout Templates

- **MainLayout**: Primary layout for applications
  - Usage: `@layout MainLayout`
  - Features:
    - Header with app title
    - Side navigation
    - Main content area
    - Footer

## Usage Examples

### Basic Page with Table

```razor
@page "/users"
@layout MainLayout

<StandardPage Title="Users">
    <DataTable Data="@users" Columns="@columns" Pageable="true" Sortable="true" />
</StandardPage>

@code {
    private List<User> users;
    private List<ColumnDefinition> columns;

    protected override async Task OnInitializedAsync()
    {
        users = await UserService.GetUsersAsync();
        columns = new List<ColumnDefinition>
        {
            new ColumnDefinition { Field = "Username", Title = "Username" },
            new ColumnDefinition { Field = "Email", Title = "Email" },
            new ColumnDefinition { Field = "Role", Title = "Role" }
        };
    }
}
```

### Login Form Implementation

```razor
@page "/login"
@layout AuthLayout

<div class="login-container">
    <LoginForm OnLogin="@HandleLogin" ErrorMessage="@errorMessage" />
</div>

@code {
    private string errorMessage;

    private async Task HandleLogin(LoginModel model)
    {
        try
        {
            await AuthService.LoginAsync(model.Username, model.Password);
            NavigationManager.NavigateTo("/");
        }
        catch (Exception ex)
        {
            errorMessage = ex.Message;
        }
    }
}
```

## Customization

Most components support customization through:

1. **CSS Classes**: Pass custom CSS classes to override styling
2. **Templates**: Provide custom templates for content rendering
3. **Themes**: MudBlazor theme customization 