using Microsoft.AspNetCore.Components;
using NetQueryBuilder.Conditions;
using NetQueryBuilder.Operators;
using NetQueryBuilder.Queries;

namespace NetQueryBuilder.Blazor.Components.Conditions.Relations;

public partial class RelationalOperators
{
    private IEnumerable<ExpressionOperator> _operators = new List<ExpressionOperator>();

    [CascadingParameter] public IQuery Query { get; set; } = null!;

    [Parameter] public SimpleCondition Condition { get; set; } = null!;

    private ExpressionOperator? Operator { get; set; }

    protected override void OnParametersSet()
    {
        _operators = Condition.AvailableOperatorsForCurrentProperty();
        Operator = Condition.Operator;
    }

    private void OperatorChanged(ExpressionOperator? op)
    {
        Operator = op;
        Condition.Operator = op;
    }
}