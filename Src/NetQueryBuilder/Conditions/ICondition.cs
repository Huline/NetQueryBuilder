using System;
using System.Linq.Expressions;

namespace NetQueryBuilder.Conditions
{
    public interface ICondition
    {
        public BlockCondition? Parent { get; set; }
        EventHandler? ConditionChanged { get; set; }
        LogicalOperator LogicalOperator { get; set; }
        ICondition GetRoot();
        Expression? Compile();
    }
}