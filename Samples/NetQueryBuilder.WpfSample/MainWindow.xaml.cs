using System.Windows;
using NetQueryBuilder.Configurations;
using NetQueryBuilder.WpfSample.Models;

namespace NetQueryBuilder.WpfSample;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        InitializeQueryBuilder();
    }

    private void InitializeQueryBuilder()
    {
        // Create sample data
        var people = GenerateSamplePeople();

        // Create query configurator for Person entity
        var configurator = new QueryableQueryConfigurator<Person>(people.AsQueryable());

        // Set configurator on the QueryBuilderContainer
        QueryContainer.Configurator = configurator;
    }

    private static List<Person> GenerateSamplePeople()
    {
        return new List<Person>
        {
            new() { Id = 1, FirstName = "John", LastName = "Doe", Email = "john.doe@example.com", Age = 30, BirthDate = new DateTime(1993, 5, 15), IsActive = true, City = "New York", Country = "USA" },
            new() { Id = 2, FirstName = "Jane", LastName = "Smith", Email = "jane.smith@example.com", Age = 25, BirthDate = new DateTime(1998, 8, 22), IsActive = true, City = "London", Country = "UK" },
            new() { Id = 3, FirstName = "Bob", LastName = "Johnson", Email = "bob.johnson@example.com", Age = 35, BirthDate = new DateTime(1988, 3, 10), IsActive = false, City = "Paris", Country = "France" },
            new() { Id = 4, FirstName = "Alice", LastName = "Williams", Email = "alice.w@example.com", Age = 28, BirthDate = new DateTime(1995, 11, 5), IsActive = true, City = "Berlin", Country = "Germany" },
            new() { Id = 5, FirstName = "Charlie", LastName = "Brown", Email = "charlie.b@example.com", Age = 42, BirthDate = new DateTime(1981, 1, 20), IsActive = true, City = "Tokyo", Country = "Japan" },
            new() { Id = 6, FirstName = "Diana", LastName = "Davis", Email = "diana.d@example.com", Age = 31, BirthDate = new DateTime(1992, 7, 8), IsActive = false, City = "Sydney", Country = "Australia" },
            new() { Id = 7, FirstName = "Eve", LastName = "Martinez", Email = "eve.m@example.com", Age = 27, BirthDate = new DateTime(1996, 4, 18), IsActive = true, City = "Madrid", Country = "Spain" },
            new() { Id = 8, FirstName = "Frank", LastName = "Garcia", Email = "frank.g@example.com", Age = 38, BirthDate = new DateTime(1985, 9, 25), IsActive = true, City = "Rome", Country = "Italy" },
        };
    }

    private static List<Product> GenerateSampleProducts()
    {
        return new List<Product>
        {
            new() { Id = 1, Name = "Laptop", Category = "Electronics", Price = 999.99m, StockQuantity = 50, IsAvailable = true, CreatedDate = DateTime.Now.AddMonths(-6) },
            new() { Id = 2, Name = "Mouse", Category = "Electronics", Price = 29.99m, StockQuantity = 200, IsAvailable = true, CreatedDate = DateTime.Now.AddMonths(-3) },
            new() { Id = 3, Name = "Keyboard", Category = "Electronics", Price = 79.99m, StockQuantity = 150, IsAvailable = true, CreatedDate = DateTime.Now.AddMonths(-4) },
            new() { Id = 4, Name = "Monitor", Category = "Electronics", Price = 349.99m, StockQuantity = 75, IsAvailable = true, CreatedDate = DateTime.Now.AddMonths(-5) },
            new() { Id = 5, Name = "Desk Chair", Category = "Furniture", Price = 199.99m, StockQuantity = 30, IsAvailable = false, CreatedDate = DateTime.Now.AddMonths(-8) },
            new() { Id = 6, Name = "Desk Lamp", Category = "Furniture", Price = 49.99m, StockQuantity = 100, IsAvailable = true, CreatedDate = DateTime.Now.AddMonths(-2) },
        };
    }
}