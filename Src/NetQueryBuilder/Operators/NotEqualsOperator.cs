using System.Linq.Expressions;

namespace NetQueryBuilder.Operators
{
    public class NotEqualsOperator : BinaryOperator
    {
        public NotEqualsOperator(IExpressionStringifier expressionStringifier)
            : base(ExpressionType.NotEqual, "NotEquals", expressionStringifier)
        {
        }
    }
}