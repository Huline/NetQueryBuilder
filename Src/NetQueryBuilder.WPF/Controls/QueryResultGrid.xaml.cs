using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using NetQueryBuilder.Properties;
using NetQueryBuilder.Queries;
using NetQueryBuilder.WPF.ViewModels;

namespace NetQueryBuilder.WPF.Controls;

/// <summary>
/// Displays query results in a paginated DataGrid.
/// </summary>
public partial class QueryResultGrid : UserControl
{
    public static readonly DependencyProperty ResultsProperty =
        DependencyProperty.Register(
            nameof(Results),
            typeof(QueryResult<dynamic>),
            typeof(QueryResultGrid),
            new PropertyMetadata(null, OnResultsChanged));

    public static readonly DependencyProperty DisplayPropertiesProperty =
        DependencyProperty.Register(
            nameof(DisplayProperties),
            typeof(ObservableCollection<SelectPropertyPath>),
            typeof(QueryResultGrid),
            new PropertyMetadata(null, OnDisplayPropertiesChanged));

    private readonly QueryResultViewModel _viewModel;

    public QueryResultGrid()
    {
        InitializeComponent();
        _viewModel = new QueryResultViewModel();
        DataContext = _viewModel;
    }

    /// <summary>
    /// Gets or sets the query results to display.
    /// </summary>
    public QueryResult<dynamic>? Results
    {
        get => (QueryResult<dynamic>?)GetValue(ResultsProperty);
        set => SetValue(ResultsProperty, value);
    }

    /// <summary>
    /// Gets or sets the properties to display as columns.
    /// </summary>
    public ObservableCollection<SelectPropertyPath>? DisplayProperties
    {
        get => (ObservableCollection<SelectPropertyPath>?)GetValue(DisplayPropertiesProperty);
        set => SetValue(DisplayPropertiesProperty, value);
    }

    private static void OnResultsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is QueryResultGrid grid)
        {
            grid._viewModel.Results = e.NewValue as QueryResult<dynamic>;
        }
    }

    private static void OnDisplayPropertiesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is QueryResultGrid grid)
        {
            grid._viewModel.DisplayProperties = e.NewValue as ObservableCollection<SelectPropertyPath>;
        }
    }
}
