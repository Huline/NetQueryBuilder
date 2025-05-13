using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using NetQueryBuilder.Conditions;
using NetQueryBuilder.Properties;

namespace NetQueryBuilder.Queries
{
    public interface IQuery
    {
        /// A property triggered when the state of the query changes.
        /// It allows subscription to event handlers to listen for modifications
        /// or updates in the query's configuration or execution state.
        EventHandler OnChanged { get; set; }

        /// A collection of property paths that are selected for the query.
        IReadOnlyCollection<SelectPropertyPath> SelectPropertyPaths { get; }

        /// A collection of property paths that are used in the conditions of the query.
        IReadOnlyCollection<PropertyPath> ConditionPropertyPaths { get; }

        /// Represents the root condition for the query.
        /// It defines the logical structure and criteria for filtering data within the query.
        /// This property allows construction and manipulation of conditions that dictate
        /// how data is matched and retrieved based on specified attributes and logical operators.
        BlockCondition Condition { get; }

        /// Compiles the condition into a lambda expression usable for query execution.
        /// <returns>A lambda expression representing the compiled query condition. Returns null if the condition is not set.</returns>
        LambdaExpression Compile();

        /// Executes the query and retrieves a paginated result set based on the provided page size.
        /// <param name="pageSize">The number of items per page in the result set.</param>
        /// <returns>A task that represents the asynchronous operation, which, when completed, contains the query result of dynamic type.</returns>
        Task<QueryResult<dynamic>> Execute(int pageSize);

        /// Executes the query and retrieves a page of results with the specified page size.
        /// <param name="pageSize">The number of items to include in each page of the query results.</param>
        /// <returns>
        ///     A task representing the asynchronous operation. The task's result contains a <see cref="QueryResult{dynamic}" /> object
        ///     with the retrieved data, total number of items, and pagination details.
        /// </returns>
        Task<QueryResult<TProjection>> Execute<TProjection>(int pageSize);
    }
}