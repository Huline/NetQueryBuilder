using System.Linq.Expressions;
using NetQueryBuilder.Conditions;

namespace NetQueryBuilder.Operators;

public interface IOperatorFactory
{
    ExpressionOperator FromExpression(ExpressionType expressionType);

    IEnumerable<ExpressionOperator> GetAllForProperty(PropertyPath propertyPath);
}

public class DefaultOperatorFactory : IOperatorFactory
{
    public virtual ExpressionOperator FromExpression(ExpressionType expressionType)
    {
        return expressionType switch
        {
            ExpressionType.Equal => new EqualsOperator(),
            ExpressionType.NotEqual => new NotEqualsOperator(),
            ExpressionType.LessThan => new LessThanOperator(),
            ExpressionType.LessThanOrEqual => new LessThanOrEqualOperator(),
            ExpressionType.GreaterThan => new GreaterThanOperator(),
            ExpressionType.GreaterThanOrEqual => new GreaterThanOrEqualOperator(),
            _ => throw new NotSupportedException()
        };
    }

    public virtual IEnumerable<ExpressionOperator> GetAllForProperty(PropertyPath propertyPath)
    {
        return propertyPath.PropertyType switch
        {
            Type type when type == typeof(int) => new List<ExpressionOperator>
            {
                new EqualsOperator(),
                new NotEqualsOperator(),
                new LessThanOperator(),
                new LessThanOrEqualOperator(),
                new GreaterThanOperator(),
                new GreaterThanOrEqualOperator(),
                new InListOperator<int>(),
                new InListOperator<int>(true)
            },
            Type type when type == typeof(string) => new List<ExpressionOperator>
            {
                new EqualsOperator(),
                new NotEqualsOperator(),
                // new EfLikeOperator(),
                // new EfLikeOperator(true),
                new InListOperator<string>(),
                new InListOperator<string>(true)
            },
            Type type when type == typeof(bool) => new List<ExpressionOperator>
            {
                new EqualsOperator(),
                new NotEqualsOperator()
            },
            Type type when type == typeof(DateTime) => new List<ExpressionOperator>
            {
                new EqualsOperator(),
                new NotEqualsOperator(),
                new LessThanOperator(),
                new LessThanOrEqualOperator(),
                new GreaterThanOperator(),
                new GreaterThanOrEqualOperator()
            },
            _ => new List<ExpressionOperator>
            {
                new EqualsOperator(),
                new NotEqualsOperator()
            }
        };
    }
}