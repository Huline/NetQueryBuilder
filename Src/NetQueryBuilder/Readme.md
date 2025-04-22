# NetQueryBuilder

NetQueryBuilder is a flexible .NET library for building dynamic IQueryable-based queries, providing a solid foundation for various data source implementations.

## License

**Important:** Net Query Builder is open-source under the MIT license for personal, educational, and non-commercial use.
For commercial use, a valid commercial license must be purchased from [Gumroad link].

## Description

The core package of NetQueryBuilder provides an extensible architecture for building dynamic queries in .NET. Designed around the IQueryable interface, it enables the creation of typed query conditions and complex expressions without
depending on any specific ORM.

While this package can be used independently, it's primarily designed as a foundation for custom implementations or extension packages like NetQueryBuilder.EntityFramework.

## Key Features

- IQueryable-based query construction, ORM-independent
- Extensible architecture allowing advanced customization
- Configurable operator system (Equals, Like, GreaterThan, etc.)
- Dynamic condition expressions with support for complex property paths
- Clear interfaces for extension and customization

## Installation

```shell script
dotnet add package NetQueryBuilder
```

## Core Components

### IQueryConfigurator

The `IQueryConfigurator` interface is the main entry point for configuring and building queries:

```csharp
public interface IQueryConfigurator
{
    IEnumerable<Type> GetEntities();
    IQueryConfigurator UseExpressionStringifier(IExpressionStringifier expressionStringifier);
    IQueryConfigurator ConfigureSelect(Action<ISelectConfigurator> selectBuilder);
    IQueryConfigurator ConfigureConditions(Action<IConditionConfigurator> selectBuilder);
    IQuery BuildFor<T>() where T : class;
    IQuery BuildFor(Type type);
}
```

### Basic Query Creation

```csharp
// Create a custom instance of IQueryConfigurator
IQueryConfigurator queryConfigurator = new YourCustomQueryConfigurator();

// Build a query for a specific type
IQuery query = queryConfigurator.BuildFor<YourEntityType>();

// Execute the query
var results = await query.Execute();
```

### Custom Operators

NetQueryBuilder allows you to define custom operators for specific needs:

```csharp
// Create a custom operator
public class CustomOperator : OperatorBase
{
    public override string Symbol => "CustomOp";

    public override Expression BuildExpression(Expression left, Expression right)
    {
        // Custom logic to build an expression
        return Expression.Call(
            typeof(CustomMethods).GetMethod("YourCustomMethod"),
            left,
            right
        );
    }
}
```

### Condition Configuration

```csharp
// Manual condition configuration
queryConfigurator.ConfigureConditions(config => {
    // Register custom operators
    config.RegisterOperator<CustomOperator>();
    
    // Additional configuration
});

// Create a condition in a query
var query = queryConfigurator.BuildFor<YourEntityType>();
var property = query.ConditionPropertyPaths.First(p => p.PropertyFullName == "YourProperty");
query.Condition.CreateNew(property, new CustomOperator(), yourValue);
```

## Extending with Your Own Data Sources

To integrate NetQueryBuilder with your own data source or ORM, you need to implement the key interfaces:

```csharp
// Custom query configurator implementation
public class CustomQueryConfigurator : IQueryConfigurator
{
    public IQuery BuildFor<T>() where T : class
    {
        // Your implementation to create a query
        return new CustomQuery<T>();
    }
    
    // Other method implementations
}

// Custom query implementation
public class CustomQuery<T> : IQuery where T : class
{
    public async Task<IEnumerable<object>> Execute()
    {
        // Your logic to execute the query on your data source
        // Transform your condition criteria into appropriate query for your ORM/source
    }
    
    // Other method implementations
}
```

## Available Implementations

While the core package can be used on its own, NetQueryBuilder also offers specific implementations:

- **NetQueryBuilder.EntityFramework**: Integration with Entity Framework Core
- **NetQueryBuilder.Blazor**: UI components for Blazor

## Customization Use Cases

NetQueryBuilder core is particularly useful in the following scenarios:

- Integration with ORMs not officially supported
- Creating queries for REST APIs or NoSQL data sources
- Implementing domain-specific filtering logic
- Creating DSLs (Domain-Specific Languages) for queries
- Use in environments where Entity Framework is not available or desired

## Contribution

Contributions are welcome! You can contribute by implementing adapters for other ORMs or by improving the features of the core package.