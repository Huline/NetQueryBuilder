# NetQueryBuilder.EntityFramework

NetQueryBuilder.EntityFramework is an extension package that integrates NetQueryBuilder with Entity Framework Core, providing a powerful solution for building dynamic queries in your EF Core applications.

## License

**Important:** NetQueryBuilder.EntityFramework is open-source under the MIT license for personal, educational, and non-commercial use.
For commercial use, a valid commercial license must be purchased from [https://huline.gumroad.com/l/netquerybuilder](https://huline.gumroad.com/l/netquerybuilder).

## Description

This package extends the core NetQueryBuilder library to work seamlessly with Entity Framework Core. It allows you to dynamically build and execute complex queries against your EF Core DbContext without writing raw SQL or complex LINQ
expressions manually.

For detailed information about the core functionality and customization options, please refer to the [NetQueryBuilder Core documentation](../NetQueryBuilder/Readme.md).

## Installation

```shell script
dotnet add package NetQueryBuilder
dotnet add package NetQueryBuilder.EntityFramework
```

## Setup & Configuration

### Basic Setup

Register the NetQueryBuilder services in your application's service collection:

```csharp
// In your Startup.cs or Program.cs
services.AddDbContext<YourDbContext>(options => 
{
    options.UseSqlServer(connectionString);
});

// Register the EF implementation of IQueryConfigurator
services.AddScoped<IQueryConfigurator, EfQueryConfigurator<YourDbContext>>();
```

## Usage Examples

### Basic Query

```csharp
public class ProductService
{
    private readonly IQueryConfigurator _queryConfigurator;
    
    public ProductService(IQueryConfigurator queryConfigurator)
    {
        _queryConfigurator = queryConfigurator;
    }
    
    public async Task<IEnumerable<Product>> GetAllProductsAsync()
    {
        var query = _queryConfigurator.BuildFor<Product>();
        return (await query.Execute(50)).Cast<Product>();
    }
}
```

### Filtering with Conditions

```csharp
public async Task<IEnumerable<Product>> GetProductsByCategory(string category)
{
    var query = _queryConfigurator.BuildFor<Product>();
    
    // Create a condition using the Equals operator
    query.Condition.CreateNew<EqualsOperator>(
        query.ConditionPropertyPaths.First(p => p.PropertyFullName == "Category"),
        category
    );
    
    return (await query.Execute(50)).Cast<Product>();
}
```

### Complex Filtering with Navigation Properties

```csharp
public async Task<IEnumerable<Order>> GetOrdersByCustomerCity(string city)
{
    var query = _queryConfigurator.BuildFor<Order>();
    
    // Navigate through relationships
    query.Condition.CreateNew<EqualsOperator>(
        query.ConditionPropertyPaths.First(p => p.PropertyFullName == "Customer.City"),
        city
    );
    
    return (await query.Execute(50)).Cast<Order>();
}
```

### Using Different Operators

```csharp
public async Task<IEnumerable<Product>> SearchProductsByName(string searchTerm)
{
    var query = _queryConfigurator.BuildFor<Product>();
    var nameProperty = query.ConditionPropertyPaths.First(p => p.PropertyFullName == "Name");
    
    // Use the Like operator for partial matching
    query.Condition.CreateNew(
        nameProperty,
        nameProperty.GetCompatibleOperators().First(o => o.ToString() == "Like"),
        searchTerm
    );
    
    return (await query.Execute(50)).Cast<Product>();
}

public async Task<IEnumerable<Product>> GetProductsInPriceRange(decimal minPrice, decimal maxPrice)
{
    var query = _queryConfigurator.BuildFor<Product>();
    var priceProperty = query.ConditionPropertyPaths.First(p => p.PropertyFullName == "Price");
    
    // Create a compound condition for price range
    query.Condition.CreateNew(
        priceProperty,
        priceProperty.GetCompatibleOperators().First(o => o.ToString() == "GreaterThanOrEqual"),
        minPrice
    );
    
    query.Condition.And();
    
    query.Condition.CreateNew(
        priceProperty,
        priceProperty.GetCompatibleOperators().First(o => o.ToString() == "LessThanOrEqual"),
        maxPrice
    );
    
    return (await query.Execute(50)).Cast<Product>();
}
```

## Performance Considerations

The EntityFramework implementation translates your dynamic queries into EF Core's expression trees, which are then converted to SQL by EF Core. This means:

1. No client-side evaluation is performed (when possible)
2. The generated queries respect EF Core's optimization patterns
3. Navigation properties are handled efficiently with proper JOINs

However, complex dynamic queries may generate more complex SQL than hand-crafted queries. For performance-critical operations with known query patterns, consider using direct LINQ expressions.

## Contribution

Contributions are welcome! You can contribute by improving the EF Core integration or by adding support for additional EF Core features.