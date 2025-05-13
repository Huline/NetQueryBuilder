using System;

namespace NetQueryBuilder.Properties
{
    public interface IPropertyStringifier
    {
        /// <summary>
        ///     Retrieves the name of a property based on its provided identifier or representation.
        /// </summary>
        /// <param name="propertyName">The identifier or representation of the property whose name is to be retrieved.</param>
        /// <returns>A string representing the name of the property.</returns>
        string GetName(string propertyName);

        /// <summary>
        ///     Formats the value of a property based on its name, type, and the provided object value.
        /// </summary>
        /// <param name="propertyName">The identifier or name of the property whose value is to be formatted.</param>
        /// <param name="type">The data type of the property being formatted.</param>
        /// <param name="value">The value of the property to be formatted.</param>
        /// <returns>A string representing the formatted value of the property.</returns>
        string FormatValue(string propertyName, Type type, object value);
    }
}