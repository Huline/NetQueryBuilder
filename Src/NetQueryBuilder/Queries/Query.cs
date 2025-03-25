using System.Collections;
using System.Linq.Dynamic.Core;
using System.Linq.Dynamic.Core.CustomTypeProviders;
using System.Linq.Expressions;
using NetQueryBuilder.Conditions;
using NetQueryBuilder.Extensions;
using NetQueryBuilder.Operators;

namespace NetQueryBuilder.Queries;

public abstract class Query<TEntity> : IQuery where TEntity : class
{
    private readonly BlockCondition _condition;
    private readonly IOperatorFactory _operatorFactory;
    private readonly ParameterExpression _parameter;
    protected LambdaExpression Lambda;

    private Query(LambdaExpression lambda, IOperatorFactory operatorFactory)
    {
        if (lambda.Body is not BinaryExpression)
            throw new InvalidOperationException("Expression is not valid");

        Lambda = lambda;
        _parameter = lambda.Parameters.First();
        _operatorFactory = operatorFactory;
        SelectedPropertyPaths = AvailableProperties();
        _condition = new BlockCondition([
            new SimpleCondition(SelectedPropertyPaths.First(), LogicalOperator.And)
        ], LogicalOperator.And);
        _condition.ConditionChanged += OnConditionConditionChanged;
    }

    protected Query(IOperatorFactory operatorFactory)
        : this(CreateRelationalPredicate<TEntity>(
            typeof(TEntity).GetProperties().First().Name,
            Expression.Parameter(
                typeof(TEntity),
                typeof(TEntity).Name.ToLower()),
            typeof(TEntity).GetProperties().First().PropertyType.GetDefaultValue(),
            ExpressionType.Equal), operatorFactory)
    {
    }

    protected Query(string expression, DefaultDynamicLinqCustomTypeProvider customTypeProvider, IOperatorFactory operatorFactory)
        : this(DynamicExpressionParser.ParseLambda(
            new ParsingConfig { RenameParameterExpression = true, CustomTypeProvider = customTypeProvider },
            typeof(TEntity),
            typeof(bool),
            expression), operatorFactory)
    {
        // var config = new ParsingConfig { RenameParameterExpression = true };
        // config.CustomTypeProvider = new CustomEFTypeProvider(config, true);
    }

    public EventHandler? OnChanged { get; set; }
    public IEnumerable<PropertyPath> SelectedPropertyPaths { get; set; }
    public IReadOnlyCollection<ICondition> Conditions => [_condition];

    public abstract Task<IEnumerable> Execute(IEnumerable<PropertyPath>? selectedProperties);

    public virtual LambdaExpression Compile()
    {
        Lambda = Expression.Lambda(
            _condition.Compile(),
            _parameter);
        return Lambda;
    }

    public IEnumerable<PropertyPath> AvailableProperties()
    {
        return PropertyInspector.GetAllPropertyPaths(typeof(TEntity), _parameter, _operatorFactory);
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