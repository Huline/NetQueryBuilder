using NetQueryBuilder.Operators;
using NetQueryBuilder.Queries;

namespace NetQueryBuilder.Configurations;

public interface IQueryConfigurator
{
    IEnumerable<Type> GetEntities();
    IQueryConfigurator UseExpressionStringifier(IExpressionStringifier expressionStringifier);
    IQueryConfigurator ConfigureSelect(Action<ISelectConfigurator> selectBuilder);
    IQueryConfigurator ConfigureConditions(Action<IConditionConfigurator> selectBuilder);
    IQuery BuildFor<T>() where T : class;
    IQuery BuildFor(Type type);
}