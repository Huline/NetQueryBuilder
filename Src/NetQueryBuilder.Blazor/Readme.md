# NetQueryBuilder.Blazor

Blazor UI components for building dynamic queries in Blazor applications, integrating seamlessly with the NetQueryBuilder ecosystem.

## License

**Important:** NetQueryBuilder.Blazor is open-source under the MIT license for personal, educational, and non-commercial use.
For commercial use, a valid commercial license must be purchased from [https://huline.gumroad.com/l/netquerybuilder](https://huline.gumroad.com/l/netquerybuilder).

## Description

This package extends NetQueryBuilder with standalone Blazor UI components that allow users to visually construct complex queries through an intuitive interface. The components are framework-agnostic and work with any Blazor application - no additional UI framework is required.

For information about the core query building functionality, please refer to the [NetQueryBuilder Core documentation](../NetQueryBuilder/Readme.md). For Entity Framework integration, see the [NetQueryBuilder.EntityFramework documentation](../NetQueryBuilder.EntityFramework/Readme.md).

## Installation

```shell
dotnet add package NetQueryBuilder
dotnet add package NetQueryBuilder.EntityFramework  # If using EF Core
dotnet add package NetQueryBuilder.Blazor
```

## Setup

### 1. Register Services

In your Blazor application's `Program.cs`:

```csharp
// Register NetQueryBuilder services
builder.Services.AddDbContext<YourDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped<IQueryConfigurator, EfQueryConfigurator<YourDbContext>>();
```

### 2. Add Imports

In your `_Imports.razor`:

```cshtml
@using NetQueryBuilder.Blazor.Components
```

### 3. Include Stylesheet (Optional)

The components include basic styling. Include the CSS in your `index.html` or `App.razor`:

```html
<link href="_content/NetQueryBuilder.Blazor/css/netquerybuilder.css" rel="stylesheet" />
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

<button class="btn btn-primary" @onclick="ExecuteQuery">Execute</button>

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

The components use standard HTML elements with CSS classes that you can override:

```css
/* Override default styles */
.nqb-query-builder {
    /* Your custom styles */
}

.nqb-condition {
    /* Your custom styles */
}

.nqb-results-table {
    /* Your custom styles */
}
```

### Integration with UI Frameworks

The components work with any UI framework. For example, with MudBlazor (as shown in the sample app):

```cshtml
@* Wrap components in MudBlazor cards if desired *@
<MudCard>
    <MudCardContent>
        <QueryBuilder TEntity="Product" />
    </MudCardContent>
</MudCard>
```

Or with Bootstrap:

```cshtml
<div class="card">
    <div class="card-body">
        <QueryBuilder TEntity="Product" />
    </div>
</div>
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

## Performance Considerations

When working with large data sets, consider:

1. Implement paging for query results
2. Add debounce logic for automatic query execution
3. Use server-side filtering for initial data sets

## Sample Application

The [NetQueryBuilder.BlazorSampleApp](../../Samples/NetQueryBuilder.BlazorSampleApp) demonstrates a complete implementation using MudBlazor for enhanced styling. Note that MudBlazor is used only in the sample - it's not a requirement for the library.

## Contributing

Contributions are welcome! You can contribute by improving the UI components or by adding new features to the Blazor integration.
