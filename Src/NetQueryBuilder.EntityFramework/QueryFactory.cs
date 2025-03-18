using System.Collections;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NetQueryBuilder.Operators;
using NetQueryBuilder.Queries;

namespace NetQueryBuilder.EntityFramework;

public class QueryFactory<TDbContext> : IQueryFactory
    where TDbContext : DbContext
{
    private readonly IServiceProvider _serviceProvider;

    public QueryFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Task<IEnumerable<Type>> GetEntities()
    {
        return Task.FromResult<IEnumerable<Type>>(_serviceProvider.GetRequiredService<TDbContext>()
            .Model
            .GetEntityTypes()
            .Select(t => t.ClrType)
            .ToList());
    }

    public Query<T> Create<T>() where T : class
    {
        return new EFQuery<T>(_serviceProvider.GetRequiredService<TDbContext>(), _serviceProvider.GetRequiredService<IOperatorFactory>());
    }
}

public class EFQuery<T> : Query<T> where T : class
{
    private readonly DbContext _dbContext;

    public EFQuery(DbContext context, IOperatorFactory operatorFactory)
        : base(operatorFactory)
    {
        _dbContext = context;
    }

    public override async Task<IEnumerable> Execute(IEnumerable<PropertyPath>? selectedProperties)
    {
        // var predicat = Lambda.ToString()
        //     .Replace("AndAlso", "&&")
        //     .Replace("OrElse", "||")
        //     .Replace("False", "false")
        //     .Replace("True", "true")
        //     .Replace(" Not ", " !")
        //     .Replace(" Not(", " !(");
        //
        // var types = _dbContext
        //     .Model
        //     .GetEntityTypes()
        //     .Select(t => t.ClrType.Namespace)
        //     .Concat(new[]
        //     {
        //         _dbContext.GetType().Namespace,
        //         typeof(EF).Namespace,
        //         "System"
        //     })
        //     .Distinct()
        //     .ToList();
        //
        // var options = ScriptOptions
        //     .Default
        //     .AddReferences(typeof(EFOperatorFactory).Assembly)
        //     .AddReferences(typeof(EF).Assembly)
        //     .AddReferences(typeof(T).Assembly)
        //     .AddImports(types);

        //Expression<Func<T, bool>> predicate = await CSharpScript.EvaluateAsync<Expression<Func<T, bool>>>(predicat, options);
        Expression<Func<T, bool>> predicate = Lambda as Expression<Func<T, bool>>;

        return await QueryData(predicate, selectedProperties.Select(p => p.PropertyName).ToList());
    }

    public async Task<IEnumerable> QueryData(
        Expression<Func<T, bool>> predicate,
        IEnumerable<string> selectedProperties)
    {
        IQueryable<T> query = _dbContext.Set<T>().AsQueryable();
        var navigationPaths = ExtractNavigationPaths(selectedProperties);
        foreach (var path in navigationPaths) query = query.Include(path);

        query = query.Where(predicate);
        var select = new SelectBuilderService<T>().BuildSelect(selectedProperties);
        query = query.Select(select);

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