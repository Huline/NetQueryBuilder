using System.Collections;
using System.Linq.Expressions;
using NetQueryBuilder.Conditions;

namespace NetQueryBuilder.Queries;

public interface IQuery
{
    EventHandler? OnChanged { get; set; }
    IReadOnlyCollection<SelectPropertyPath> SelectPropertyPaths { get; }
    IReadOnlyCollection<PropertyPath> ConditionPropertyPaths { get; }
    BlockCondition Condition { get; }
    LambdaExpression? Compile();
    Task<IEnumerable> Execute();
}