using System.Windows;
using System.Windows.Controls;
using NetQueryBuilder.Conditions;
using NetQueryBuilder.Queries;
using NetQueryBuilder.WPF.ViewModels;

namespace NetQueryBuilder.WPF.Controls;

/// <summary>
/// Editor for a simple condition (Property Operator Value).
/// </summary>
public partial class SimpleConditionEditor : UserControl
{
    public static readonly DependencyProperty QueryProperty =
        DependencyProperty.Register(
            nameof(Query),
            typeof(IQuery),
            typeof(SimpleConditionEditor),
            new PropertyMetadata(null, OnQueryChanged));

    public static readonly DependencyProperty ConditionProperty =
        DependencyProperty.Register(
            nameof(Condition),
            typeof(SimpleCondition),
            typeof(SimpleConditionEditor),
            new PropertyMetadata(null, OnConditionChanged));

    private bool _isUpdatePending;
    private IQuery? _lastQuery;
    private SimpleCondition? _lastCondition;

    public SimpleConditionEditor()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Gets or sets the query context.
    /// </summary>
    public IQuery? Query
    {
        get => (IQuery?)GetValue(QueryProperty);
        set => SetValue(QueryProperty, value);
    }

    /// <summary>
    /// Gets or sets the condition to edit.
    /// </summary>
    public SimpleCondition? Condition
    {
        get => (SimpleCondition?)GetValue(ConditionProperty);
        set => SetValue(ConditionProperty, value);
    }

    /// <summary>
    /// Occurs when the user requests to delete this condition.
    /// </summary>
    public event EventHandler? DeleteRequested;

    private static void OnQueryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is SimpleConditionEditor editor)
            editor.UpdateViewModel();
    }

    private static void OnConditionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is SimpleConditionEditor editor)
            editor.UpdateViewModel();
    }

    private void UpdateViewModel()
    {
        // Don't schedule multiple deferred updates
        if (_isUpdatePending)
            return;

        _isUpdatePending = true;

        // Defer update until UI thread is idle (all bindings resolved)
        // Using DataBind priority ensures RelativeSource bindings have resolved
        Dispatcher.BeginInvoke(new Action(() =>
        {
            _isUpdatePending = false;
            UpdateViewModelCore();
        }), System.Windows.Threading.DispatcherPriority.DataBind);
    }

    private void UpdateViewModelCore()
    {
        // Check if both required properties are set
        if (Query == null || Condition == null)
        {
            // Don't aggressively clear DataContext during initialization
            // Only clear if we previously had valid properties and now they're null
            if (DataContext is SimpleConditionViewModel && (_lastQuery != null || _lastCondition != null))
            {
                // Properties were valid before but now are null - clear
                DataContext = null;
                _lastQuery = null;
                _lastCondition = null;
            }
            return;
        }

        // Check if values actually changed
        if (Query == _lastQuery &&
            Condition == _lastCondition &&
            DataContext is SimpleConditionViewModel)
        {
            // Values haven't changed, keep existing ViewModel
            return;
        }

        // Create new ViewModel with current property values
        _lastQuery = Query;
        _lastCondition = Condition;

        var viewModel = new SimpleConditionViewModel(Query, Condition);
        viewModel.DeleteRequested += (s, e) => DeleteRequested?.Invoke(this, EventArgs.Empty);
        DataContext = viewModel;
    }
}
