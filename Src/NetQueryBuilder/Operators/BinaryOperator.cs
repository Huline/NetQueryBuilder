using System;
using System.Linq.Expressions;
using NetQueryBuilder.Utils;

namespace NetQueryBuilder.Operators
{
    public abstract class BinaryOperator : ExpressionOperator
    {
        protected BinaryOperator(ExpressionType type, string name, IExpressionStringifier expressionStringifier)
            : base(type, name, expressionStringifier)
        {
        }

        public override Expression ToExpression(Expression left, Expression right)
        {
            return Expression.MakeBinary(ExpressionType, left, right);
        }

        public override object? GetDefaultValue(Type type, object? value)
        {
            if (type == value?.GetType()) return value;
            return type.GetDefaultValue();
        }
    }
}