using System;
using System.Linq.Expressions;

namespace NetQueryBuilder.Conditions
{
    /// <summary>
    ///     Represents a condition component that is part of a condition hierarchy.
    ///     It provides functionality to retrieve the root condition, track changes,
    ///     and compile conditions into LINQ expressions.
    /// </summary>
    public interface ICondition
    {
        /// Gets or sets the parent `BlockCondition` of the current condition.
        /// This property establishes a hierarchical relationship between conditions,
        /// allowing them to be organized into nested structures. Each condition
        /// may have a parent `BlockCondition`, which helps maintain and manage the
        /// overall logical grouping of conditions.
        BlockCondition Parent { get; set; }

        /// Gets or sets the event handler that is triggered whenever the condition changes.
        /// This property allows subscribers to be notified when a condition within the system
        /// is modified, enabling real-time updates and synchronization of dependent components.
        EventHandler ConditionChanged { get; set; }

        /// Gets or sets the logical operator (`And` or `Or`) that defines the logical
        /// relationship for the current condition. This property specifies how the
        /// condition interacts with other conditions in terms of logical expressions,
        /// influencing how the conditions are evaluated or combined in queries.
        LogicalOperator LogicalOperator { get; set; }

        /// <summary>
        ///     Retrieves the root condition in the condition hierarchy.
        /// </summary>
        /// <returns>
        ///     The top-level <see cref="ICondition" /> in the condition hierarchy.
        ///     If the current condition has no parent, it returns itself as the root.
        /// </returns>
        ICondition GetRoot();

        /// <summary>
        ///     Compiles the condition into a LINQ Expression.
        /// </summary>
        /// <returns>
        ///     An <see cref="Expression" /> that represents the condition.
        ///     If no valid compilation is possible, returns null.
        /// </returns>
        Expression Compile();
    }
}