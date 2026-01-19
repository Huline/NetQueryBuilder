# NetQueryBuilder.EntityFrameworkNet4

Entity Framework 6 integration for NetQueryBuilder, targeting **.NET Framework 4.8**. This package enables dynamic query
building and execution for legacy applications that cannot migrate to .NET Core/.NET 5+.

## License

**Important:** NetQueryBuilder.EntityFrameworkNet4 is open-source under the MIT license for personal, educational, and
non-commercial use.
For commercial use, a valid commercial license must be purchased
from [https://huline.gumroad.com/l/netquerybuilder](https://huline.gumroad.com/l/netquerybuilder).

## When to Use This Package

Use this package if:

- Your application targets **.NET Framework 4.8**
- You're using **Entity Framework 6** (not EF Core)
- You cannot migrate to .NET Core/.NET 5+ yet
- You need dynamic query building for legacy systems

For modern applications using .NET 6+,
use [NetQueryBuilder.EntityFramework](https://www.nuget.org/packages/NetQueryBuilder.EntityFramework/) instead.

## Installation

```powershell
Install-Package NetQueryBuilder
Install-Package NetQueryBuilder.EntityFrameworkNet4
```

Or via .NET CLI:

```shell
dotnet add package NetQueryBuilder
dotnet add package NetQueryBuilder.EntityFrameworkNet4
```

## Setup

### 1. Register Services

```csharp
// Using Microsoft.Extensions.DependencyInjection
services.AddScoped<IQueryConfigurator>(provider =>
{
    var dbContext = provider.GetRequiredService<YourDbContext>();
    return new EfQueryConfigurator<YourDbContext>(dbContext);
});
```

### 2. Configure Your DbContext

```csharp
public class YourDbContext : DbContext
{
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Order> Orders { get; set; }

    public YourDbContext() : base("name=YourConnectionString")
    {
    }
}
```

## Usage

### Basic Query Building

```csharp
// Get the configurator
var configurator = container.Resolve<IQueryConfigurator>();

// Build a query for Customers
var query = configurator.BuildFor<Customer>();

// Add conditions
var nameProperty = query.ConditionPropertyPaths.First(p => p.PropertyFullName == "Name");
query.Condition.CreateNew<ContainsOperator>(nameProperty, "Smith");

// Execute the query
var results = await query.Execute(100); // Max 100 results
var customers = results.Cast<Customer>();
```

### Using Navigation Properties

```csharp
// Query with related entities
var query = configurator.BuildFor<Order>();

// Filter by customer name (navigation property)
var customerNameProperty = query.ConditionPropertyPaths
    .First(p => p.PropertyFullName == "Customer.Name");

query.Condition.CreateNew<EqualsOperator>(customerNameProperty, "Acme Corp");
```

### Complex Conditions

```csharp
var query = configurator.BuildFor<Customer>();

// Build: Status = "Active" AND (City = "Paris" OR City = "London")
query.Condition.CreateNew<EqualsOperator>(statusProperty, "Active");
query.Condition.And();
query.Condition.OpenBlock();
query.Condition.CreateNew<EqualsOperator>(cityProperty, "Paris");
query.Condition.Or();
query.Condition.CreateNew<EqualsOperator>(cityProperty, "London");
query.Condition.CloseBlock();
```

## Available Operators

| Operator                     | Description        | Example               |
|------------------------------|--------------------|-----------------------|
| `EqualsOperator`             | Exact match        | `Status = "Active"`   |
| `NotEqualsOperator`          | Not equal          | `Status != "Deleted"` |
| `ContainsOperator`           | String contains    | `Name LIKE '%Smith%'` |
| `StartsWithOperator`         | String starts with | `Name LIKE 'John%'`   |
| `EndsWithOperator`           | String ends with   | `Name LIKE '%son'`    |
| `GreaterThanOperator`        | Greater than       | `Age > 18`            |
| `LessThanOperator`           | Less than          | `Price < 100`         |
| `GreaterThanOrEqualOperator` | Greater or equal   | `Quantity >= 10`      |
| `LessThanOrEqualOperator`    | Less or equal      | `Date <= Today`       |
| `IsNullOperator`             | Null check         | `DeletedAt IS NULL`   |
| `IsNotNullOperator`          | Not null check     | `Email IS NOT NULL`   |

## Differences from NetQueryBuilder.EntityFramework

| Feature          | EntityFrameworkNet4 | EntityFramework (Core)         |
|------------------|---------------------|--------------------------------|
| Target Framework | .NET Framework 4.8  | .NET 6.0+                      |
| EF Version       | Entity Framework 6  | EF Core 6.0+                   |
| Async Support    | Limited             | Full                           |
| Performance      | Good                | Better (EF Core optimizations) |
| Recommended For  | Legacy apps         | New applications               |

## Migration Path

When you're ready to migrate to .NET Core/.NET 5+:

1. Replace `NetQueryBuilder.EntityFrameworkNet4` with `NetQueryBuilder.EntityFramework`
2. Update DbContext to use EF Core conventions
3. The query building API remains the same

## Troubleshooting

### Missing EntityFramework reference

Ensure you have EntityFramework 6.x installed:

```powershell
Install-Package EntityFramework -Version 6.5.1
```

### DbContext not found

Make sure your DbContext inherits from `System.Data.Entity.DbContext` (EF6), not
`Microsoft.EntityFrameworkCore.DbContext` (EF Core).

## Contributing

Contributions are welcome! Please see the main [NetQueryBuilder repository](https://github.com/huline/NetQueryBuilder)
for contribution guidelines.
