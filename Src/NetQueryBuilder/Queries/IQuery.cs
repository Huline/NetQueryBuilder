using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using NetQueryBuilder.Conditions;

namespace NetQueryBuilder.Queries
{
    public interface IQuery
    {
        EventHandler? OnChanged { get; set; }
        IReadOnlyCollection<SelectPropertyPath> SelectPropertyPaths { get; }
        IReadOnlyCollection<PropertyPath> ConditionPropertyPaths { get; }
        BlockCondition Condition { get; }
        LambdaExpression? Compile();
        Task<IEnumerable> Execute(int? limit = null, int? offset = null);
    }
}