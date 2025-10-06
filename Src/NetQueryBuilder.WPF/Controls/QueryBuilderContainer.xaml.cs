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
        if (d is QueryBuilderContainer container && e.NewValue is IQueryConfigurator configurator)
        {
            container.InitializeViewModel(configurator);
        }
    }

    private void InitializeViewModel(IQueryConfigurator configurator)
    {
        _viewModel = new QueryBuilderContainerViewModel(configurator);
        DataContext = _viewModel;

        // Subscribe to property changes to update the QueryBuilder
        _viewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(QueryBuilderContainerViewModel.CurrentQuery))
            {
                UpdateQueryBuilder();
            }
        };

        // Initialize first query
        UpdateQueryBuilder();
    }

    private void UpdateQueryBuilder()
    {
        if (_viewModel?.CurrentQuery == null)
        {
            QueryBuilderHost.Content = null;
            ResultsGrid.Results = null;
            return;
        }

        var queryBuilder = new QueryBuilder
        {
            Query = _viewModel.CurrentQuery
        };

        // Subscribe to query execution to update results
        if (queryBuilder.ViewModel != null)
        {
            queryBuilder.ViewModel.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(QueryBuilderViewModel.QueryResults))
                {
                    ResultsGrid.Results = queryBuilder.ViewModel.QueryResults;
                    ResultsGrid.DisplayProperties = queryBuilder.ViewModel.SelectableProperties;
                }
            };
        }

        QueryBuilderHost.Content = queryBuilder;
    }
}
