using System.Windows;
using Microsoft.EntityFrameworkCore;
using NetQueryBuilder.EntityFramework;
using NetQueryBuilder.WpfSample.Data;
using NetQueryBuilder.WpfSample.Models;

namespace NetQueryBuilder.WpfSample;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly SampleDbContext _dbContext;

    public MainWindow()
    {
        InitializeComponent();

        // Initialize Entity Framework with InMemory database
        var options = new DbContextOptionsBuilder<SampleDbContext>()
            .UseInMemoryDatabase("SampleDatabase")
            .UseLazyLoadingProxies()
            .Options;

        _dbContext = new SampleDbContext(options);

        // Initialize and seed database, then setup query builder
        InitializeQueryBuilderAsync();

        // Clean up when window closes
        Closed += (s, e) => _dbContext?.Dispose();
    }

    private async void InitializeQueryBuilderAsync()
    {
        // Seed the database with sample data
        await _dbContext.SeedDatabase();

        // Create Entity Framework query configurator
        var configurator = new EfQueryConfigurator<SampleDbContext>(_dbContext);

        // Set configurator on the QueryBuilderContainer
        QueryContainer.Configurator = configurator;
    }
}
