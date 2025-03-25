using System.Collections;
using System.Linq.Expressions;
using NetQueryBuilder.Conditions;

namespace NetQueryBuilder.Queries;

public interface IQuery
{
    EventHandler? OnChanged { get; set; }
    IReadOnlyCollection<SelectPropertyPath> SelectPropertyPaths { get; }
    IReadOnlyCollection<PropertyPath> ConditionPropertyPaths { get; }
    IReadOnlyCollection<ICondition> Conditions { get; }
    LambdaExpression Compile();
    Task<IEnumerable> Execute();
}