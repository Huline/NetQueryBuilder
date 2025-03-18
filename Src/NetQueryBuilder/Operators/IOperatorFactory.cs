namespace NetQueryBuilder.Operators;

public interface IOperatorFactory
{
    IEnumerable<ExpressionOperator> GetAllForProperty(PropertyPath propertyPath);
}

public class DefaultOperatorFactory : IOperatorFactory
{
    private readonly IExpressionStringifier _expressionStringifier;

    public DefaultOperatorFactory(IExpressionStringifier expressionStringifier)
    {
        _expressionStringifier = expressionStringifier;
    }

    public virtual IEnumerable<ExpressionOperator> GetAllForProperty(PropertyPath propertyPath)
    {
        return propertyPath.PropertyType switch
        {
            Type type when type == typeof(int) => new List<ExpressionOperator>
            {
                new EqualsOperator(_expressionStringifier),
                new NotEqualsOperator(_expressionStringifier),
                new LessThanOperator(_expressionStringifier),
                new LessThanOrEqualOperator(_expressionStringifier),
                new GreaterThanOperator(_expressionStringifier),
                new GreaterThanOrEqualOperator(_expressionStringifier),
                new InListOperator<int>(_expressionStringifier),
                new InListOperator<int>(_expressionStringifier, true)
            },
            Type type when type == typeof(string) => new List<ExpressionOperator>
            {
                new EqualsOperator(_expressionStringifier),
                new NotEqualsOperator(_expressionStringifier),
                new InListOperator<string>(_expressionStringifier),
                new InListOperator<string>(_expressionStringifier, true)
            },
            Type type when type == typeof(bool) => new List<ExpressionOperator>
            {
                new EqualsOperator(_expressionStringifier),
                new NotEqualsOperator(_expressionStringifier)
            },
            Type type when type == typeof(DateTime) => new List<ExpressionOperator>
            {
                new EqualsOperator(_expressionStringifier),
                new NotEqualsOperator(_expressionStringifier),
                new LessThanOperator(_expressionStringifier),
                new LessThanOrEqualOperator(_expressionStringifier),
                new GreaterThanOperator(_expressionStringifier),
                new GreaterThanOrEqualOperator(_expressionStringifier)
            },
            _ => new List<ExpressionOperator>
            {
                new EqualsOperator(_expressionStringifier),
                new NotEqualsOperator(_expressionStringifier)
            }
        };
    }
}