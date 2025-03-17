using NetQueryBuilder.Conditions;
using NetQueryBuilder.EntityFramework.Operators;
using NetQueryBuilder.Operators;

namespace NetQueryBuilder.EntityFramework;

public class EFOperatorFactory : DefaultOperatorFactory
{
    public override IEnumerable<ExpressionOperator> GetAllForProperty(PropertyPath propertyPath)
    {
        var result = base.GetAllForProperty(propertyPath);
        if (propertyPath.PropertyType != typeof(string))
            return result;
        return result.Concat([
            new EfLikeOperator(),
            new EfLikeOperator(true)
        ]);
    }
}