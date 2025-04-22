using System.Linq.Expressions;

namespace NetQueryBuilder.Operators
{
    public class LessThanOrEqualOperator : BinaryOperator
    {
        public LessThanOrEqualOperator(IExpressionStringifier expressionStringifier)
            : base(ExpressionType.LessThanOrEqual, "LessThanOrEqual", expressionStringifier)
        {
        }
    }
}