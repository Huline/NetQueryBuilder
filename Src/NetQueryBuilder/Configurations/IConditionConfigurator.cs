using System;
using NetQueryBuilder.Properties;

namespace NetQueryBuilder.Configurations
{
    /// <summary>
    ///     Provides configuration options for defining and customizing conditions in queries.
    ///     This interface allows manipulation of fields, nested condition depth, and stringification logic for property names and values.
    /// </summary>
    public interface IConditionConfigurator
    {
        /// <summary>
        ///     Restricts the condition configuration to a specified set of fields.
        /// </summary>
        /// <param name="fields">A list of field names to which the conditions should be limited.</param>
        /// <returns>An instance of <see cref="IConditionConfigurator" /> for further configuration.</returns>
        IConditionConfigurator LimitToFields(params string[] fields);

        /// <summary>
        ///     Removes a specified set of fields from the condition configuration.
        /// </summary>
        /// <param name="fields">A list of field names to be excluded from the condition configuration.</param>
        /// <returns>An instance of <see cref="IConditionConfigurator" /> for further configuration.</returns>
        IConditionConfigurator RemoveFields(params string[] fields);

        /// <summary>
        ///     Specifies a custom stringifier to determine how property names and values are formatted during condition configuration.
        /// </summary>
        /// <param name="propertyStringifier">An instance of <see cref="IPropertyStringifier" /> that defines the logic for formatting property names and values.</param>
        /// <returns>An instance of <see cref="IConditionConfigurator" /> for further configuration.</returns>
        IConditionConfigurator UseStringifier(IPropertyStringifier propertyStringifier);

        /// <summary>
        ///     Sets a limit on the depth of nested conditions during configuration.
        /// </summary>
        /// <param name="depth">The maximum allowed depth for nested conditions.</param>
        /// <returns>An instance of <see cref="IConditionConfigurator" /> for further configuration.</returns>
        IConditionConfigurator LimitDepth(int depth);

        /// <summary>
        ///     Excludes specific relationship types from the condition configuration.
        /// </summary>
        /// <param name="relationships">An array of <see cref="Type" /> objects representing the relationship types to be excluded.</param>
        /// <returns>An instance of <see cref="IConditionConfigurator" /> for further configuration.</returns>
        IConditionConfigurator ExcludeRelationships(params Type[] relationships);
    }
}