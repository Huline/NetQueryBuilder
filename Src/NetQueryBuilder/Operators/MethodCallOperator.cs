using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace NetQueryBuilder.Operators
{
    public abstract class MethodCallOperator : ExpressionOperator
    {
        protected MethodCallOperator(string name, IExpressionStringifier expressionStringifier, MethodInfo methodInfo, bool isNegated = false)
            : base(ExpressionType.Call, name, expressionStringifier)
        {
            MethodInfo = methodInfo ?? throw new ArgumentNullException(nameof(methodInfo));
            IsNegated = isNegated;
        }

        protected MethodInfo MethodInfo { get; }
        protected bool IsNegated { get; }

        public override bool Equals(object obj)
        {
            return obj != null
                   && obj is MethodCallOperator op
                   && base.Equals(obj)
                   && EqualityComparer<MethodInfo>.Default.Equals(MethodInfo, op.MethodInfo)
                   && IsNegated == op.IsNegated;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = base.GetHashCode();
                hashCode = (hashCode * 397) ^ (MethodInfo != null ? MethodInfo.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ IsNegated.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(MethodCallOperator left, MethodCallOperator right)
        {
            return EqualityComparer<MethodCallOperator>.Default.Equals(left, right);
        }

        public static bool operator !=(MethodCallOperator left, MethodCallOperator right)
        {
            return !(left == right);
        }
    }
}