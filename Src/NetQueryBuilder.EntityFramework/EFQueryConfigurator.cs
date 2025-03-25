using Microsoft.EntityFrameworkCore;
using NetQueryBuilder.Configurations;
using NetQueryBuilder.Operators;
using NetQueryBuilder.Queries;

namespace NetQueryBuilder.EntityFramework;

public class EfQueryConfigurator<TDbContext>(TDbContext dbContext) : IQueryConfigurator
    where TDbContext : DbContext
{
    private ConditionConfiguration _conditionConfiguration = new(ArraySegment<string>.Empty, ArraySegment<string>.Empty, -1, [], null);
    private IExpressionStringifier _expressionStringifier = new UpperSeparatorExpressionStringifier();
    private SelectConfiguration _selectConfiguration = new(ArraySegment<string>.Empty, ArraySegment<string>.Empty, -1, [], null);

    public IEnumerable<Type> GetEntities()
    {
        return dbContext
            .Model
            .GetEntityTypes()
            .Select(t => t.ClrType)
            .ToList();
    }

    public IQueryConfigurator UseExpressionStringifier(IExpressionStringifier expressionStringifier)
    {
        _expressionStringifier = expressionStringifier;
        return this;
    }

    public IQueryConfigurator ConfigureSelect(Action<ISelectConfigurator> selectBuilder)
    {
        var selectConfigurator = new SelectConfigurator();
        selectBuilder(selectConfigurator);
        _selectConfiguration = selectConfigurator.Build();
        return this;
    }

    public IQueryConfigurator ConfigureConditions(Action<IConditionConfigurator> selectBuilder)
    {
        var conditionConfigurator = new ConditionConfigurator();
        selectBuilder(conditionConfigurator);
        _conditionConfiguration = conditionConfigurator.Build();
        return this;
    }

    public IQuery BuildFor<T>() where T : class
    {
        return new EfQuery<T>(dbContext, _selectConfiguration, _conditionConfiguration, new EfOperatorFactory(_expressionStringifier));
    }

    public IQuery BuildFor(Type type)
    {
        return (IQuery)Activator.CreateInstance(typeof(EfQuery<>).MakeGenericType(dbContext.Model.GetEntityTypes().First().ClrType), dbContext, _selectConfiguration, _conditionConfiguration, new EfOperatorFactory(_expressionStringifier)) !;
    }
}