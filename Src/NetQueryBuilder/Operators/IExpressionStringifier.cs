using System.Linq.Expressions;

namespace NetQueryBuilder.Operators
{
    /// <summary>
    ///     Defines a mechanism for converting LINQ expression types or operator names into their formatted string representations.
    /// </summary>
    public interface IExpressionStringifier
    {
        /// <summary>
        ///     Converts an expression type or operator name into a formatted string representation.
        /// </summary>
        /// <param name="expressionType">The type of the expression being converted.</param>
        /// <param name="name">The name of the operator to be formatted. If null or empty, the expression type is used instead.</param>
        /// <returns>A string representation of the expression or operator, formatted with upper case separation.</returns>
        string GetString(ExpressionType expressionType, string name);
    }
}