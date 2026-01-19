# NetQueryBuilder.AspNetCore

NetQueryBuilder.AspNetCore provides Tag Helpers, View Components, and base page models for building dynamic queries in ASP.NET Core Razor Pages applications without requiring JavaScript.

## License

**Important:** NetQueryBuilder.AspNetCore is open-source under the MIT license for personal, educational, and non-commercial use.
For commercial use, a valid commercial license must be purchased from [https://huline.gumroad.com/l/netquerybuilder](https://huline.gumroad.com/l/netquerybuilder).

## Description

This package extends NetQueryBuilder with server-side ASP.NET Core components that allow users to visually construct complex queries through standard HTML forms. It uses Tag Helpers for rendering UI elements and View Components for displaying results, providing a traditional server-rendered experience without JavaScript dependencies.

For information about the core query building functionality, please refer to the [NetQueryBuilder Core documentation](../NetQueryBuilder/Readme.md). For Entity Framework integration, see the [NetQueryBuilder.EntityFramework documentation](../NetQueryBuilder.EntityFramework/Readme.md).

## Installation

```shell
dotnet add package NetQueryBuilder
dotnet add package NetQueryBuilder.EntityFramework  # If using EF Core
dotnet add package NetQueryBuilder.AspNetCore
```

## Setup

### 1. Register Services

In your `Program.cs`:

```csharp
builder.Services.AddRazorPages();
builder.Services.AddDbContext<YourDbContext>(options =>
    options.UseSqlServer(connectionString));

// Single line registers all NetQueryBuilder services
builder.Services.AddNetQueryBuilder<YourDbContext>(options =>
{
    options.SessionTimeout = TimeSpan.FromMinutes(30);
    options.DefaultPageSize = 10;
});
```

### 2. Configure Middleware

```csharp
app.UseRouting();
app.UseNetQueryBuilder();  // Handles session + static files
app.MapRazorPages();
```

### 3. Add Tag Helpers

In your `_ViewImports.cshtml`:

```cshtml
@addTagHelper *, NetQueryBuilder.AspNetCore
```

### 4. Include Stylesheet

In your layout or page:

```html
<link rel="stylesheet" href="~/_content/NetQueryBuilder.AspNetCore/css/netquerybuilder.css" />
```

## Usage

### Basic Query Builder Page

Create a Razor Page that inherits from `NetQueryPageModelBase`:

**QueryBuilder.cshtml.cs:**
```csharp
public class QueryBuilderModel : NetQueryPageModelBase
{
    public QueryBuilderModel(
        IQuerySessionService sessionService,
        IQueryConfigurator configurator)
        : base(sessionService, configurator) { }

    public void OnGet() { }

    // Base class automatically handles OnPostExecuteQueryAsync
    // using reflection to dispatch to the correct entity type
}
```

**QueryBuilder.cshtml:**
```cshtml
@page
@model QueryBuilderModel

<h1>Query Builder</h1>

<form method="post">
    <nqb-entity-selector session-id="@Model.SessionId"></nqb-entity-selector>
    <nqb-property-selector session-id="@Model.SessionId"></nqb-property-selector>
    <nqb-condition-builder session-id="@Model.SessionId"></nqb-condition-builder>

    @await Component.InvokeAsync("ExpressionDisplay", new { sessionId = Model.SessionId })

    <button type="submit" asp-page-handler="ExecuteQuery" class="nqb-button nqb-button-primary">
        Run Query
    </button>
</form>

@if (Model.State.Results != null)
{
    @await Component.InvokeAsync("QueryResults", new { sessionId = Model.SessionId })
    @await Component.InvokeAsync("Pagination", new { sessionId = Model.SessionId })
}
```

## Tag Helpers

### Entity Selector

Renders a dropdown to select the entity type to query:

```cshtml
<nqb-entity-selector session-id="@Model.SessionId"></nqb-entity-selector>
```

### Property Selector

Renders checkboxes for selecting which properties to display:

```cshtml
<nqb-property-selector session-id="@Model.SessionId"></nqb-property-selector>
```

### Condition Builder

Renders the condition builder UI with operators and values:

```cshtml
<nqb-condition-builder session-id="@Model.SessionId"></nqb-condition-builder>
```

## View Components

### QueryResults

Displays query results in a table:

```cshtml
@await Component.InvokeAsync("QueryResults", new { sessionId = Model.SessionId })
```

### Pagination

Renders pagination controls:

```cshtml
@await Component.InvokeAsync("Pagination", new { sessionId = Model.SessionId })
```

### ExpressionDisplay

Shows the generated query expression:

```cshtml
@await Component.InvokeAsync("ExpressionDisplay", new { sessionId = Model.SessionId })
```

## Session Management

The `QuerySessionService` manages query state between HTTP requests:

```csharp
// Get current session state
var state = await _sessionService.GetSessionAsync(sessionId);

// Update session state
state.SelectedEntityType = typeof(Product);
await _sessionService.SaveSessionAsync(sessionId, state);
```

Sessions are automatically cleaned up after the configured timeout by a background service.

## Custom Page Handlers

You can add custom handlers alongside the base functionality:

```csharp
public class QueryBuilderModel : NetQueryPageModelBase
{
    public QueryBuilderModel(
        IQuerySessionService sessionService,
        IQueryConfigurator configurator)
        : base(sessionService, configurator) { }

    public void OnGet() { }

    // Custom handler for exporting results
    public async Task<IActionResult> OnPostExportAsync()
    {
        var state = await SessionService.GetSessionAsync(SessionId);

        if (state.Results == null)
            return RedirectToPage();

        var csv = GenerateCsv(state.Results);
        return File(Encoding.UTF8.GetBytes(csv), "text/csv", "export.csv");
    }
}
```

## Styling

The included CSS provides a clean, accessible design. You can customize it by:

1. Overriding CSS variables:

```css
:root {
    --nqb-primary-color: #1E88E5;
    --nqb-border-radius: 4px;
    --nqb-spacing: 1rem;
}
```

2. Adding your own styles after the default stylesheet:

```html
<link rel="stylesheet" href="~/_content/NetQueryBuilder.AspNetCore/css/netquerybuilder.css" />
<link rel="stylesheet" href="~/css/custom-query-builder.css" />
```

## Accessibility

All form controls include ARIA attributes for accessibility:

- Labels are properly associated with inputs
- Error messages use `aria-describedby`
- Focus management follows WAI-ARIA best practices
- Keyboard navigation is fully supported

## Performance Considerations

- Sessions are stored in memory by default; consider distributed cache for multi-server deployments
- Large result sets should use pagination (configured via `DefaultPageSize`)
- Background service cleans up expired sessions automatically

## Contributing

Contributions are welcome! You can contribute by improving the Tag Helpers, View Components, or adding new features to the ASP.NET Core integration.
