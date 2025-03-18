using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using NetQueryBuilder.Operators;

namespace NetQueryBuilder.EntityFramework.Operators;

public class EfLikeOperator : MethodCallOperator
{
    public EfLikeOperator(bool isNegated = false)
        : base(typeof(DbFunctionsExtensions).GetMethod("Like", new[] { typeof(DbFunctions), typeof(string), typeof(string) }), isNegated)
    {
    }

    public override string DisplayText => IsNegated ? "Not like" : "Like";

    public override Expression ToExpression(Expression left, Expression right)
    {
        return IsNegated
            ? Expression.Not(GetExpression(left, right))
            : GetExpression(left, right);
    }

    public override object? GetDefaultValue(Type type)
    {
        return string.Empty;
    }

    private MethodCallExpression GetExpression(Expression left, Expression right)
    {
        if (right.Type != typeof(string)) right = Expression.Convert(right, typeof(string));

        var efFunctionsProperty = typeof(EF).GetProperty("Functions");
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