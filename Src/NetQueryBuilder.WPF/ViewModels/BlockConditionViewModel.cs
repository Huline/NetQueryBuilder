using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using NetQueryBuilder.Conditions;
using NetQueryBuilder.Queries;
using NetQueryBuilder.WPF.Commands;

namespace NetQueryBuilder.WPF.ViewModels;

/// <summary>
/// ViewModel for a block condition (grouped conditions with logical operators).
/// </summary>
public class BlockConditionViewModel : ViewModelBase
{
    private readonly IQuery _query;
    private readonly BlockCondition _condition;
    private ObservableCollection<ICondition> _childConditions;
    private ObservableCollection<ICondition> _selectedConditions;
    private int _indentationLevel;

    public BlockConditionViewModel(IQuery query, BlockCondition condition, int indentationLevel = 0)
    {
        _query = query ?? throw new ArgumentNullException(nameof(query));
        _condition = condition ?? throw new ArgumentNullException(nameof(condition));
        _indentationLevel = indentationLevel;

        _childConditions = new ObservableCollection<ICondition>(_condition.Conditions);
        _selectedConditions = new ObservableCollection<ICondition>();

        AddConditionCommand = new RelayCommand(_ => AddCondition());
        GroupConditionsCommand = new RelayCommand(_ => GroupConditions(), _ => CanGroupConditions());
        UngroupConditionsCommand = new RelayCommand(_ => UngroupConditions(), _ => CanUngroup());

        // Subscribe to condition changes to update the collection
        _condition.ConditionChanged += OnConditionChanged;
    }

    /// <summary>
    /// Gets the collection of child conditions.
    /// </summary>
    public ObservableCollection<ICondition> ChildConditions
    {
        get => _childConditions;
        set => SetProperty(ref _childConditions, value);
    }

    /// <summary>
    /// Gets the collection of selected conditions for grouping.
    /// </summary>
    public ObservableCollection<ICondition> SelectedConditions
    {
        get => _selectedConditions;
        set => SetProperty(ref _selectedConditions, value);
    }

    /// <summary>
    /// Gets the indentation level for visual hierarchy.
    /// </summary>
    public int IndentationLevel
    {
        get => _indentationLevel;
        set => SetProperty(ref _indentationLevel, value);
    }

    /// <summary>
    /// Gets the left margin based on indentation level.
    /// </summary>
    public Thickness IndentationMargin => new Thickness(IndentationLevel * 20, 0, 0, 0);

    /// <summary>
    /// Gets whether this block has a parent (can be ungrouped).
    /// </summary>
    public bool HasParent => _condition.Parent != null;

    /// <summary>
    /// Gets the query context.
    /// </summary>
    public IQuery Query => _query;

    /// <summary>
    /// Gets the underlying block condition.
    /// </summary>
    public BlockCondition Condition => _condition;

    /// <summary>
    /// Gets the command to add a new condition.
    /// </summary>
    public ICommand AddConditionCommand { get; }

    /// <summary>
    /// Gets the command to group selected conditions.
    /// </summary>
    public ICommand GroupConditionsCommand { get; }

    /// <summary>
    /// Gets the command to ungroup conditions.
    /// </summary>
    public ICommand UngroupConditionsCommand { get; }

    private void AddCondition()
    {
        _condition.CreateNew();
        RefreshChildConditions();
    }

    private bool CanGroupConditions()
    {
        return SelectedConditions.Count >= 2;
    }

    private void GroupConditions()
    {
        _condition.Group(SelectedConditions);
        SelectedConditions.Clear();
        RefreshChildConditions();
    }

    private bool CanUngroup()
    {
        return HasParent && SelectedConditions.Count > 0;
    }

    private void UngroupConditions()
    {
        _condition.Ungroup(SelectedConditions);
        SelectedConditions.Clear();
        RefreshChildConditions();
    }

    private void OnConditionChanged(object? sender, EventArgs e)
    {
        RefreshChildConditions();
    }

    private void RefreshChildConditions()
    {
        ChildConditions = new ObservableCollection<ICondition>(_condition.Conditions);
    }

    public void RemoveCondition(ICondition condition)
    {
        _condition.Remove(condition);
        RefreshChildConditions();
    }
}
