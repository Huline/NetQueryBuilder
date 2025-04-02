using System.Collections;
using System.Linq.Dynamic.Core;
using System.Linq.Dynamic.Core.CustomTypeProviders;
using System.Linq.Expressions;
using NetQueryBuilder.Conditions;
using NetQueryBuilder.Configurations;
using NetQueryBuilder.Operators;
using NetQueryBuilder.Utils;

namespace NetQueryBuilder.Queries;

public abstract class Query<TEntity> : IQuery where TEntity : class
{
    private readonly BlockCondition _condition;
    private readonly List<PropertyPath> _conditionPropertyPaths;
    private readonly IOperatorFactory _operatorFactory;
    private readonly ParameterExpression _parameter;
    private readonly List<SelectPropertyPath> _selectPropertyPaths;
    private LambdaExpression? _lambda;

    protected Query(SelectConfiguration selectConfiguration, ConditionConfiguration conditionConfiguration, IOperatorFactory operatorFactory)
    {
        _parameter = Expression.Parameter(
            typeof(TEntity),
            typeof(TEntity).Name.ToLower());
        _operatorFactory = operatorFactory;
        _selectPropertyPaths = AvailableProperties(selectConfiguration.PropertyStringifier).Where(p => MatchConfiguration(p, selectConfiguration)).Select(p => new SelectPropertyPath(p)).ToList();
        _conditionPropertyPaths = AvailableProperties(conditionConfiguration.PropertyStringifier).Where(p => MatchConfiguration(p, conditionConfiguration)).ToList();
        _condition = new BlockCondition([], LogicalOperator.And);
        _lambda = null; 
        _condition.ConditionChanged += OnConditionConditionChanged;
    }

    public EventHandler? OnChanged { get; set; }
    public IReadOnlyCollection<SelectPropertyPath> SelectPropertyPaths => _selectPropertyPaths;
    public IReadOnlyCollection<PropertyPath> ConditionPropertyPaths => _conditionPropertyPaths;
    public BlockCondition Condition => _condition;

    public virtual async Task<IEnumerable> Execute()
    {
        var predicate = Compile() as Expression<Func<TEntity, bool>>;
        var selectedProps = SelectPropertyPaths
            .Where(p => p.IsSelected)
            .Select(p => p.Property.PropertyFullName)
            .ToList();
        var queryable = GetQueryable(selectedProps);

        if (predicate != null)
            queryable = queryable.Where(predicate);
        if (selectedProps.Count != 0)
        {
            var select = SelectBuilderService<TEntity>.BuildSelect(selectedProps);
            queryable = queryable.Select(select);
        }

        return await ToList(queryable);
    }

    public virtual LambdaExpression? Compile()
    {
        if(_condition.Compile() == null)
            return null;
        _lambda = Expression.Lambda(
            _condition.Compile(),
            _parameter);
        return _lambda;
    }

    protected abstract IQueryable<TEntity> GetQueryable(IReadOnlyCollection<string> selectedProperties);

    protected virtual Task<IEnumerable> ToList(IQueryable<TEntity> queryable)
    {
        return Task.FromResult<IEnumerable>(queryable.ToList());
    }

    private IEnumerable<PropertyPath> AvailableProperties(IPropertyStringifier? propertyStringifier)
    {
        return PropertyInspector.GetAllPropertyPaths(typeof(TEntity), _parameter, propertyStringifier, _operatorFactory);
    }


    private bool MatchConfiguration(PropertyPath propertyPath, SelectConfiguration selectConfiguration)
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

    private bool MatchConfiguration(PropertyPath propertyPath, ConditionConfiguration conditionConfiguration)
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

    private void OnConditionConditionChanged(object? sender, EventArgs e)
    {
        OnChanged?.Invoke(this, e);
    }


    private static Expression<Func<T, bool>> CreateRelationalPredicate<T>(
        string propertyName,
        ParameterExpression parameter,
        object comparisonValue,
        ExpressionType expressionType)
    {
        var property = typeof(T).GetProperty(propertyName);

        var memberAccess = Expression.MakeMemberAccess(parameter, property!);
        var right = Expression.Constant(comparisonValue);
        var binary = Expression.MakeBinary(expressionType, memberAccess, right);

        var expression = Expression.Lambda(binary, parameter) as Expression<Func<T, bool>>;
        return expression ?? throw new InvalidOperationException("Expression is not valid");
    }
}