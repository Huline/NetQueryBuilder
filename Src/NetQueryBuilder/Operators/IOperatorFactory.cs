using System.Collections.Generic;

namespace NetQueryBuilder.Operators
{
    public interface IOperatorFactory
    {
        IEnumerable<ExpressionOperator> GetAllForProperty(PropertyPath propertyPath);
    }
}