using System.Linq.Expressions;

namespace NetQueryBuilder.Operators;

public interface IExpressionStringifier
{
    string GetString(ExpressionType expressionType, string name);
}