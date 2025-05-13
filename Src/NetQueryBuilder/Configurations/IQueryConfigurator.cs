using System;
using System.Collections.Generic;
using NetQueryBuilder.Operators;
using NetQueryBuilder.Queries;

namespace NetQueryBuilder.Configurations
{
    /// <summary>
    ///     Represents a configurator for building and customizing queries within the system.
    /// </summary>
    public interface IQueryConfigurator
    {
        /// <summary>
        ///     Retrieves a collection of entity types supported by the query configurator.
        /// </summary>
        /// <returns>A collection of <see cref="Type" /> representing the entity types.</returns>
        IEnumerable<Type> GetEntities();

        /// <summary>
        ///     Updates the expression stringifier used by the query configurator.
        /// </summary>
        /// <param name="expressionStringifier">The <see cref="IExpressionStringifier" /> to be used for stringifying expressions.</param>
        /// <returns>The current instance of <see cref="IQueryConfigurator" /> for method chaining.</returns>
        IQueryConfigurator UseExpressionStringifier(IExpressionStringifier expressionStringifier);

        /// <summary>
        ///     Configures the select functionality by applying the specified select builder to the query configurator.
        /// </summary>
        /// <param name="selectBuilder">An action that defines custom configuration for the <see cref="ISelectConfigurator" />.</param>
        /// <returns>The current instance of <see cref="IQueryConfigurator" /> for method chaining.</returns>
        IQueryConfigurator ConfigureSelect(Action<ISelectConfigurator> selectBuilder);

        /// <summary>
        ///     Configures the condition-building process for the query by using the specified condition configuration logic.
        /// </summary>
        /// <param name="selectBuilder">An action that defines the configuration logic for condition-building, using an instance of <see cref="IConditionConfigurator" />.</param>
        /// <returns>The current instance of <see cref="IQueryConfigurator" /> for method chaining.</returns>
        IQueryConfigurator ConfigureConditions(Action<IConditionConfigurator> selectBuilder);

        /// <summary>
        ///     Constructs a query object for the specified type.
        /// </summary>
        /// <typeparam name="T">The entity type for which the query is being built. Must be a class.</typeparam>
        /// <returns>An instance of <see cref="IQuery" /> configured for the specified entity type.</returns>
        IQuery BuildFor<T>() where T : class;

        /// <summary>
        ///     Builds a query instance for the specified entity type.
        /// </summary>
        /// <param name="type">The <see cref="Type" /> of the entity for which the query will be constructed.</param>
        /// <returns>An instance of <see cref="IQuery" /> representing the query for the specified entity type.</returns>
        IQuery BuildFor(Type type);
    }
}