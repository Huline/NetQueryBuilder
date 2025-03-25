using System.Collections;
using System.Linq.Expressions;
using NetQueryBuilder.Conditions;

namespace NetQueryBuilder.Queries;

public interface IQuery
{
    EventHandler? OnChanged { get; set; }
    IEnumerable<PropertyPath> SelectedPropertyPaths { get; set; }
    IReadOnlyCollection<ICondition> Conditions { get; }

    IEnumerable<PropertyPath> AvailableProperties();
    LambdaExpression Compile();
    Task<IEnumerable> Execute(IEnumerable<PropertyPath>? selectedProperties);
}