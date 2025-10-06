using System.Windows;
using System.Windows.Controls;
using NetQueryBuilder.Queries;
using NetQueryBuilder.WPF.ViewModels;

namespace NetQueryBuilder.WPF.Controls;

/// <summary>
/// The main QueryBuilder control for building and executing queries.
/// </summary>
public partial class QueryBuilder : UserControl
{
    public static readonly DependencyProperty QueryProperty =
        DependencyProperty.Register(
            nameof(Query),
            typeof(IQuery),
            typeof(QueryBuilder),
            new PropertyMetadata(null, OnQueryChanged));

    public QueryBuilder()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Gets or sets the query to build.
    /// </summary>
    public IQuery? Query
    {
        get => (IQuery?)GetValue(QueryProperty);
        set => SetValue(QueryProperty, value);
    }

    /// <summary>
    /// Gets the ViewModel (for accessing query results externally).
    /// </summary>
    public QueryBuilderViewModel? ViewModel => DataContext as QueryBuilderViewModel;

    private static void OnQueryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is QueryBuilder builder && e.NewValue is IQuery query)
        {
            builder.DataContext = new QueryBuilderViewModel(query);
        }
    }
}
