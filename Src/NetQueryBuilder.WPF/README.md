# NetQueryBuilder.WPF

WPF UI components for NetQueryBuilder - Build dynamic queries with a visual interface.

## Overview

NetQueryBuilder.WPF provides a comprehensive set of WPF controls for building and executing dynamic queries against various data sources. Built on the NetQueryBuilder core library, it offers a rich, professional UI with support for complex query conditions, property selection, and paginated result display.

## Features

- **QueryBuilderContainer**: Top-level control with entity selection and query lifecycle management
- **QueryBuilder**: Main query building interface with SELECT, WHERE, and results sections
- **Condition System**: Hierarchical condition editor supporting simple and grouped conditions
- **Type-aware Input**: Automatic input controls based on property types (string, number, date, boolean, etc.)
- **Results Display**: Paginated DataGrid with dynamic columns
- **Professional Theming**: Modern, Material Design-inspired styling
- **MVVM Architecture**: Clean separation of concerns with ViewModels
- **Multi-Framework Support**: Targets .NET 6.0, 8.0, and 9.0

## Installation

```bash
dotnet add package NetQueryBuilder.WPF
```

## Quick Start

### 1. Add Theme Resources

In your `App.xaml` or `Window` resources:

```xml
<Window.Resources>
    <ResourceDictionary>
        <ResourceDictionary.MergedDictionaries>
            <ResourceDictionary Source="pack://application:,,,/NetQueryBuilder.WPF;component/Themes/Generic.xaml"/>
        </ResourceDictionary.MergedDictionaries>
    </ResourceDictionary>
</Window.Resources>
```

### 2. Use QueryBuilderContainer

```xml
<Window xmlns:wpf="clr-namespace:NetQueryBuilder.WPF.Controls;assembly=NetQueryBuilder.WPF">
    <wpf:QueryBuilderContainer x:Name="QueryContainer"/>
</Window>
```

### 3. Initialize with Data

```csharp
using NetQueryBuilder.Configurations;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        // Create sample data
        var people = GetPeople();

        // Create configurator
        var configurator = new QueryableQueryConfigurator<Person>(people.AsQueryable());

        // Assign to container
        QueryContainer.Configurator = configurator;
    }
}
```

## Architecture

### Component Hierarchy

```
QueryBuilderContainer
├── Entity Selector (ComboBox)
├── QueryBuilder<T>
│   ├── SELECT Section (Property Checkboxes)
│   ├── WHERE Section (ConditionEditor)
│   ├── Expression Preview
│   └── Execute Button
└── QueryResultGrid (Paginated DataGrid)
```

### Key Components

#### QueryBuilderContainer
Top-level control that manages:
- Entity type selection
- Query instance lifecycle
- "New Query" functionality

```csharp
public class QueryBuilderContainer : UserControl
{
    public IQueryConfigurator? Configurator { get; set; }
}
```

#### QueryBuilder<T>
Generic query builder with three main sections:
- **SELECT**: Property selection with checkboxes
- **WHERE**: Hierarchical condition editor
- **Expression Preview**: Shows generated LINQ expression

```csharp
public class QueryBuilder<T> : UserControl
{
    public IQuery? Query { get; set; }
}
```

#### ConditionEditor
Recursive router that renders:
- `SimpleConditionEditor`: Field + Operator + Value
- `BlockConditionEditor`: Grouped conditions with AND/OR logic

#### QueryResultGrid
Displays paginated query results:
- Dynamic columns from selected properties
- Pagination controls (First, Previous, Next, Last)
- Total items count
- Auto-generated DataGrid

### ViewModels

All controls use MVVM pattern with dedicated ViewModels:

- `QueryBuilderContainerViewModel`: Entity selection, query creation
- `QueryBuilderViewModel`: Query configuration, execution, debounced expression updates
- `SimpleConditionViewModel`: Single condition management
- `BlockConditionViewModel`: Grouped condition management
- `QueryResultViewModel`: Pagination, result display

## Styling & Theming

### Color Scheme

The library uses a professional Material Design-inspired palette:

- **Primary**: #1976D2 (Blue)
- **Secondary**: #424242 (Dark Gray)
- **Success**: #4CAF50 (Green)
- **Error**: #F44336 (Red)
- **Background**: #FAFAFA (Light Gray)

### Button Styles

- `PrimaryButtonStyle`: Filled blue button for primary actions
- `SecondaryButtonStyle`: Outlined button for secondary actions
- `IconButtonStyle`: Transparent button for icons
- `PaginationButtonStyle`: Pagination-specific styling

### Custom Styling

Override default styles by redefining them after importing `Generic.xaml`:

```xml
<ResourceDictionary>
    <ResourceDictionary.MergedDictionaries>
        <ResourceDictionary Source="pack://application:,,,/NetQueryBuilder.WPF;component/Themes/Generic.xaml"/>
    </ResourceDictionary.MergedDictionaries>

    <!-- Your custom styles here -->
    <Style x:Key="PrimaryButtonStyle" TargetType="Button" BasedOn="{StaticResource {x:Type Button}}">
        <!-- Custom styling -->
    </Style>
</ResourceDictionary>
```

## Advanced Usage

### Standalone QueryBuilder

Use `QueryBuilder` directly without the container:

```xml
<wpf:QueryBuilder Query="{Binding MyQuery}"/>
```

```csharp
var configurator = new QueryableQueryConfigurator<Person>(people.AsQueryable());
var query = configurator.BuildFor<Person>();
MyQuery = query;
```

### Results-Only Display

Use `QueryResultGrid` independently:

```xml
<wpf:QueryResultGrid Results="{Binding QueryResults}"
                    DisplayProperties="{Binding SelectedProperties}"/>
```

### Custom Operators

Add custom operators through the core library:

```csharp
configurator.ConfigureConditions(config =>
{
    config.AddOperator(new MyCustomOperator());
});
```

## Dependencies

- **NetQueryBuilder**: Core query building library
- **Microsoft.Xaml.Behaviors.Wpf**: For WPF behaviors (1.1.122)
- **.NET 6.0/8.0/9.0-windows**: Multi-framework targeting

## Sample Application

A complete sample application is available at `Samples/NetQueryBuilder.WpfSample` demonstrating:
- Entity selection (Person/Product)
- Complex query building
- Condition grouping
- Result pagination
- All UI features

Run the sample:

```bash
dotnet run --project Samples/NetQueryBuilder.WpfSample/NetQueryBuilder.WpfSample.csproj
```

## Comparison with Blazor Version

| Feature | WPF | Blazor |
|---------|-----|--------|
| UI Framework | WPF (XAML + Code-behind) | Blazor (Razor Components) |
| Architecture | MVVM with ViewModels | Component-based |
| Styling | ResourceDictionaries, Styles | CSS, Custom Components |
| Data Binding | DependencyProperty, TwoWay | Parameter binding, EventCallback |
| Rendering | Native WPF controls | HTML + CSS (MudBlazor) |

Both implementations share the same core NetQueryBuilder library, ensuring feature parity and consistent behavior.

## Contributing

Contributions are welcome! Please follow the existing code style and architecture patterns.

## License

See the main repository LICENSE file for licensing information.
