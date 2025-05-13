using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetQueryBuilder.Queries
{
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

        /// <summary>
        ///     Navigates to the specified page number of the query result.
        /// </summary>
        /// <param name="pageNumber">The page number to navigate to. Must be within the valid range of 0 to TotalPage - 1.</param>
        /// <returns>A new <see cref="QueryResult{TEntity}" /> instance representing the specified page.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the provided page number is outside the valid range.</exception>
        public async Task<QueryResult<TEntity>> GoToPage(int pageNumber)
        {
            if (pageNumber < 0 || pageNumber >= TotalPage)
                throw new ArgumentOutOfRangeException(nameof(pageNumber), "Page number is out of range.");

            var offset = pageNumber * PageSize;
            var queryResult = await _fetchItems(PageSize, offset);
            return new QueryResult<TEntity>(
                _fetchItems,
                queryResult,
                TotalItems,
                TotalPage,
                pageNumber,
                PageSize);
        }

        /// <summary>
        ///     Creates a new instance of <see cref="QueryResult{TEntity}" /> based on the provided total count, fetch function, and page size.
        /// </summary>
        /// <param name="count">The total number of items in the query result.</param>
        /// <param name="fetchItems">A function to fetch items for a specific page, with parameters for page size and offset.</param>
        /// <param name="pageSize">The number of items to display per page.</param>
        /// <returns>A <see cref="QueryResult{TEntity}" /> containing the fetched items and pagination details.</returns>
        public static async Task<QueryResult<TEntity>> FromQuery(int count, Func<int, int, Task<IReadOnlyCollection<TEntity>>> fetchItems, int pageSize)
        {
            var totalPage = (int)Math.Ceiling((double)count / pageSize);
            const int currentPage = 0;
            var items = await fetchItems(pageSize, 0);
            return new QueryResult<TEntity>(fetchItems, items, count, totalPage, currentPage, pageSize);
        }
    }
}