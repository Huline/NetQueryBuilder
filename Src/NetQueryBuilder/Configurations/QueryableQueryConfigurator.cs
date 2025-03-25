using NetQueryBuilder.Queries;

namespace NetQueryBuilder.Configurations;

public class QueryableQueryConfigurator<TEntity> : IQueryConfigurator
{
    private readonly IQueryable<TEntity> _queryable;

    public QueryableQueryConfigurator(IQueryable<TEntity> queryable)
    {
        _queryable = queryable;
    }

    public IEnumerable<Type> GetEntities()
    {
        return [typeof(TEntity)];
    }

    public IQueryConfigurator ConfigureSelect(Action<ISelectConfigurator> selectBuilder)
    {
        return this;
    }

    public IQueryConfigurator ConfigureConditions(Action<IConditionConfigurator> selectBuilder)
    {
        return this;
    }

    public IQuery BuildFor<T>() where T : class
    {
        throw new NotImplementedException();
    }

    public IQuery BuildFor(Type type)
    {
        throw new NotImplementedException();
    }
}