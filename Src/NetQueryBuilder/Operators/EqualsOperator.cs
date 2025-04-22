using System.Linq.Expressions;

namespace NetQueryBuilder.Operators
{
    public class EqualsOperator : BinaryOperator
    {
        public EqualsOperator(IExpressionStringifier expressionStringifier)
            : base(ExpressionType.Equal, "Equals", expressionStringifier)
        {
        }
    }
}