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
        if (Query == null || Condition == null)
        {
            DataContext = null;
            return;
        }

        var viewModel = new SimpleConditionViewModel(Query, Condition);
        viewModel.DeleteRequested += (s, e) => DeleteRequested?.Invoke(this, EventArgs.Empty);
        DataContext = viewModel;
    }
}
