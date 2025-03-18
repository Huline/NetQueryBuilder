using System.Linq.Expressions;
using NetQueryBuilder.Extensions;

namespace NetQueryBuilder.Operators;

public class BinaryOperator : ExpressionOperator
{
    public override Expression ToExpression(Expression left, Expression right)
    {
        return Expression.MakeBinary(ExpressionType, left, right);
    }

    public override object? GetDefaultValue(Type type)
    {
        return type.GetDefaultValue();
    }
}