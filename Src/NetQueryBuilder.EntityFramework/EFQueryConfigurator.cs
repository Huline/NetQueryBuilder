using Microsoft.EntityFrameworkCore;
using NetQueryBuilder.Configurations;
using NetQueryBuilder.Operators;
using NetQueryBuilder.Queries;

namespace NetQueryBuilder.EntityFramework;

public class EFQueryConfigurator<TDbContext> : IQueryConfigurator
    where TDbContext : DbContext
{
    private readonly TDbContext _dbContext;

    public EFQueryConfigurator(TDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public IEnumerable<Type> GetEntities()
    {
        return _dbContext
            .Model
            .GetEntityTypes()
            .Select(t => t.ClrType)
            .ToList();
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
        return new EfQuery<T>(_dbContext, new EfOperatorFactory(new UpperSeparatorExpressionStringifier()));
    }

    public IQuery BuildFor(Type type)
    {
        return (IQuery)Activator.CreateInstance(typeof(EfQuery<>).MakeGenericType(_dbContext.Model.GetEntityTypes().First().ClrType), _dbContext, new EfOperatorFactory(new UpperSeparatorExpressionStringifier())) !;
    }
}