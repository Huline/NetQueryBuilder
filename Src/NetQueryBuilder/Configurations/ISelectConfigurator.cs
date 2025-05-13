using System;
using NetQueryBuilder.Properties;

namespace NetQueryBuilder.Configurations
{
    /// <summary>
    ///     Defines the configuration options for selecting and customizing data retrieval in a query.
    /// </summary>
    public interface ISelectConfigurator
    {
        /// <summary>
        ///     Restricts the selection to the specified fields.
        /// </summary>
        /// <param name="fields">An array of field names to include in the selection.</param>
        /// <returns>An instance of <see cref="ISelectConfigurator" /> with the updated configuration for field selection.</returns>
        ISelectConfigurator LimitToFields(params string[] fields);

        /// <summary>
        ///     Excludes the specified fields from the selection.
        /// </summary>
        /// <param name="fields">An array of field names to exclude from the selection.</param>
        /// <returns>An instance of <see cref="ISelectConfigurator" /> with the updated configuration for field exclusion.</returns>
        ISelectConfigurator RemoveFields(params string[] fields);

        /// <summary>
        ///     Configures the stringifier to be used for property name and value transformations.
        /// </summary>
        /// <param name="propertyStringifier">An instance of <see cref="IPropertyStringifier" /> that defines the rules for property stringification.</param>
        /// <returns>An instance of <see cref="ISelectConfigurator" /> with the updated configuration for using the specified stringifier.</returns>
        ISelectConfigurator UseStringifier(IPropertyStringifier propertyStringifier);

        /// <summary>
        ///     Limits the depth of relationships included in the selection.
        /// </summary>
        /// <param name="depth">The maximum depth of relationships to include in the selection.</param>
        /// <returns>An instance of <see cref="ISelectConfigurator" /> with the updated configuration for depth limitation.</returns>
        ISelectConfigurator LimitDepth(int depth);

        /// <summary>
        ///     Excludes the specified relationships from the selection.
        /// </summary>
        /// <param name="relationships">An array of relationship types to exclude from the selection.</param>
        /// <returns>An instance of <see cref="ISelectConfigurator" /> with the updated configuration for relationship exclusion.</returns>
        ISelectConfigurator ExcludeRelationships(params Type[] relationships);
    }
}