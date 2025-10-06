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
        if (Query == null || Condition == null)
        {
            DataContext = null;
            return;
        }

        DataContext = new BlockConditionViewModel(Query, Condition, IndentationLevel);
    }
}
