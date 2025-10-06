# NetQueryBuilder WPF Sample Application

This sample application demonstrates how to use the NetQueryBuilder.WPF library with Entity Framework Core to build dynamic queries with a rich WPF user interface.

## Features

- **Entity Framework Integration**: Uses EF Core with InMemory database provider
- **Multiple Entities**: Person, Address, and Utility models with relationships
- **Navigation Properties**: Full support for querying related data
- **Professional UI**: Material Design-inspired WPF controls
- **Dynamic Query Building**: Visual query builder with property selection and conditions
- **Paginated Results**: DataGrid with pagination controls

## Project Structure

```
NetQueryBuilder.WpfSample/
├── Data/
│   ├── SampleDbContext.cs           # EF Core DbContext
│   └── SampleDbContextExtensions.cs # Database seeding
├── Models/
│   ├── Person.cs                    # Main entity
│   ├── Address.cs                   # Related entity (1-to-many)
│   └── Utility.cs                   # Related entity (1-to-many)
├── MainWindow.xaml                  # Main window layout
├── MainWindow.xaml.cs               # Application logic
└── App.xaml                         # Application resources (themes)
```

## Data Model

### Person
- **PersonId** (string, PK)
- FirstName, LastName
- NumberOfChildren (int)
- IsAlive (bool)
- Created (DateTime)
- **Addresses** (navigation property)

### Address
- **AddressId** (int, PK)
- PersonId (FK to Person)
- City
- IsPrimary (bool)
- **Person** (navigation property)
- **Utilities** (navigation property)

### Utility
- **UtilityId** (int, PK)
- AddressId (FK to Address)
- Provider, AccountNumber, Type
- **Address** (navigation property)

## How It Works

1. **Database Initialization**: On startup, the app creates an InMemory EF Core database
2. **Data Seeding**: Sample data is loaded with 4 people, 5 addresses, and 4 utilities
3. **EF Configurator**: `EfQueryConfigurator<SampleDbContext>` provides query building capabilities
4. **Query Builder UI**: `QueryBuilderContainer` control displays entity selection and query builder
5. **Dynamic Querying**: Users can:
   - Select which entity to query (Person, Address, Utility)
   - Choose properties to display in results
   - Build complex WHERE conditions
   - View generated LINQ expressions
   - Execute queries and view paginated results

## Running the Application

```bash
# Build the project
dotnet build

# Run the application
dotnet run

# Or run from Visual Studio / Rider
```

## Example Queries

### Find People with Children in Paris
1. Select **Person** entity
2. Add condition: `NumberOfChildren > 0`
3. Add condition: `Addresses.City == "Paris"`
4. Select properties: FirstName, LastName, NumberOfChildren
5. Execute query

### Find Primary Addresses with Utilities
1. Select **Address** entity
2. Add condition: `IsPrimary == true`
3. Add condition: `Utilities.Count > 0`
4. Select properties: City, Person.FirstName, Person.LastName
5. Execute query

### Find Electricity Utilities
1. Select **Utility** entity
2. Add condition: `Type == "Electricity"`
3. Select properties: Provider, AccountNumber, Address.City, Address.Person.FirstName
4. Execute query

## Key Technologies

- **.NET 9.0** - Target framework
- **WPF** - UI framework
- **Entity Framework Core 9.0** - ORM
- **InMemory Database** - For demo purposes
- **NetQueryBuilder.WPF** - Query builder UI components
- **NetQueryBuilder.EntityFramework** - EF Core integration

## Notes

- The application uses lazy loading proxies for navigation properties
- Database is seeded on every run (InMemory database is transient)
- All entities are automatically discovered from the DbContext
- The QueryBuilder supports nested property paths (e.g., `Address.Person.FirstName`)
- Theme resources are loaded in `App.xaml` for consistent styling
