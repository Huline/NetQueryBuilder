# NetQueryBuilder.Blazor

NetQueryBuilder.Blazor provides a comprehensive UI component suite for building dynamic queries in Blazor applications, integrating seamlessly with the NetQueryBuilder ecosystem.

## License

**Important:** NetQueryBuilder.Blazor is open-source under the MIT license for personal, educational, and non-commercial use.
For commercial use, a valid commercial license must be purchased from [https://huline.gumroad.com/l/netquerybuilder](https://huline.gumroad.com/l/netquerybuilder).

## Description

This package extends NetQueryBuilder with Blazor UI components that allow users to visually construct complex queries through an intuitive interface. It's built on top of MudBlazor to provide a rich, responsive user experience while
leveraging the powerful query building capabilities of NetQueryBuilder.

For information about the core query building functionality, please refer to the [NetQueryBuilder Core documentation](../NetQueryBuilder/Readme.md). For Entity Framework integration, see
the [NetQueryBuilder.EntityFramework documentation](../NetQueryBuilder.EntityFramework/Readme.md).

## Installation

```shell script
dotnet add package NetQueryBuilder
dotnet add package NetQueryBuilder.EntityFramework  # If using EF Core
dotnet add package NetQueryBuilder.Blazor
dotnet add package MudBlazor  # Required dependency
```

## Setup

### 1. Register Services

In your Blazor application's `Program.cs` or `Startup.cs`:

```csharp
// Register MudBlazor services
builder.Services.AddMudServices();

// Register NetQueryBuilder services
builder.Services.AddDbContext<YourDbContext>(options =>
    options.UseSqlServer(connectionString));
    
builder.Services.AddScoped<IQueryConfigurator, EfQueryConfigurator<YourDbContext>>();
```

### 2. Add MudBlazor Theme

In your `_Imports.razor`:

```cshtml
@using MudBlazor
@using NetQueryBuilder.Blazor.Components
```

In your `App.razor` or layout file:

```cshtml
<MudThemeProvider/>
<MudDialogProvider/>
<MudSnackbarProvider/>
```

Include MudBlazor CSS in your `index.html` or `_Host.cshtml`:

```html
<link href="_content/MudBlazor/MudBlazor.min.css" rel="stylesheet" />
<script src="_content/MudBlazor/MudBlazor.min.js"></script>
```

## Usage

### Basic Query Builder

The main component is `QueryBuilder<TEntity>`, which provides a complete interface for building and executing queries:

```cshtml
@page "/products-query"
@using NetQueryBuilder.Blazor.Components
@using YourNamespace.Models
@inject IQueryConfigurator QueryConfigurator

<h3>Product Query Builder</h3>

<QueryBuilder TEntity="Product" />
```

This will render a full query builder with:

- SELECT section for choosing fields to display
- WHERE section for building conditions
- Run button to execute the query
- Results table displaying the query output

### Handling Query Results

You can capture and process the query results:

```cshtml
<QueryBuilder TEntity="Product" OnQueryExecuted="HandleQueryResults" />

@code {
    private IEnumerable<Product> _queryResults;
    
    private void HandleQueryResults(IEnumerable<object> results)
    {
        _queryResults = results.Cast<Product>();
        // Do something with the results
    }
}
```

### Using the Expression Output

If you want to use the generated expression in other parts of your application:

```cshtml
<QueryBuilder TEntity="Product" 
              Expression="@_currentExpression" 
              ExpressionChanged="@HandleExpressionChanged" />

@code {
    private string _currentExpression;
    
    private void HandleExpressionChanged(string expression)
    {
        _currentExpression = expression;
        // You can save this expression or use it elsewhere
    }
}
```

## Component Breakdown

You can use individual components instead of the full QueryBuilder for more granular control:

### Condition Builder

```cshtml
@inject IQueryConfigurator QueryConfigurator

@code {
    private IQuery _query;
    
    protected override void OnInitialized()
    {
        _query = QueryConfigurator.BuildFor<Product>();
    }
}

<CascadingValue Value="@_query">
    <ConditionComponent Condition="@_query.Condition" />
</CascadingValue>

<MudButton Color="Color.Primary" OnClick="ExecuteQuery">Execute</MudButton>

@code {
    private async Task ExecuteQuery()
    {
        var results = await _query.Execute(50);
        // Process results
    }
}
```

### Property Selector

```cshtml
<PropertySelector TEntity="Product" 
                 SelectedProperties="@_selectedProperties"
                 SelectedPropertiesChanged="@HandleSelectedPropertiesChanged" />

@code {
    private List<SelectPropertyPath> _selectedProperties = new();
    
    private void HandleSelectedPropertiesChanged(List<SelectPropertyPath> properties)
    {
        _selectedProperties = properties;
        // Update your query or UI
    }
}
```

### Results Table

```cshtml
<QueryResultTable TEntity="Product" 
                 Data="@_queryResults" 
                 Properties="@_selectedProperties" />
```

## Customization

### Custom Styling

The QueryBuilder uses MudBlazor components and can be customized using MudBlazor's theming system:

```cshtml
<MudThemeProvider Theme="@_customTheme" />

@code {
    private MudTheme _customTheme = new()
    {
        Palette = new PaletteLight
        {
            Primary = "#1E88E5",
            Secondary = "#FF4081",
            // Other color customizations
        }
    };
}
```

### Custom Operator Components

You can create custom operator components for specialized filtering needs:

1. Create a custom operator in your NetQueryBuilder core implementation
2. Register the operator with your QueryConfigurator
3. Create a corresponding UI component in Blazor

```csharp
// Register custom operator
services.AddScoped<IQueryConfigurator>(provider => 
{
    var configurator = new EfQueryConfigurator<YourDbContext>(
        provider.GetRequiredService<YourDbContext>());
    
    configurator.ConfigureConditions(config => 
    {
        config.RegisterOperator<YourCustomOperator>();
    });
    
    return configurator;
});
```

### Embedding in Existing Forms

You can integrate the QueryBuilder into your existing forms:

```cshtml
<EditForm Model="@_formModel" OnValidSubmit="HandleValidSubmit">
    <DataAnnotationsValidator />
    
    <MudCard>
        <MudCardContent>
            <MudTextField @bind-Value="_formModel.Name" Label="Report Name" />
            
            <MudDivider Class="my-4" />
            
            <MudText Typo="Typo.h6">Query Criteria</MudText>
            <QueryBuilder TEntity="Product" 
                         Expression="@_formModel.QueryExpression" 
                         ExpressionChanged="@(expr => _formModel.QueryExpression = expr)" />
        </MudCardContent>
        
        <MudCardActions>
            <MudButton ButtonType="ButtonType.Submit" 
                      Color="Color.Primary" 
                      Variant="Variant.Filled">Save Report</MudButton>
        </MudCardActions>
    </MudCard>
</EditForm>
```

## Performance Considerations

When working with large data sets, consider:

1. Implement paging for query results
2. Add debounce logic for automatic query execution
3. Use server-side filtering for initial data sets

## Contributing

Contributions are welcome! You can contribute by improving the UI components or by adding new features to the Blazor integration.