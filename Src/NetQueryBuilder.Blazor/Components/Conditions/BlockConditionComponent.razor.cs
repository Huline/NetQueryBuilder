using Microsoft.AspNetCore.Components;
using NetQueryBuilder.Conditions;

namespace NetQueryBuilder.Blazor.Components.Conditions;

public partial class BlockConditionComponent
{
    [Parameter] public BlockCondition Condition { get; set; } = null!;
    [Parameter] public int IndentationLevel { get; set; }
    private List<ICondition> SelectedConditions { get; } = new();
    private readonly IEnumerable<LogicalOperator> _operators = Enum.GetValues<LogicalOperator>();

    private void GroupConditions()
    {
        Condition.Group(SelectedConditions);
        SelectedConditions.Clear();
    }

    private void UnGroupConditions()
    {
        Condition.Ungroup(SelectedConditions);
        SelectedConditions.Clear();
    }

    private void Select(bool e, ICondition childCondition)
    {
        if (e)
            SelectedConditions.Add(childCondition);
        else
            SelectedConditions.Remove(childCondition);
    }

    private void AddPredicate()
    {
        Condition.CreateNew();
    }

    private void RemovePredicate(ICondition condition)
    {
        Condition.Remove(condition);
    }

    private void ChangeOperator(ICondition condition, LogicalOperator op)
    {
        condition.LogicalOperator = op;
    }
}