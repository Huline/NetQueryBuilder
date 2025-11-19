using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows.Threading;
using NetQueryBuilder.Properties;
using NetQueryBuilder.Queries;
using NetQueryBuilder.WPF.Commands;

namespace NetQueryBuilder.WPF.ViewModels;

/// <summary>
///     ViewModel for the main QueryBuilder control.
/// </summary>
public class QueryBuilderViewModel : ViewModelBase
{
    private readonly DispatcherTimer _debounceTimer;
    private string _expressionPreview = string.Empty;
    private bool _isExecuting;
    private QueryResult<dynamic>? _queryResults;
    private ObservableCollection<SelectPropertyPath> _selectableProperties;

    public QueryBuilderViewModel(IQuery query)
    {
        Query = query ?? throw new ArgumentNullException(nameof(query));
        _selectableProperties = new ObservableCollection<SelectPropertyPath>(Query.SelectPropertyPaths);

        // Setup debounce timer for expression updates
        _debounceTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(300)
        };
        _debounceTimer.Tick += OnDebounceTimerTick;

        // Subscribe to query changes
        Query.OnChanged += OnQueryChanged;

        // Initialize commands
        ExecuteQueryCommand = new AsyncRelayCommand(async _ => await ExecuteQueryAsync(), _ => !IsExecuting);
        SelectAllFieldsCommand = new RelayCommand(_ => SelectAllFields());
        ClearAllFieldsCommand = new RelayCommand(_ => ClearAllFields());

        // Initial expression compilation
        UpdateExpression();
    }

    public QueryBuilderViewModel()
    {
    }

    /// <summary>
    ///     Gets the underlying query.
    /// </summary>
    public IQuery Query { get; }

    /// <summary>
    ///     Gets the collection of selectable properties for the SELECT clause.
    /// </summary>
    public ObservableCollection<SelectPropertyPath> SelectableProperties
    {
        get => _selectableProperties;
        set => SetProperty(ref _selectableProperties, value);
    }

    /// <summary>
    ///     Gets or sets the generated expression preview.
    /// </summary>
    public string ExpressionPreview
    {
        get => _expressionPreview;
        set => SetProperty(ref _expressionPreview, value);
    }

    /// <summary>
    ///     Gets or sets the query results.
    /// </summary>
    public QueryResult<dynamic>? QueryResults
    {
        get => _queryResults;
        set => SetProperty(ref _queryResults, value);
    }

    /// <summary>
    ///     Gets or sets whether a query is currently executing.
    /// </summary>
    public bool IsExecuting
    {
        get => _isExecuting;
        set => SetProperty(ref _isExecuting, value);
    }

    /// <summary>
    ///     Gets the command to execute the query.
    /// </summary>
    public ICommand ExecuteQueryCommand { get; }

    /// <summary>
    ///     Gets the command to select all fields.
    /// </summary>
    public ICommand SelectAllFieldsCommand { get; }

    /// <summary>
    ///     Gets the command to clear all field selections.
    /// </summary>
    public ICommand ClearAllFieldsCommand { get; }

    private void OnQueryChanged(object? sender, EventArgs e)
    {
        // Debounce expression updates to avoid excessive compilations
        _debounceTimer.Stop();
        _debounceTimer.Start();
    }

    private void OnDebounceTimerTick(object? sender, EventArgs e)
    {
        _debounceTimer.Stop();
        UpdateExpression();
    }

    private void UpdateExpression()
    {
        var compiled = Query.Compile();
        ExpressionPreview = compiled?.ToString() ?? string.Empty;
    }

    private async Task ExecuteQueryAsync()
    {
        IsExecuting = true;
        try
        {
            QueryResults = await Query.Execute(10);
        }
        finally
        {
            IsExecuting = false;
        }
    }

    private void SelectAllFields()
    {
        foreach (var property in SelectableProperties) property.IsSelected = true;
    }

    private void ClearAllFields()
    {
        foreach (var property in SelectableProperties) property.IsSelected = false;
    }
}