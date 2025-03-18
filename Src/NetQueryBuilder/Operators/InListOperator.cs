using System.Collections;
using System.Linq.Expressions;
using NetQueryBuilder.Util;

namespace NetQueryBuilder.Operators;

public class InListOperator<T> : MethodCallOperator
{
    public InListOperator(bool isNegated = false)
        : base(EnumerableMethodInfo.Contains<T>(), isNegated)
    {
    }

    public override string DisplayText => IsNegated ? "Not in list" : "In list";

    public override Expression ToExpression(Expression left, Expression right)
    {
        return IsNegated
            ? Expression.Not(GetExpression(left, right))
            : GetExpression(left, right);
    }

    public override object? GetDefaultValue(Type type)
    {
        var listType = typeof(List<>).MakeGenericType(type);
        return Activator.CreateInstance(listType);
    }

    private MethodCallExpression GetExpression(Expression left, Expression right)
    {
        if (left is MemberExpression memberExpression)
        {
            // Créer un type de liste pour le type de membre correct
            // Si right est une constante contenant une liste, utiliser cette liste
            if (right is ConstantExpression constantExpression &&
                constantExpression.Value is IEnumerable enumerable)
                return Expression.Call(
                    null,
                    MethodInfo,
                    constantExpression,
                    memberExpression);

            // Sinon, créer une liste vide du type approprié
            var listType = typeof(List<>).MakeGenericType(memberExpression.Type);
            var emptyList = Activator.CreateInstance(listType);
            var constantExp = Expression.Constant(emptyList);

            return Expression.Call(
                null,
                MethodInfo,
                constantExp,
                memberExpression);
        }

        return Expression.Call(
            null,
            MethodInfo,
            left,
            right);
    }
}