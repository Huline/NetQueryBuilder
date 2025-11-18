using System.Windows;
using System.Windows.Controls;
using NetQueryBuilder.Conditions;
using NetQueryBuilder.Queries;
using NetQueryBuilder.WPF.ViewModels;

namespace NetQueryBuilder.WPF.Controls;

/// <summary>
/// Editor for a block condition (grouped conditions).
/// </summary>
public partial class BlockConditionEditor : UserControl
{
    public static readonly DependencyProperty QueryProperty =
        DependencyProperty.Register(
            nameof(Query),
            typeof(IQuery),
            typeof(BlockConditionEditor),
            new PropertyMetadata(null, OnQueryChanged));

    public static readonly DependencyProperty ConditionProperty =
        DependencyProperty.Register(
            nameof(Condition),
            typeof(BlockCondition),
            typeof(BlockConditionEditor),
            new PropertyMetadata(null, OnConditionChanged));

    public static readonly DependencyProperty IndentationLevelProperty =
        DependencyProperty.Register(
            nameof(IndentationLevel),
            typeof(int),
            typeof(BlockConditionEditor),
            new PropertyMetadata(0, OnIndentationLevelChanged));

    private bool _isUpdatePending;
    private IQuery? _lastQuery;
    private BlockCondition? _lastCondition;
    private int _lastIndentationLevel;

    public BlockConditionEditor()
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
    /// Gets or sets the block condition to edit.
    /// </summary>
    public BlockCondition? Condition
    {
        get => (BlockCondition?)GetValue(ConditionProperty);
        set => SetValue(ConditionProperty, value);
    }

    /// <summary>
    /// Gets or sets the indentation level for visual hierarchy.
    /// </summary>
    public int IndentationLevel
    {
        get => (int)GetValue(IndentationLevelProperty);
        set => SetValue(IndentationLevelProperty, value);
    }

    private static void OnQueryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is BlockConditionEditor editor)
            editor.UpdateViewModel();
    }

    private static void OnConditionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is BlockConditionEditor editor)
            editor.UpdateViewModel();
    }

    private static void OnIndentationLevelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is BlockConditionEditor editor)
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
            if (DataContext is BlockConditionViewModel && (_lastQuery != null || _lastCondition != null))
            {
                // Properties were valid before but now are null - clear
                DataContext = null;
                _lastQuery = null;
                _lastCondition = null;
                _lastIndentationLevel = 0;
            }
            return;
        }

        // Check if values actually changed
        if (Query == _lastQuery &&
            Condition == _lastCondition &&
            IndentationLevel == _lastIndentationLevel &&
            DataContext is BlockConditionViewModel)
        {
            // Values haven't changed, keep existing ViewModel
            return;
        }

        // Create new ViewModel with current property values
        _lastQuery = Query;
        _lastCondition = Condition;
        _lastIndentationLevel = IndentationLevel;
        DataContext = new BlockConditionViewModel(Query, Condition, IndentationLevel);
    }
}
