using System.Collections.Generic;
using NetQueryBuilder.Properties;

namespace NetQueryBuilder.Operators
{
    /// <summary>
    ///     Represents a factory for retrieving applicable <see cref="ExpressionOperator" /> instances
    ///     based on a specified property path. Implementations of this interface provide logic
    ///     to determine which operators are compatible with a given property.
    /// </summary>
    public interface IOperatorFactory
    {
        /// <summary>
        ///     Retrieves a collection of <see cref="ExpressionOperator" /> objects that are applicable
        ///     to the specified property represented by the given <see cref="PropertyPath" />.
        /// </summary>
        /// <param name="propertyPath">The path representing the target property for which operators will be retrieved.</param>
        /// <returns>A collection of <see cref="ExpressionOperator" /> objects compatible with the specified property.</returns>
        IEnumerable<ExpressionOperator> GetAllForProperty(PropertyPath propertyPath);
    }
}