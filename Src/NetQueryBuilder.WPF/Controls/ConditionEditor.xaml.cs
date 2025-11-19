using System.Windows;
using System.Windows.Controls;
using NetQueryBuilder.Conditions;
using NetQueryBuilder.Queries;

namespace NetQueryBuilder.WPF.Controls;

/// <summary>
///     A router control that selects the appropriate editor for a condition.
/// </summary>
public partial class ConditionEditor : UserControl
{
    public static readonly DependencyProperty QueryProperty =
        DependencyProperty.Register(
            nameof(Query),
            typeof(IQuery),
            typeof(ConditionEditor),
            new PropertyMetadata(null));

    public static readonly DependencyProperty ConditionProperty =
        DependencyProperty.Register(
            nameof(Condition),
            typeof(ICondition),
            typeof(ConditionEditor),
            new PropertyMetadata(null));

    public static readonly DependencyProperty IndentationLevelProperty =
        DependencyProperty.Register(
            nameof(IndentationLevel),
            typeof(int),
            typeof(ConditionEditor),
            new PropertyMetadata(0));

    public ConditionEditor()
    {
        InitializeComponent();
    }

    /// <summary>
    ///     Gets or sets the query context.
    /// </summary>
    public IQuery? Query
    {
        get => (IQuery?)GetValue(QueryProperty);
        set => SetValue(QueryProperty, value);
    }

    public ICondition? Condition
    {
        get => (ICondition?)GetValue(ConditionProperty);
        set => SetValue(ConditionProperty, value);
    }

    /// <summary>
    ///     Gets or sets the indentation level for nested conditions.
    /// </summary>
    public int IndentationLevel
    {
        get => (int)GetValue(IndentationLevelProperty);
        set => SetValue(IndentationLevelProperty, value);
    }
}