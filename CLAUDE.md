# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

NetQueryBuilder is a comprehensive .NET library ecosystem for building and executing dynamic queries across different data sources with extensible UI components. It provides a type-safe, expression-tree based query building system with multi-framework support (.NET 6.0-9.0, .NET Framework 4.8) and rich UI components for Blazor and WPF applications.

## Architecture

### Core Query System

The architecture centers around expression tree-based query building with these key interfaces:

- **`IQuery`**: Central contract defining query execution with conditions, selections, and data source operations
- **`IQueryConfigurator`**: Factory pattern for building strongly-typed queries with entity-specific configurations
- **`IQueryExecutor<T>`**: Executes queries against data sources, returning `IQueryable<T>` for further LINQ operations
- **Condition System**: Hierarchical structure with `BlockCondition` (logical grouping) and `SimpleCondition` (atomic conditions)

### Property Navigation System

- **`PropertyPath`**: Navigates complex object graphs using dot notation ("Customer.Address.City")
- **`IPropertyConfigurator`**: Configures property metadata including display names, descriptions, and validation
- Type-safe reflection-based resolution with caching for performance

### Operator Framework

Extensible operator system with expression building:
- Built-in operators: `Equals`, `NotEquals`, `Contains`, `Like`, `GreaterThan`, `LessThan`, `Between`, `In`, `IsNull`
- **`IOperatorFactory`**: Creates custom domain-specific operators
- Each operator implements `IOperator` with `BuildExpression()` and `Stringify()` methods

### UI Component Architecture

#### Blazor Components (MudBlazor-based):
- **`QueryBuilderContainer`**: Root component managing entity selection and query lifecycle
- **`QueryBuilder<T>`**: Generic query builder with type-specific property configuration
- **`QueryResultTable`**: Dynamic result display with pagination (`QueryResult<T>` wrapper)
- Form controls: `TextField`, `Select`, `DatePicker`, `NumberField` with two-way binding

#### Event System:
- `OnConditionChanged`: Fires when query conditions are modified
- `OnQueryExecuted`: Post-execution event with result metadata
- Parent-child communication through cascading parameters

#### ASP.NET Core Razor Pages Components:
- **Tag Helpers**: `EntitySelectorTagHelper`, `PropertySelectorTagHelper`, `ConditionBuilderTagHelper` for rendering query UI
- **View Components**: `QueryResultsViewComponent`, `PaginationViewComponent`, `ExpressionDisplayViewComponent` for async rendering
- **`NetQueryPageModelBase`**: Reusable base class for Razor Pages with common handlers
- **`QuerySessionService`**: Session-based state management between postbacks
- **Form-based interaction**: Standard HTTP POST with page handlers (no JavaScript required)
- **Shared CSS**: Reuses `netquerybuilder.css` from Blazor for consistent styling

## Project Structure

```
NetQueryBuilder/
├── Src/
│   ├── NetQueryBuilder/                     # Core library
│   │   ├── Conditions/                      # Condition types and builders
│   │   ├── Configurations/                  # Query and property configurators
│   │   ├── Operators/                       # Operator implementations
│   │   ├── Properties/                      # Property path and metadata
│   │   ├── Queries/                         # Query interfaces and base classes
│   │   └── UI/                              # UI abstraction layer
│   ├── NetQueryBuilder.EntityFramework/     # EF Core 6.0+ integration
│   ├── NetQueryBuilder.EntityFrameworkNet4/ # .NET Framework 4.8 EF6 support
│   ├── NetQueryBuilder.Blazor/              # Blazor components
│   ├── NetQueryBuilder.AspNetCore/          # ASP.NET Core Razor Pages integration
│   │   ├── Services/                        # Session and state management
│   │   ├── TagHelpers/                      # UI rendering tag helpers
│   │   ├── ViewComponents/                  # Async data display components
│   │   ├── Pages/                           # Base page models
│   │   ├── Models/                          # View models and state
│   │   └── Views/                           # View component views
│   └── NetQueryBuilder.WPF/                 # WPF controls
├── Tests/
│   ├── NetQueryBuilder.Tests/               # Core unit tests
│   ├── NetQueryBuilder.EntityFramework.Tests/
│   ├── NetQueryBuilder.EntityFrameworkNet.Tests/
│   └── NetQueryBuilder.Blazor.Tests/        # bunit component tests
└── Samples/
    ├── NetQueryBuilder.BlazorSampleApp/     # Blazor reference implementation
    └── NetQueryBuilder.AspNetCoreSampleApp/ # Razor Pages reference implementation
```

## Development Commands

### Building
```bash
# Build entire solution
dotnet build NetQueryBuilder.sln

# Build with specific configuration
dotnet build NetQueryBuilder.sln --configuration Release

# Build individual projects
dotnet build Src/NetQueryBuilder/NetQueryBuilder.csproj
dotnet build Src/NetQueryBuilder.Blazor/NetQueryBuilder.Blazor.csproj
```

### Testing
```bash
# Run all tests with coverage
dotnet test NetQueryBuilder.sln --collect:"XPlat Code Coverage"

# Run specific test project
dotnet test Tests/NetQueryBuilder.Tests/NetQueryBuilder.Tests.csproj

# Run tests with filter
dotnet test --filter "FullyQualifiedName~NetQueryBuilder.Tests.Conditions"

# Run Blazor component tests
dotnet test Tests/NetQueryBuilder.Blazor.Tests/NetQueryBuilder.Blazor.Tests.csproj
```

