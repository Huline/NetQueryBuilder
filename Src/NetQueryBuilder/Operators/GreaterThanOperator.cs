using System.Linq.Expressions;

namespace NetQueryBuilder.Operators;

public class GreaterThanOperator : BinaryOperator
{
    public GreaterThanOperator(IExpressionStringifier expressionStringifier)
        : base(ExpressionType.GreaterThan, "GreaterThan", expressionStringifier)
    {
    }
}