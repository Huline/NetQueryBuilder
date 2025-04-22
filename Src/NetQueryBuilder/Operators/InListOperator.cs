using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using NetQueryBuilder.Utils;

namespace NetQueryBuilder.Operators
{
    public class InListOperator<T> : MethodCallOperator
    {
        public InListOperator(IExpressionStringifier expressionStringifier, bool isNegated = false)
            : base(isNegated ? "NotInList" : "InList", expressionStringifier, EnumerableMethodInfo.Contains<T>(), isNegated)
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
            var listType = typeof(List<>).MakeGenericType(type);
            if (value?.GetType() == listType)
                return value;
            return Activator.CreateInstance(listType);
        }

        private Expression GetExpression(Expression left, Expression right)
        {
            if (left.Type.IsAssignableFrom(typeof(MemberExpression)))
                return Expression.Call(
                    null,
                    MethodInfo,
                    left,
                    right);

            if (right is ConstantExpression constantExpression &&
                constantExpression.Value is IEnumerable)
                return Expression.Call(
                    null,
                    MethodInfo,
                    constantExpression,
                    left);

            var listType = typeof(List<>).MakeGenericType(left.Type);
            var emptyList = Activator.CreateInstance(listType);
            var constantExp = Expression.Constant(emptyList);

            return Expression.Call(
                null,
                MethodInfo,
                constantExp,
                left);
        }
    }
}