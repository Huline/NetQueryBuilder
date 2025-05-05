using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using NetQueryBuilder.Conditions;

namespace NetQueryBuilder.Queries
{
    public interface IQuery
    {
        EventHandler OnChanged { get; set; }
        IReadOnlyCollection<SelectPropertyPath> SelectPropertyPaths { get; }
        IReadOnlyCollection<PropertyPath> ConditionPropertyPaths { get; }
        BlockCondition Condition { get; }
        LambdaExpression Compile();
        Task<IReadOnlyCollection<dynamic>> Execute(int? limit = null, int? offset = null);
        Task<IReadOnlyCollection<TProjection>> Execute<TProjection>(int? limit = null, int? offset = null);
    }
}