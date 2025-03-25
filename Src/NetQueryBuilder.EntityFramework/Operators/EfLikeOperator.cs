using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using NetQueryBuilder.Operators;

namespace NetQueryBuilder.EntityFramework.Operators;

public class EfLikeOperator : MethodCallOperator
{
    public EfLikeOperator(IExpressionStringifier expressionStringifier, bool isNegated = false)
        : base(isNegated ? "NotLike" : "Like", expressionStringifier, typeof(DbFunctionsExtensions).GetMethod("Like", [typeof(DbFunctions), typeof(string), typeof(string)]), isNegated)
    {
    }

    public override Expression ToExpression(Expression left, Expression right)
    {
        return IsNegated
            ? Expression.Not(GetExpression(left, right))
            : GetExpression(left, right);
    }

    public override object? GetDefaultValue(Type type, object? value)
    {
        return value is string ? value : string.Empty;
    }

    private MethodCallExpression GetExpression(Expression left, Expression right)
    {
        if (right.Type != typeof(string)) right = Expression.Convert(right, typeof(string));

        var efFunctionsProperty = typeof(EF).GetProperty("Functions");
        if (efFunctionsProperty == null)
            throw new InvalidOperationException("EF.Functions property not found");
        var efFunctionsExpression = Expression.Property(null, efFunctionsProperty);

        if (left is MemberExpression memberExpression)
            return Expression.Call(
                null,
                MethodInfo,
                efFunctionsExpression,
                memberExpression,
                right);

        return Expression.Call(MethodInfo,
            efFunctionsExpression,
            left,
            right);
    }
}