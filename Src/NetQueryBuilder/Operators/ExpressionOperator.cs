using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace NetQueryBuilder.Operators
{
    public abstract class ExpressionOperator
    {
        private readonly IExpressionStringifier _expressionStringifier;
        private readonly string _name;
        protected readonly ExpressionType ExpressionType;


        public ExpressionOperator(ExpressionType type, string name, IExpressionStringifier expressionStringifier)
        {
            _expressionStringifier = expressionStringifier;
            ExpressionType = type;
            _name = name;
        }

        public abstract Expression ToExpression(Expression left, Expression right);
        public abstract object? GetDefaultValue(Type type, object? value);

        public override bool Equals(object? obj)
        {
            return obj != null
                   && obj is ExpressionOperator op
                   && ExpressionType == op.ExpressionType;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ExpressionType);
        }

        public static bool operator ==(ExpressionOperator? left, ExpressionOperator? right)
        {
            return EqualityComparer<ExpressionOperator>.Default.Equals(left!, right!);
        }

        public static bool operator !=(ExpressionOperator? left, ExpressionOperator? right)
        {
            return !(left == right);
        }

        public override string ToString()
        {
            return _expressionStringifier.GetString(ExpressionType, _name);
        }
    }
}