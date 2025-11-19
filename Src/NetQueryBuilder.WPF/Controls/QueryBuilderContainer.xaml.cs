using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using NetQueryBuilder.Configurations;
using NetQueryBuilder.WPF.ViewModels;

namespace NetQueryBuilder.WPF.Controls;

/// <summary>
/// Top-level container for the query builder with entity selection.
/// </summary>
public partial class QueryBuilderContainer : UserControl
{
    public static readonly DependencyProperty ConfiguratorProperty =
        DependencyProperty.Register(
            nameof(Configurator),
            typeof(IQueryConfigurator),
            typeof(QueryBuilderContainer),
            new PropertyMetadata(null, OnConfiguratorChanged));

    private QueryBuilderContainerViewModel? _viewModel;

    public QueryBuilderContainer()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Gets or sets the query configurator.
    /// </summary>
    public IQueryConfigurator? Configurator
    {
        get => (IQueryConfigurator?)GetValue(ConfiguratorProperty);
        set => SetValue(ConfiguratorProperty, value);
    }

    private static void OnConfiguratorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        Debug.WriteLine("=== QueryBuilderContainer: Configurator changed ===");
        if (d is QueryBuilderContainer container && e.NewValue is IQueryConfigurator configurator)
        {
            Debug.WriteLine("=== QueryBuilderContainer: Initializing ViewModel ===");
            container.InitializeViewModel(configurator);
        }
        else
        {
            Debug.WriteLine($"=== QueryBuilderContainer: Invalid state - d={d?.GetType().Name}, configurator={e.NewValue} ===");
        }
    }

    private void InitializeViewModel(IQueryConfigurator configurator)
    {
        Debug.WriteLine("=== QueryBuilderContainer: Creating QueryBuilderContainerViewModel ===");
        _viewModel = new QueryBuilderContainerViewModel(configurator);
        DataContext = _viewModel;

        // Subscribe to property changes to update the QueryBuilder
        _viewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(QueryBuilderContainerViewModel.CurrentQuery))
            {
                Debug.WriteLine("=== QueryBuilderContainer: CurrentQuery property changed ===");
                UpdateQueryBuilder();
            }
        };

        // Initialize first query
        Debug.WriteLine("=== QueryBuilderContainer: Initial UpdateQueryBuilder call ===");
        UpdateQueryBuilder();
    }

    private void UpdateQueryBuilder()
    {
        Debug.WriteLine("=== QueryBuilderContainer: UpdateQueryBuilder called ===");

        if (_viewModel?.CurrentQuery == null)
        {
            Debug.WriteLine("=== QueryBuilderContainer: No query available, clearing UI ===");
            QueryBuilderHost.Content = null;
            ResultsGrid.Results = null;
            return;
        }

        Debug.WriteLine($"=== QueryBuilderContainer: Creating QueryBuilder for {_viewModel.CurrentQuery.GetType().Name} ===");

        var queryBuilder = new QueryBuilder
        {
            Query = _viewModel.CurrentQuery
        };

        Debug.WriteLine($"=== QueryBuilderContainer: QueryBuilder created, ViewModel={queryBuilder.ViewModel != null} ===");

        // Subscribe to query execution to update results
        if (queryBuilder.ViewModel != null)
        {
            queryBuilder.ViewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(QueryBuilderViewModel.QueryResults))
                {
                    Debug.WriteLine("=== QueryBuilderContainer: Query results updated ===");
                    ResultsGrid.Results = queryBuilder.ViewModel.QueryResults;
                    ResultsGrid.DisplayProperties = queryBuilder.ViewModel.SelectableProperties;
                }
            };
        }

        QueryBuilderHost.Content = queryBuilder;
        Debug.WriteLine("=== QueryBuilderContainer: QueryBuilder set as content ===");
    }
}
