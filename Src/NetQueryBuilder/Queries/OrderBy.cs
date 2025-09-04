using System;
using System.Linq;
using System.Linq.Expressions;
using NetQueryBuilder.Properties;

namespace NetQueryBuilder.Queries
{
    public interface OrderBy
    {
        void Set(PropertyPath propertyPath, OrderDirection direction);
    }

    public class OrderBy<TEntity> : OrderBy
    {
        private OrderDirection _direction;
        private PropertyPath _propertyPath;

        public void Set(PropertyPath propertyPath, OrderDirection direction)
        {
            _propertyPath = propertyPath;
            _direction = direction;
        }

        public IQueryable<TEntity> Compile(IQueryable<TEntity> queryable)
        {
            if (_propertyPath == null)
                throw new InvalidOperationException("PropertyPath must be set before compiling.");

            var parameter = Expression.Parameter(typeof(TEntity), "x");
            var property = Expression.Property(parameter, _propertyPath.PropertyName);
            var lambda = Expression.Lambda(property, parameter);

            var expression = Expression.Lambda<Func<TEntity, object>>(Expression.Convert(lambda.Body, typeof(object)), lambda.Parameters);
            return _direction == OrderDirection.Ascending ? queryable.OrderBy(expression) : queryable.OrderByDescending(expression);
        }
    }
}