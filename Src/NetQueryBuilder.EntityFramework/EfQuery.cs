using System.Collections;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using NetQueryBuilder.Operators;
using NetQueryBuilder.Queries;

namespace NetQueryBuilder.EntityFramework;

public class EfQuery<T> : Query<T> where T : class
{
    private readonly DbContext _dbContext;

    public EfQuery(DbContext context, IOperatorFactory operatorFactory)
        : base(operatorFactory)
    {
        _dbContext = context;
    }

    public override async Task<IEnumerable> Execute(IEnumerable<PropertyPath>? selectedProperties)
    {
        Expression<Func<T, bool>>? predicate = Lambda as Expression<Func<T, bool>>;

        return await QueryData(predicate, selectedProperties?.Select(p => p.PropertyName).ToList() ?? new List<string>());
    }

    public async Task<IEnumerable> QueryData(
        Expression<Func<T, bool>>? predicate,
        IReadOnlyCollection<string> selectedProperties)
    {
        IQueryable<T> query = _dbContext.Set<T>().AsQueryable();
        var navigationPaths = ExtractNavigationPaths(selectedProperties);
        foreach (var path in navigationPaths) query = query.Include(path);

        if (predicate != null)
            query = query.Where(predicate);
        if (selectedProperties.Count > 0)
        {
            var select = new SelectBuilderService<T>().BuildSelect(selectedProperties);
            query = query.Select(select);
        }

        return await query.ToListAsync();
    }

    private IEnumerable<string> ExtractNavigationPaths(IEnumerable<string> selectedProperties)
    {
        var paths = new HashSet<string>();

        foreach (var propPath in selectedProperties)
            // "Address.City" peut être incluse directement => "Address.City"
            // ou on pourrait vouloir juste "Address"
            // EF supporte la notation "Address.City", donc on peut l’ajouter tel quel.
            // Mais si vous voulez gérer ThenInclude, il faudra une logique plus complexe.
            if (propPath.Contains("."))
                // On récupère avant-dernier segment pour un ThenInclude
                // Pour simplifier l’exemple, on inclut le chemin entier :
                paths.Add(propPath.Substring(0, propPath.LastIndexOf('.')));

        return paths;
    }
}