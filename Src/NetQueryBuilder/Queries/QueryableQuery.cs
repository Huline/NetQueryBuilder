using System.Collections.Generic;
using System.Linq;
using NetQueryBuilder.Configurations;
using NetQueryBuilder.Operators;

namespace NetQueryBuilder.Queries
{
    public class QueryableQuery<T> : Query<T> where T : class
    {
        private readonly IQueryable<T> _queryable;

        public QueryableQuery(IQueryable<T> queryable, SelectConfiguration selectConfiguration, ConditionConfiguration conditionConfiguration, IOperatorFactory operatorFactory)
            : base(selectConfiguration, conditionConfiguration, operatorFactory)
        {
            _queryable = queryable;
        }


        protected override IQueryable<T> GetQueryable(IReadOnlyCollection<PropertyPath> selectedProperties)
        {
            return _queryable;
        }
    }
}