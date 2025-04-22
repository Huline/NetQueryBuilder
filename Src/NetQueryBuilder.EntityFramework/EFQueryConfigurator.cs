using Microsoft.EntityFrameworkCore;
using NetQueryBuilder.Configurations;
using NetQueryBuilder.Operators;
using NetQueryBuilder.Queries;

namespace NetQueryBuilder.EntityFramework;

public class EfQueryConfigurator<TDbContext> : IQueryConfigurator
    where TDbContext : DbContext
{
    private readonly TDbContext _dbContext;
    private ConditionConfiguration _conditionConfiguration = new(ArraySegment<string>.Empty, ArraySegment<string>.Empty, -1, ArraySegment<Type>.Empty, null);
    private IExpressionStringifier _expressionStringifier = new UpperSeparatorExpressionStringifier();
    private SelectConfiguration _selectConfiguration = new(ArraySegment<string>.Empty, ArraySegment<string>.Empty, -1, ArraySegment<Type>.Empty, null);

    public EfQueryConfigurator(TDbContext dbContext)
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
        return new EfQuery<T>(_dbContext, _selectConfiguration, _conditionConfiguration, new EfOperatorFactory(_expressionStringifier));
    }

    public IQuery BuildFor(Type type)
    {
        return (IQuery)Activator.CreateInstance(typeof(EfQuery<>).MakeGenericType(_dbContext.Model.GetEntityTypes().First().ClrType), _dbContext, _selectConfiguration, _conditionConfiguration, new EfOperatorFactory(_expressionStringifier)) !;
    }
}