using NetQueryBuilder.EntityFramework.Operators;
using NetQueryBuilder.Operators;
using NetQueryBuilder.Properties;

namespace NetQueryBuilder.EntityFramework;

public class EfOperatorFactory : DefaultOperatorFactory
{
    private readonly IExpressionStringifier _expressionStringifier;

    public EfOperatorFactory(IExpressionStringifier expressionStringifier) : base(expressionStringifier)
    {
        _expressionStringifier = expressionStringifier;
    }

    public override IEnumerable<ExpressionOperator> GetAllForProperty(PropertyPath propertyPath)
    {
        var result = base.GetAllForProperty(propertyPath);
        if (propertyPath.PropertyType != typeof(string))
            return result;
        return result.Concat(new List<ExpressionOperator>
            {
                new EfLikeOperator(_expressionStringifier),
                new EfLikeOperator(_expressionStringifier, true)
            }
        );
    }
}