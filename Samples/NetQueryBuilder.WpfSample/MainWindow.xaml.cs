using System.Diagnostics;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using NetQueryBuilder.EntityFramework;
using NetQueryBuilder.WpfSample.Data;

namespace NetQueryBuilder.WpfSample;

/// <summary>
///     Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly SampleDbContext _dbContext;

    public MainWindow()
    {
        InitializeComponent();

        Debug.WriteLine("=== MainWindow: Constructor started ===");

        // Initialize Entity Framework with InMemory database
        var options = new DbContextOptionsBuilder<SampleDbContext>()
            .UseInMemoryDatabase("SampleDatabase")
            .UseLazyLoadingProxies()
            .Options;

        _dbContext = new SampleDbContext(options);
        InitializeAsync().ConfigureAwait(false).GetAwaiter().GetResult();
    }

    private async Task InitializeAsync()
    {
        try
        {
            Debug.WriteLine("=== MainWindow: Starting async initialization ===");

            // Seed the database with sample data
            await _dbContext.SeedDatabase().ConfigureAwait(false);
            Debug.WriteLine("=== MainWindow: Database seeded ===");

            // Create Entity Framework query configurator
            var configurator = new EfQueryConfigurator<SampleDbContext>(_dbContext);
            Debug.WriteLine(
                $"=== MainWindow: Configurator created with {configurator.GetEntities().Count()} entities ===");

            // Set configurator on the QueryBuilderContainer
            QueryContainer.Configurator = configurator;
            Debug.WriteLine("=== MainWindow: Configurator set on QueryContainer ===");

            // Show window after successful initialization
            Visibility = Visibility.Visible;
            Debug.WriteLine("=== MainWindow: Window made visible ===");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"=== MainWindow: INITIALIZATION FAILED: {ex.Message} ===");
            Debug.WriteLine($"=== MainWindow: Stack trace: {ex.StackTrace} ===");

            MessageBox.Show(
                $"Failed to initialize the application:\n\n{ex.Message}\n\nStack trace:\n{ex.StackTrace}",
                "Initialization Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            Close();
        }
    }
}