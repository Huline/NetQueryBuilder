using System;
using System.Collections.Generic;
using System.Linq;
using NetQueryBuilder.Operators;
using NetQueryBuilder.Queries;

namespace NetQueryBuilder.Configurations
{
    public class QueryableQueryConfigurator<TEntity> : IQueryConfigurator
    {
        private readonly IQueryable<TEntity> _queryable;
        private ConditionConfiguration _conditionConfiguration = new ConditionConfiguration(ArraySegment<string>.Empty, ArraySegment<string>.Empty, -1, ArraySegment<Type>.Empty, null);
        private IExpressionStringifier _expressionStringifier = new UpperSeparatorExpressionStringifier();
        private SelectConfiguration _selectConfiguration = new SelectConfiguration(ArraySegment<string>.Empty, ArraySegment<string>.Empty, -1, ArraySegment<Type>.Empty, null);

        public QueryableQueryConfigurator(IQueryable<TEntity> queryable)
        {
            _queryable = queryable;
        }

        public IEnumerable<Type> GetEntities()
        {
            return new List<Type> { typeof(TEntity) };
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
            return new QueryableQuery<T>((_queryable as IQueryable<T>)!, _selectConfiguration, _conditionConfiguration, new DefaultOperatorFactory(_expressionStringifier));
        }

        public IQuery BuildFor(Type type)
        {
            return (IQuery)Activator.CreateInstance(typeof(QueryableQuery<>).MakeGenericType(type), _queryable, _selectConfiguration, _conditionConfiguration, new DefaultOperatorFactory(_expressionStringifier)) !;
        }
    }
}