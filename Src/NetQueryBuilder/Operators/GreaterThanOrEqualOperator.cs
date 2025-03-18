using System.Linq.Expressions;

namespace NetQueryBuilder.Operators;

public class GreaterThanOrEqualOperator : BinaryOperator
{
    public GreaterThanOrEqualOperator(IExpressionStringifier expressionStringifier)
        : base(ExpressionType.GreaterThanOrEqual, "GreaterThanOrEqual", expressionStringifier)
    {
    }
}