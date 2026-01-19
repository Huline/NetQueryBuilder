# NetQueryBuilder

[![Build Status](https://github.com/huline/NetQueryBuilder/actions/workflows/deploy.yaml/badge.svg)](https://github.com/huline/NetQueryBuilder/actions/workflows/deploy.yaml)
[![NuGet](https://img.shields.io/nuget/v/NetQueryBuilder.svg)](https://www.nuget.org/packages/NetQueryBuilder/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/NetQueryBuilder.svg)](https://www.nuget.org/packages/NetQueryBuilder/)
[![License](https://img.shields.io/badge/license-MIT%2FCommercial-blue.svg)](LICENSE)

NetQueryBuilder is a comprehensive .NET ecosystem for building and executing dynamic queries across different data
sources with extensible UI components.

## Screenshots

### Blazor Query Builder

<!-- TODO: Add screenshot of Blazor query builder UI -->
![Blazor Query Builder](docs/images/blazor-query-builder.png)

### WPF Query Editor

<!-- TODO: Add screenshot of WPF query editor UI -->
![WPF Query Editor](docs/images/wpf-query-editor.png)

### ASP.NET Core Razor Pages

<!-- TODO: Add screenshot of ASP.NET Core query builder UI -->
![ASP.NET Core Query Builder](docs/images/aspnetcore-query-builder.png)

## Live Demos

- **Blazor Demo**: [Try it online](<!-- TODO: Add Blazor demo URL -->)
- **WPF Sample**: [Download executable](<!-- TODO: Add WPF sample download URL -->)

## Vision

NetQueryBuilder aims to simplify the creation of dynamic query interfaces in .NET applications by providing:

- A type-safe, extensible core for building queries
- Seamless integration with popular ORMs and data sources
- Rich UI components for visual query building
- A consistent API across different implementations

The project enables developers to create powerful search, filter, and reporting functionality without writing complex
LINQ expressions or SQL queries manually, while maintaining full control over performance and customization.

## License

**Important:** NetQueryBuilder is open-source under the MIT license for personal, educational, and non-commercial use.
For commercial use, a valid commercial license must be purchased
from [https://huline.gumroad.com/l/netquerybuilder](https://huline.gumroad.com/l/netquerybuilder).
See [LICENSE](./Src/NetQueryBuilder/LICENSE) for more details.

## Packages

NetQueryBuilder is structured as a modular ecosystem with these main packages:

| Package                             | NuGet                                                                                                                                           | Description                                     | Documentation                                           |
|-------------------------------------|-------------------------------------------------------------------------------------------------------------------------------------------------|-------------------------------------------------|---------------------------------------------------------|
| **NetQueryBuilder**                 | [![NuGet](https://img.shields.io/nuget/v/NetQueryBuilder.svg)](https://www.nuget.org/packages/NetQueryBuilder/)                                 | Core library with IQueryable-based architecture | [Docs](./Src/NetQueryBuilder/Readme.md)                 |
| **NetQueryBuilder.EntityFramework** | [![NuGet](https://img.shields.io/nuget/v/NetQueryBuilder.EntityFramework.svg)](https://www.nuget.org/packages/NetQueryBuilder.EntityFramework/) | Entity Framework Core integration               | [Docs](./Src/NetQueryBuilder.EntityFramework/Readme.md) |
| **NetQueryBuilder.Blazor**          | [![NuGet](https://img.shields.io/nuget/v/NetQueryBuilder.Blazor.svg)](https://www.nuget.org/packages/NetQueryBuilder.Blazor/)                   | Blazor UI components (MudBlazor)                | [Docs](./Src/NetQueryBuilder.Blazor/Readme.md)          |
| **NetQueryBuilder.AspNetCore**      | [![NuGet](https://img.shields.io/nuget/v/NetQueryBuilder.AspNetCore.svg)](https://www.nuget.org/packages/NetQueryBuilder.AspNetCore/)           | ASP.NET Core Razor Pages integration            | [Docs](./Src/NetQueryBuilder.AspNetCore/Readme.md)      |
| **NetQueryBuilder.WPF**             | [![NuGet](https://img.shields.io/nuget/v/NetQueryBuilder.WPF.svg)](https://www.nuget.org/packages/NetQueryBuilder.WPF/)                         | WPF UI components                               | [Docs](./Src/NetQueryBuilder.WPF/README.md)             |

## Quick Start

### 1. Install Packages

Choose the packages that match your application's needs:

```shell script
# Core package (required)
dotnet add package NetQueryBuilder

# For Entity Framework Core integration
dotnet add package NetQueryBuilder.EntityFramework

# For Blazor UI components
dotnet add package NetQueryBuilder.Blazor
dotnet add package MudBlazor  # Required by NetQueryBuilder.Blazor
```

### 2. Basic Setup

#### Using with Entity Framework Core

```csharp
// Register services
services.AddDbContext<YourDbContext>(options => 
    options.UseSqlServer(connectionString));
    
services.AddScoped<IQueryConfigurator, EfQueryConfigurator<YourDbContext>>();
```

#### Add Blazor UI Components

In your Blazor page or component:

```cshtml
@page "/products"
@using NetQueryBuilder.Blazor.Components
@using YourNamespace.Models

<h3>Product Query Builder</h3>

<QueryBuilder TEntity="Product" />
```

## Features Overview

### Core Features

- Dynamic query construction with IQueryable
- Extensible operator system (Equals, Like, Contains, etc.)
- Property path navigation for complex object graphs
- Expression building and stringification

### Entity Framework Features

- Full integration with EF Core's querying capabilities
- Support for navigation properties and relationships
- Efficient translation to SQL queries
- Eager loading configuration

### Blazor UI Features

- Complete query builder component with SELECT and WHERE sections
- Condition builder for complex filtering logic
- Results table with dynamic columns
- Support for saving and loading query expressions

## Use Cases

- **Advanced Search Interfaces**: Create powerful search forms with complex filtering
- **Dynamic Reporting**: Allow users to define their own report criteria
- **Admin Dashboards**: Build flexible data exploration tools
- **Data Export Tools**: Generate filtered data exports with user-defined criteria
- **API Query Interfaces**: Enable complex filtering in your API endpoints

## Example: Building a Product Search

```csharp
// In a service class
public async Task<IEnumerable<Product>> SearchProducts(string category, decimal? minPrice)
{
    var query = _queryConfigurator.BuildFor<Product>();
    
    if (!string.IsNullOrEmpty(category))
    {
        var categoryProperty = query.ConditionPropertyPaths
            .First(p => p.PropertyFullName == "Category");
            
        query.Condition.CreateNew<EqualsOperator>(categoryProperty, category);
        
        if (minPrice.HasValue)
        {
            query.Condition.And();
            
            var priceProperty = query.ConditionPropertyPaths
                .First(p => p.PropertyFullName == "Price");
                
            query.Condition.CreateNew<GreaterThanOrEqualOperator>(priceProperty, minPrice.Value);
        }
    }
    
    return (await query.Execute(50)).Cast<Product>();
}
```

## Advanced Customization

NetQueryBuilder is designed to be extensible:

- Create custom operators for specialized filtering needs
- Implement adapters for different data sources
- Design custom UI components to match your application's look and feel
- Extend the query building process with your own logic

For details on customization options, see the package-specific documentation.

## Community and Support

- **GitHub Issues**: Report bugs or request features on our GitHub repository
- **Discussions**: Join our GitHub discussions for questions and ideas
- **Commercial Support**: Enterprise support is available with commercial licenses

## Contributing

Contributions are welcome! Whether it's:

- Bug fixes
- Documentation improvements
- New features
- Additional integrations

Please see our [Contributing Guide](CONTRIBUTING.md) for details.

## Roadmap

- Additional ORM integrations (Dapper, NHibernate)
- MAUI UI components
- GraphQL integration
- Query optimization features
- Cloud database adapters

## License Details

The MIT license applies to personal, educational, and non-commercial use. For commercial applications, please purchase a
license from [https://huline.gumroad.com/l/netquerybuilder](https://huline.gumroad.com/l/netquerybuilder)

Commercial licenses include:

- Unlimited commercial use
- Priority support
- Access to premium extensions
- No attribution requirement