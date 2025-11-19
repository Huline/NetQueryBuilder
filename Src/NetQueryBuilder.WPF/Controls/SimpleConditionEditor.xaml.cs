using System.Diagnostics;
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
            new PropertyMetadata(null, OnConditionChanged, CoerceCondition));

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

    private static object CoerceCondition(DependencyObject d, object baseValue)
    {
        if (d is SimpleConditionEditor editor)
        {
            Debug.WriteLine($"=== SimpleConditionEditor: CoerceCondition called with value={baseValue?.GetType().Name ?? "null"}, current={editor.Condition?.GetType().Name ?? "null"} ===");

            // If the new value is null but we have a valid current value, keep the current value
            if (baseValue == null && editor.Condition != null)
            {
                Debug.WriteLine($"=== SimpleConditionEditor: CoerceCondition rejecting null, keeping current SimpleCondition ===");
                return editor.Condition;
            }

            // If the new value is not a SimpleCondition (e.g., it's a ViewModel), reject it and keep the old value
            if (baseValue != null && baseValue is not SimpleCondition)
            {
                Debug.WriteLine($"=== SimpleConditionEditor: CoerceCondition rejecting invalid value of type {baseValue.GetType().Name} ===");
                return editor.Condition ?? baseValue;
            }
        }
        return baseValue;
    }

    private static void OnQueryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        Debug.WriteLine($"=== SimpleConditionEditor: OnQueryChanged - Old={e.OldValue?.GetType().Name}, New={e.NewValue?.GetType().Name} ===");
        if (d is SimpleConditionEditor editor)
            editor.UpdateViewModel();
    }

    private static void OnConditionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        Debug.WriteLine($"=== SimpleConditionEditor: OnConditionChanged - Old={e.OldValue?.GetType().Name}, New={e.NewValue?.GetType().Name} ===");
        if (d is SimpleConditionEditor editor)
            editor.UpdateViewModel();
    }

    private void UpdateViewModel()
    {
        // Create ViewModel synchronously to avoid binding errors
        UpdateViewModelCore();
    }

    private void UpdateViewModelCore()
    {
        Debug.WriteLine($"=== SimpleConditionEditor: UpdateViewModelCore called - Query={Query != null}, Condition={Condition != null} ===");

        // Check if both required properties are set
        if (Query == null || Condition == null)
        {
            Debug.WriteLine("=== SimpleConditionEditor: Missing Query or Condition, skipping ViewModel creation ===");
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
            Debug.WriteLine("=== SimpleConditionEditor: Values haven't changed, keeping existing ViewModel ===");
            // Values haven't changed, keep existing ViewModel
            return;
        }

        // Create new ViewModel with current property values
        Debug.WriteLine($"=== SimpleConditionEditor: Creating new SimpleConditionViewModel ===");
        _lastQuery = Query;
        _lastCondition = Condition;

        var viewModel = new SimpleConditionViewModel(Query, Condition);
        viewModel.DeleteRequested += (s, e) => DeleteRequested?.Invoke(this, EventArgs.Empty);
        DataContext = viewModel;
        Debug.WriteLine($"=== SimpleConditionEditor: ViewModel set as DataContext ===");
    }
}
