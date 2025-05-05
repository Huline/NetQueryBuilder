using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Linq;
using NetQueryBuilder.Configurations;
using NetQueryBuilder.Operators;
using NetQueryBuilder.Queries;

namespace NetQueryBuilder.EntityFrameworkNet4
{
    public class EfQueryConfigurator<TDbContext> : IQueryConfigurator
        where TDbContext : DbContext
    {
        private readonly TDbContext _dbContext;
        private ConditionConfiguration _conditionConfiguration = new ConditionConfiguration(new List<string>(), new List<string>(), -1, new List<Type>(), null);
        private IExpressionStringifier _expressionStringifier = new UpperSeparatorExpressionStringifier();
        private SelectConfiguration _selectConfiguration = new SelectConfiguration(new List<string>(), new List<string>(), -1, new List<Type>(), null);

        public EfQueryConfigurator(TDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<Type> GetEntities()
        {
            var objectContext = ((IObjectContextAdapter)_dbContext).ObjectContext;
            var metadataWorkspace = objectContext.MetadataWorkspace;
            var entityTypes = new List<Type>();

            foreach (var entityType in metadataWorkspace
                         .GetItems<EntityType>(DataSpace.CSpace)
                         .Where(e => !e.Abstract))
                entityTypes.Add(entityType.GetType());

            return entityTypes;
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
            return (IQuery)Activator.CreateInstance(typeof(EfQuery<>).MakeGenericType(type), _dbContext, _selectConfiguration, _conditionConfiguration, new EfOperatorFactory(_expressionStringifier));
        }
    }
}