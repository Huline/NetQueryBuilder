using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;
using NetQueryBuilder.Conditions;

namespace NetQueryBuilder.Blazor.Components.Conditions.Relations;

public partial class RelationalValueMethod
{
    [Parameter] public SimpleCondition Condition { get; set; } = null!;
    private MethodCallExpression? _methodCallExpression;
    private Type? _memberType;

    protected override void OnParametersSet()
    {
        var expression = Condition.Compile();
        if (expression is not MethodCallExpression methodCallExpression)
        {
            _methodCallExpression = null;
            _memberType = null;
            return;
        }

        _methodCallExpression = methodCallExpression;
        _memberType = ((MemberExpression)_methodCallExpression.Arguments[1]).Type;
    }

    private string GetStringValue(Expression expression)
    {
        return expression switch
        {
            ConstantExpression constExpr => constExpr.Value?.ToString() ?? string.Empty,
            UnaryExpression { NodeType: ExpressionType.Convert, Operand: ConstantExpression constOperand } => constOperand.Value?.ToString() ?? string.Empty,
            _ => string.Empty
        };
    }

    private void UpdateValue<T>(T value)
    {
        Condition.Value = value;
    }
}