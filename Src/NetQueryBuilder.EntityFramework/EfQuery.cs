using System.Collections;
using Microsoft.EntityFrameworkCore;
using NetQueryBuilder.Configurations;
using NetQueryBuilder.Operators;
using NetQueryBuilder.Queries;

namespace NetQueryBuilder.EntityFramework;

public class EfQuery<T> : Query<T> where T : class
{
    private readonly DbContext _dbContext;

    public EfQuery(DbContext context, SelectConfiguration selectConfiguration, ConditionConfiguration conditionConfiguration, IOperatorFactory operatorFactory)
        : base(selectConfiguration, conditionConfiguration, operatorFactory)
    {
        _dbContext = context;
    }

    protected override IQueryable<T> GetQueryable(IReadOnlyCollection<string> selectedProperties)
    {
        var query = _dbContext.Set<T>().AsQueryable();
        var navigationPaths = ExtractNavigationPaths(selectedProperties);
        foreach (var path in navigationPaths) query = query.Include(path);
        return query;
    }

    protected override async Task<IEnumerable> ToList(IQueryable<T> queryable)
    {
        return await queryable.ToListAsync();
    }

    private static HashSet<string> ExtractNavigationPaths(IEnumerable<string> selectedProperties)
    {
        var paths = new HashSet<string>();

        foreach (var propPath in selectedProperties)
            if (propPath.Contains("."))
                paths.Add(propPath.Substring(0, propPath.LastIndexOf('.')));

        return paths;
    }
}