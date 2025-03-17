using System.Linq.Expressions;

namespace NetQueryBuilder.Conditions;

public interface ICondition
{
    internal ICondition? Parent { get; set; }
    EventHandler ConditionChanged { get; set; }
    ExpressionType LogicalType { get; set; }
    ICondition GetRoot();
    Expression Compile();
}