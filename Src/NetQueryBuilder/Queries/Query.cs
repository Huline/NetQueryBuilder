using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Threading.Tasks;
using NetQueryBuilder.Conditions;
using NetQueryBuilder.Configurations;
using NetQueryBuilder.Operators;
using NetQueryBuilder.Utils;

namespace NetQueryBuilder.Queries
{
    public abstract class Query<TEntity> : IQuery where TEntity : class
    {
        private readonly List<PropertyPath> _conditionPropertyPaths;
        private readonly IOperatorFactory _operatorFactory;
        private readonly ParameterExpression _parameter;
        private readonly List<SelectPropertyPath> _selectPropertyPaths;
        private LambdaExpression _lambda;

        protected Query(SelectConfiguration selectConfiguration, ConditionConfiguration conditionConfiguration, IOperatorFactory operatorFactory)
        {
            _parameter = Expression.Parameter(
                typeof(TEntity),
                typeof(TEntity).Name.ToLower());
            _operatorFactory = operatorFactory;
            _selectPropertyPaths = AvailableProperties(selectConfiguration.PropertyStringifier).Where(p => MatchConfiguration(p, selectConfiguration)).Select(p => new SelectPropertyPath(p)).ToList();
            _conditionPropertyPaths = AvailableProperties(conditionConfiguration.PropertyStringifier).Where(p => MatchConfiguration(p, conditionConfiguration)).ToList();
            Condition = new BlockCondition(new List<ICondition>(), LogicalOperator.And);
            _lambda = null;
            Condition.ConditionChanged += OnConditionConditionChanged;
        }

        public EventHandler OnChanged { get; set; }
        public IReadOnlyCollection<SelectPropertyPath> SelectPropertyPaths => _selectPropertyPaths;
        public IReadOnlyCollection<PropertyPath> ConditionPropertyPaths => _conditionPropertyPaths;
        public BlockCondition Condition { get; }

        public virtual LambdaExpression Compile()
        {
            var expression = Condition.Compile();
            if (expression == null)
                return null;
            _lambda = Expression.Lambda(
                expression,
                _parameter);
            return _lambda;
        }

        public virtual async Task<IReadOnlyCollection<TProjection>> Execute<TProjection>(int? limit = null, int? offset = null)
        {
            var queryable = GetFilteredQuery(limit, offset, out var selectedProps);
            return await ToList(SelectProjectionProperties<TProjection>(selectedProps, queryable));
        }

        public virtual async Task<IReadOnlyCollection<dynamic>> Execute(int? limit = null, int? offset = null)
        {
            var queryable = GetFilteredQuery(limit, offset, out var selectedProps);
            return SelectProperties(selectedProps, queryable).AsDynamicEnumerable().ToList();
        }

        private static IQueryable<TProjection> SelectProjectionProperties<TProjection>(List<PropertyPath> selectedProps, IQueryable<TEntity> queryable)
        {
            var select = SelectBuilderService<TEntity>.BuildSelect<TProjection>(selectedProps);
            return queryable.Select(select);
        }

        private IQueryable<TEntity> GetFilteredQuery(int? limit, int? offset, out List<PropertyPath> selectedProps)
        {
            var predicate = Compile() as Expression<Func<TEntity, bool>>;
            selectedProps = SelectPropertyPaths
                .Where(p => p.IsSelected)
                .Select(p => p.Property)
                .ToList();
            var queryable = GetQueryable(selectedProps);

            if (predicate != null)
                queryable = queryable.Where(predicate);

            if (limit.HasValue)
                queryable = queryable.Take(limit.Value);
            if (offset.HasValue)
                queryable = queryable.Skip(offset.Value);
            return queryable;
        }

        protected virtual IQueryable SelectProperties(List<PropertyPath> selectedProps, IQueryable<TEntity> queryable)
        {
            var select = SelectBuilderService<TEntity>.BuildSelect(selectedProps);
            return queryable.Select(select);
        }

        protected abstract IQueryable<TEntity> GetQueryable(IReadOnlyCollection<PropertyPath> selectedProperties);

        protected virtual Task<IReadOnlyCollection<TProjection>> ToList<TProjection>(IQueryable<TProjection> queryable)
        {
            return Task.FromResult<IReadOnlyCollection<TProjection>>(queryable.ToList());
        }

        private IEnumerable<PropertyPath> AvailableProperties(IPropertyStringifier propertyStringifier)
        {
            return PropertyInspector.GetAllPropertyPaths(typeof(TEntity), _parameter, propertyStringifier, _operatorFactory);
        }


        private static bool MatchConfiguration(PropertyPath propertyPath, SelectConfiguration selectConfiguration)
        {
            if (selectConfiguration.Fields.Any() && !selectConfiguration.Fields.Contains(propertyPath.PropertyFullName))
                return false;
            if (selectConfiguration.ExcludedRelationships.Any() && selectConfiguration.ExcludedRelationships.Contains(propertyPath.ParentType))
                return false;
            if (selectConfiguration.IgnoreFields.Any() && selectConfiguration.IgnoreFields.Contains(propertyPath.PropertyFullName))
                return false;
            if (selectConfiguration.Depth >= 0 && propertyPath.Depth > selectConfiguration.Depth)
                return false;

            return true;
        }

        private static bool MatchConfiguration(PropertyPath propertyPath, ConditionConfiguration conditionConfiguration)
        {
            if (conditionConfiguration.Fields.Any() && !conditionConfiguration.Fields.Contains(propertyPath.PropertyFullName))
                return false;
            if (conditionConfiguration.ExcludedRelationships.Any() && conditionConfiguration.ExcludedRelationships.Contains(propertyPath.ParentType))
                return false;
            if (conditionConfiguration.IgnoreFields.Any() && conditionConfiguration.IgnoreFields.Contains(propertyPath.PropertyFullName))
                return false;
            if (conditionConfiguration.Depth >= 0 && propertyPath.Depth > conditionConfiguration.Depth)
                return false;
            return true;
        }

        private void OnConditionConditionChanged(object sender, EventArgs e)
        {
            OnChanged?.Invoke(this, e);
        }
    }
}