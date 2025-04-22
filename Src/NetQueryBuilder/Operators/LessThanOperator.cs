using System.Linq.Expressions;

namespace NetQueryBuilder.Operators
{
    public class LessThanOperator : BinaryOperator
    {
        public LessThanOperator(IExpressionStringifier expressionStringifier)
            : base(ExpressionType.LessThan, "LessThan", expressionStringifier)
        {
        }
    }
}