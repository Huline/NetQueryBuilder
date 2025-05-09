using System;
using System.Collections.Generic;
using System.Linq;
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
        Task<QueryResult<dynamic>> Execute(int pageSize);
        Task<QueryResult<TProjection>> Execute<TProjection>(int pageSize);
    }

    public class QueryResult<TEntity>
    {
        private readonly Func<int, int, Task<IReadOnlyCollection<TEntity>>> _fetchItems;

        private QueryResult(Func<int, int, Task<IReadOnlyCollection<TEntity>>> fetchItems, IEnumerable<TEntity> entities, int totalItems, int totalPage, int currentPage, int pageSize)
        {
            _fetchItems = fetchItems;
            Items = entities.ToList();
            TotalItems = totalItems;
            TotalPage = totalPage;
            CurrentPage = currentPage;
            PageSize = pageSize;
        }

        public IReadOnlyCollection<TEntity> Items { get; }
        public int TotalItems { get; }
        public int TotalPage { get; }
        public int CurrentPage { get; }
        public int PageSize { get; }


        public async Task<QueryResult<TEntity>> NextPage()
        {
            var currentPage = CurrentPage + 1;
            var offset = currentPage * PageSize;
            var queryResult = await _fetchItems(PageSize, offset);
            return new QueryResult<TEntity>(
                _fetchItems,
                queryResult,
                TotalItems,
                TotalPage,
                currentPage,
                PageSize);
        }

        public static async Task<QueryResult<TEntity>> FromQuery(int count, Func<int, int, Task<IReadOnlyCollection<TEntity>>> fetchItems, int pageSize)
        {
            var totalPage = (int)Math.Ceiling((double)count / pageSize);
            const int currentPage = 0;
            var items = await fetchItems(pageSize, 0);
            return new QueryResult<TEntity>(fetchItems, items, count, totalPage, currentPage, pageSize);
        }
    }
}