### Running Sample Applications
```bash
# Run Blazor sample (launches on https://localhost:5001)
dotnet run --project Samples/NetQueryBuilder.BlazorSampleApp/NetQueryBuilder.BlazorSampleApp.csproj

# Run ASP.NET Core Razor Pages sample
dotnet run --project Samples/NetQueryBuilder.AspNetCoreSampleApp/NetQueryBuilder.AspNetCoreSampleApp.csproj

# Run with hot reload
dotnet watch run --project Samples/NetQueryBuilder.BlazorSampleApp/NetQueryBuilder.BlazorSampleApp.csproj
dotnet watch run --project Samples/NetQueryBuilder.AspNetCoreSampleApp/NetQueryBuilder.AspNetCoreSampleApp.csproj
```

### Package Management
```bash
# Restore dependencies
dotnet restore NetQueryBuilder.sln

# Create NuGet packages
dotnet pack NetQueryBuilder.sln --configuration Release --output ./artifacts

# Pack individual project
dotnet pack Src/NetQueryBuilder/NetQueryBuilder.csproj --configuration Release
```

## Target Frameworks and Dependencies

| Project | Target Frameworks | Key Dependencies |
|---------|------------------|------------------|
| NetQueryBuilder (Core) | net6.0, net8.0, net9.0, netstandard2.1, net48 | Microsoft.CodeAnalysis.CSharp.Scripting (4.12.0), System.Linq.Dynamic.Core (1.6.0.2) |
| NetQueryBuilder.Blazor | net6.0, net8.0, net9.0 | MudBlazor (6.21.0), Microsoft.AspNetCore.Components |
| NetQueryBuilder.AspNetCore | net6.0, net8.0, net9.0 | Microsoft.AspNetCore.App, Microsoft.EntityFrameworkCore |
| NetQueryBuilder.EntityFramework | net6.0, net8.0, net9.0 | Microsoft.EntityFrameworkCore (6.0.36) |
| NetQueryBuilder.EntityFrameworkNet4 | net48 | EntityFramework (6.5.1) |
| Tests | net9.0 | xUnit (2.9.2), bunit (1.38.5), coverlet.collector (6.0.2) |

## Key Implementation Patterns

### Query Building Pattern
```csharp
// Configure and build query
var configurator = services.GetService<IQueryConfigurator>();
var query = configurator.BuildFor<Customer>();

// Add conditions using fluent API
query.Condition.CreateNew<EqualsOperator>("Status", "Active")
    .And.CreateNew<GreaterThanOperator>("OrderCount", 5);

// Execute with data source
var results = await executor.ExecuteAsync(query);
```

### Property Configuration
```csharp
// Configure properties with metadata
configurator.ConfigureProperty<Customer>(c => c.Name)
    .WithDisplayName("Customer Name")
    .WithDescription("Full name of the customer")
    .Required();
```

### Custom Operator Creation
```csharp
public class CustomOperator : IOperator
{
    public Expression BuildExpression(MemberExpression property, ConstantExpression value)
    {
        // Build custom expression tree
    }
    
    public string Stringify(string property, object value)
    {
        // Return human-readable format
    }
}
```

### Blazor Component Integration
```razor
@inject IQueryConfigurator Configurator

<QueryBuilderContainer Configurator="@Configurator">
    <QueryBuilder T="Customer" OnQueryExecuted="@HandleResults" />
</QueryBuilderContainer>
```

### ASP.NET Core Razor Pages Integration
```csharp
// Program.cs
builder.Services.AddRazorPages();
builder.Services.AddNetQueryBuilder(options =>
{
    options.SessionTimeout = TimeSpan.FromMinutes(30);
    options.DefaultPageSize = 10;
});
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("Demo"));
builder.Services.AddScoped<IQueryConfigurator, EfQueryConfigurator<AppDbContext>>();

app.UseSession(); // Required for state management
app.MapRazorPages();

// QueryBuilder.cshtml
@page
@model QueryBuilderModel
<form method="post">
    <nqb-entity-selector session-id="@Model.SessionId"></nqb-entity-selector>
    <nqb-property-selector session-id="@Model.SessionId"></nqb-property-selector>
    <nqb-condition-builder session-id="@Model.SessionId"></nqb-condition-builder>

    @await Component.InvokeAsync("ExpressionDisplay", new { sessionId = Model.SessionId })

    <button type="submit" formaction="?handler=ExecuteQuery">Run Query</button>
</form>

@if (Model.State.Results != null)
{
    @await Component.InvokeAsync("QueryResults", new { sessionId = Model.SessionId })
    @await Component.InvokeAsync("Pagination", new { sessionId = Model.SessionId })
}

// QueryBuilder.cshtml.cs
public class QueryBuilderModel : NetQueryPageModelBase
{
    public QueryBuilderModel(IQuerySessionService sessionService, IQueryConfigurator configurator)
        : base(sessionService, configurator) { }

    public override async Task<IActionResult> OnPostExecuteQueryAsync()
    {
        await ExecuteQueryAsync<Customer>();
        return Page();
    }
}
```

## CI/CD Pipeline (GitHub Actions)

The `deploy.yaml` workflow provides:
1. Multi-framework builds (.NET 6.0, 8.0, 9.0)
2. Full test suite execution with coverage
3. Qodana code quality analysis
4. Automated NuGet package creation and publishing on version tags
5. Artifact preservation for 90 days

## Development Guidelines

- **Nullable Reference Types**: Enabled in Blazor projects (`<Nullable>enable</Nullable>`), disabled in core for compatibility
- **Code Organization**: Blazor components use code-behind pattern (`.razor` + `.razor.cs`)
- **Dependency Injection**: All services designed for DI container registration
- **Expression Trees**: Core query logic uses `System.Linq.Expressions` for type safety
- **Async/Await**: Query execution supports async operations throughout
- **Event-Driven Updates**: UI components use events for real-time query updates

## Licensing

Dual licensing model:
- **MIT License**: Personal, educational, and non-commercial use
- **Commercial License**: Required for commercial applications (via Gumroad)