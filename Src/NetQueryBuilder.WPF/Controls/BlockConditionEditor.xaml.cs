using System.Diagnostics;
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
            new PropertyMetadata(null, OnConditionChanged, CoerceCondition));

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

    private static object CoerceCondition(DependencyObject d, object baseValue)
    {
        if (d is BlockConditionEditor editor)
        {
            Debug.WriteLine($"=== BlockConditionEditor: CoerceCondition called with value={baseValue?.GetType().Name ?? "null"}, current={editor.Condition?.GetType().Name ?? "null"} ===");

            // If the new value is null but we have a valid current value, keep the current value
            if (baseValue == null && editor.Condition != null)
            {
                Debug.WriteLine($"=== BlockConditionEditor: CoerceCondition rejecting null, keeping current BlockCondition ===");
                return editor.Condition;
            }

            // If the new value is not a BlockCondition (e.g., it's a ViewModel), reject it and keep the old value
            if (baseValue != null && baseValue is not BlockCondition)
            {
                Debug.WriteLine($"=== BlockConditionEditor: CoerceCondition rejecting invalid value of type {baseValue.GetType().Name} ===");
                return editor.Condition ?? baseValue; // Keep current value
            }
        }
        return baseValue;
    }

    private static void OnConditionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        Debug.WriteLine($"=== BlockConditionEditor: OnConditionChanged - Old={e.OldValue?.GetType().Name}, New={e.NewValue?.GetType().Name} ===");
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
        // Create ViewModel synchronously to avoid binding errors
        // The DataTemplate sets DataContext to BlockCondition, but our XAML binds to BlockConditionViewModel
        // We must replace the DataContext immediately before WPF tries to resolve bindings
        UpdateViewModelCore();
    }

    private void UpdateViewModelCore()
    {
        Debug.WriteLine($"=== BlockConditionEditor: UpdateViewModelCore called - Query={Query != null}, Condition={Condition != null} ===");

        // Check if both required properties are set
        if (Query == null || Condition == null)
        {
            Debug.WriteLine("=== BlockConditionEditor: Missing Query or Condition, skipping ViewModel creation ===");
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
            Debug.WriteLine("=== BlockConditionEditor: Values haven't changed, keeping existing ViewModel ===");
            // Values haven't changed, keep existing ViewModel
            return;
        }

        // Create new ViewModel with current property values
        Debug.WriteLine($"=== BlockConditionEditor: Creating new BlockConditionViewModel ===");
        _lastQuery = Query;
        _lastCondition = Condition;
        _lastIndentationLevel = IndentationLevel;
        DataContext = new BlockConditionViewModel(Query, Condition, IndentationLevel);
        Debug.WriteLine($"=== BlockConditionEditor: ViewModel set as DataContext ===");
    }
}
