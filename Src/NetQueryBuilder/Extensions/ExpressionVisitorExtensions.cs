using System.Linq.Expressions;
using NetQueryBuilder.Visitors;

namespace NetQueryBuilder.Extensions;

public static class ExpressionVisitorExtensions
{
    public static Expression Copy(this Expression expression)
    {
        return new CopyExpression(expression).Execute();
    }
}