using System.Linq.Expressions;

namespace NetQueryBuilder.Operators;

public abstract class ExpressionOperator
{
    public virtual ExpressionType ExpressionType { get; }
    public virtual string DisplayText { get; }

    public abstract Expression ToExpression(Expression left, Expression right);

    public override bool Equals(object? obj)
    {
        return obj is not null
               && obj is ExpressionOperator op
               && ExpressionType == op.ExpressionType;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(ExpressionType);
    }

    public static bool operator ==(ExpressionOperator left, ExpressionOperator right)
    {
        return EqualityComparer<ExpressionOperator>.Default.Equals(left, right);
    }

    public static bool operator !=(ExpressionOperator left, ExpressionOperator right)
    {
        return !(left == right);
    }

    public abstract object? GetDefaultValue(Type type);
}