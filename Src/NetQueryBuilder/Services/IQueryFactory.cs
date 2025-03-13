using NetQueryBuilder.Conditions;

namespace NetQueryBuilder.Services;

public interface IQueryFactory
{
    Task<IEnumerable<Type>> GetEntities();
    Query<T> Create<T>() where T : class;
}