using System.Collections;
using System.Linq.Expressions;
using NetQueryBuilder.Conditions;

namespace NetQueryBuilder.Queries;

public interface IQuery
{
    BlockCondition Condition { get; }
    LambdaExpression Lambda { get; }
    ParameterExpression Parameter { get; }
    IEnumerable<PropertyPath> SelectedPropertyPaths { get; set; }

    IEnumerable<PropertyPath> AvailableProperties();
    void Compile();
    Task<IEnumerable> Execute(IEnumerable<PropertyPath>? selectedProperties);
}

public interface IQuery<TEntity> : IQuery where TEntity : class
{
}