using System.Linq.Expressions;
using System.Reflection;

namespace NetQueryBuilder.Operators;

public abstract class MethodCallOperator : ExpressionOperator
{
    public MethodCallOperator(string name, IExpressionStringifier expressionStringifier, MethodInfo methodInfo, bool isNegated = false)
        : base(ExpressionType.Call, name, expressionStringifier)
    {
        MethodInfo = methodInfo;
        IsNegated = isNegated;
    }

    public MethodInfo MethodInfo { get; }
    public bool IsNegated { get; }

    public override bool Equals(object? obj)
    {
        return obj is not null
               && obj is MethodCallOperator op
               && base.Equals(obj)
               && EqualityComparer<MethodInfo>.Default.Equals(MethodInfo, op.MethodInfo)
               && IsNegated == op.IsNegated;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), MethodInfo, IsNegated);
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