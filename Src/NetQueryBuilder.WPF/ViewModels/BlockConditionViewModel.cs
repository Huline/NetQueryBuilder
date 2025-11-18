using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using NetQueryBuilder.Conditions;
using NetQueryBuilder.Queries;
using NetQueryBuilder.WPF.Commands;

namespace NetQueryBuilder.WPF.ViewModels;

/// <summary>
///     ViewModel for a block condition (grouped conditions with logical operators).
/// </summary>
public class BlockConditionViewModel : ViewModelBase
{
    private ObservableCollection<ICondition> _childConditions;
    private int _indentationLevel;
    private ObservableCollection<ICondition> _selectedConditions;

    public BlockConditionViewModel(IQuery query, BlockCondition condition, int indentationLevel = 0)
    {
        Query = query ?? throw new ArgumentNullException(nameof(query));
        Condition = condition ?? throw new ArgumentNullException(nameof(condition));
        _indentationLevel = indentationLevel;

        _childConditions = new ObservableCollection<ICondition>(Condition.Conditions);
        _selectedConditions = new ObservableCollection<ICondition>();

        AddConditionCommand = new RelayCommand(_ => AddCondition());
        GroupConditionsCommand = new RelayCommand(_ => GroupConditions(), _ => CanGroupConditions());
        UngroupConditionsCommand = new RelayCommand(_ => UngroupConditions(), _ => CanUngroup());

        // Subscribe to condition changes to update the collection
        Condition.ConditionChanged += OnConditionChanged;
    }

    /// <summary>
    ///     Gets the collection of child conditions.
    /// </summary>
    public ObservableCollection<ICondition> ChildConditions
    {
        get => _childConditions;
        set => SetProperty(ref _childConditions, value);
    }

    /// <summary>
    ///     Gets the collection of selected conditions for grouping.
    /// </summary>
    public ObservableCollection<ICondition> SelectedConditions
    {
        get => _selectedConditions;
        set => SetProperty(ref _selectedConditions, value);
    }

    /// <summary>
    ///     Gets the indentation level for visual hierarchy.
    /// </summary>
    public int IndentationLevel
    {
        get => _indentationLevel;
        set => SetProperty(ref _indentationLevel, value);
    }

    /// <summary>
    ///     Gets the left margin based on indentation level.
    /// </summary>
    public Thickness IndentationMargin => new(IndentationLevel * 20, 0, 0, 0);

    /// <summary>
    ///     Gets the indentation level for nested child conditions (incremented by 1).
    /// </summary>
    public int NestedIndentationLevel => IndentationLevel + 1;

    /// <summary>
    ///     Gets whether this block has a parent (can be ungrouped).
    /// </summary>
    public bool HasParent => Condition.Parent != null;

    /// <summary>
    ///     Gets the query context.
    /// </summary>
    public IQuery Query { get; }

    /// <summary>
    ///     Gets the underlying block condition.
    /// </summary>
    public BlockCondition Condition { get; }

    /// <summary>
    ///     Gets the command to add a new condition.
    /// </summary>
    public ICommand AddConditionCommand { get; }

    /// <summary>
    ///     Gets the command to group selected conditions.
    /// </summary>
    public ICommand GroupConditionsCommand { get; }

    /// <summary>
    ///     Gets the command to ungroup conditions.
    /// </summary>
    public ICommand UngroupConditionsCommand { get; }

    private void AddCondition()
    {
        // Check if there are any existing conditions to copy from
        var existingCondition = Condition.Conditions.OfType<SimpleCondition>().FirstOrDefault();

        if (existingCondition != null)
        {
            // Use the parameterless CreateNew which copies from existing condition
            Condition.CreateNew();
        }
        else
        {
            // No existing conditions, need to create one with a property
            var firstProperty = Query.ConditionPropertyPaths.FirstOrDefault();
            if (firstProperty != null)
                Condition.CreateNew(firstProperty);
            else
                // No properties available - should not happen if query is properly configured
                throw new InvalidOperationException("Cannot add condition: No properties are available in the query configuration.");
        }

        RefreshChildConditions();
    }

    private bool CanGroupConditions()
    {
        return SelectedConditions.Count >= 2;
    }

    private void GroupConditions()
    {
        Condition.Group(SelectedConditions);
        SelectedConditions.Clear();
        RefreshChildConditions();
    }

    private bool CanUngroup()
    {
        return HasParent && SelectedConditions.Count > 0;
    }

    private void UngroupConditions()
    {
        Condition.Ungroup(SelectedConditions);
        SelectedConditions.Clear();
        RefreshChildConditions();
    }

    private void OnConditionChanged(object? sender, EventArgs e)
    {
        RefreshChildConditions();
    }

    private void RefreshChildConditions()
    {
        ChildConditions = new ObservableCollection<ICondition>(Condition.Conditions);
    }

    public void RemoveCondition(ICondition condition)
    {
        Condition.Remove(condition);
        RefreshChildConditions();
    }
}