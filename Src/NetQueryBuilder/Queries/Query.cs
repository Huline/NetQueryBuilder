using System.Collections;
using System.Linq.Dynamic.Core;
using System.Linq.Dynamic.Core.CustomTypeProviders;
using System.Linq.Expressions;
using NetQueryBuilder.Conditions;
using NetQueryBuilder.Extensions;
using NetQueryBuilder.Operators;

namespace NetQueryBuilder.Queries;

public abstract class Query : IQuery
{
    public BlockCondition Condition { get; protected set; }
    public LambdaExpression Lambda { get; protected set; }
    public ParameterExpression Parameter { get; protected set; }
    public IEnumerable<PropertyPath> SelectedPropertyPaths { get; set; }

    public void Compile()
    {
        Lambda = Expression.Lambda(
            Condition.Compile(),
            Parameter);
    }

    public abstract Task<IEnumerable> Execute(IEnumerable<PropertyPath>? selectedProperties);

    public abstract IEnumerable<PropertyPath> AvailableProperties();
}

public abstract class Query<TEntity> : Query, IQuery<TEntity> where TEntity : class
{
    private readonly IOperatorFactory _operatorFactory;

    public Query(IOperatorFactory operatorFactory)
    {
        _operatorFactory = operatorFactory;
        Parameter = Expression.Parameter(
            typeof(TEntity),
            typeof(TEntity).Name.ToLower());

        Lambda = CreateRelationalPredicate<TEntity>(
            typeof(TEntity).GetProperties().First().Name,
            Parameter,
            typeof(TEntity).GetProperties().First().PropertyType.GetDefaultValue(),
            ExpressionType.Equal);

        if (Lambda.Body is BinaryExpression)
            Condition = new BlockCondition(new[]
            {
                new LogicalCondition(AvailableProperties().First(), ExpressionType.And)
            }, ExpressionType.And);
    }

    public Query(string expression, DefaultDynamicLinqCustomTypeProvider _customTypeProvider)
    {
        // var config = new ParsingConfig { RenameParameterExpression = true };
        // config.CustomTypeProvider = new CustomEFTypeProvider(config, true);
        var config = new ParsingConfig { RenameParameterExpression = true };
        config.CustomTypeProvider = _customTypeProvider;

        Lambda = DynamicExpressionParser.ParseLambda(
            config,
            typeof(TEntity),
            typeof(bool),
            expression);

        Parameter = Lambda.Parameters[0];

        if (Lambda.Body is BinaryExpression)
            Condition = new BlockCondition(new[]
            {
                new LogicalCondition(AvailableProperties().First(), ExpressionType.And)
            }, ExpressionType.And);
    }


    public sealed override IEnumerable<PropertyPath> AvailableProperties()
    {
        return PropertyInspector.GetAllPropertyPaths(typeof(TEntity), Parameter, _operatorFactory);
    }

    private Expression<Func<T, bool>> CreateRelationalPredicate<T>(
        string propertyName,
        ParameterExpression parameter,
        object comparisonValue,
        ExpressionType expressionType)
    {
        var property = typeof(T).GetProperty(propertyName);
        var memberAccess = Expression.MakeMemberAccess(parameter, property);

        var right = Expression.Constant(comparisonValue);

        var binary = Expression.MakeBinary(expressionType, memberAccess, right);

        Expression<Func<T, bool>> expression = Expression.Lambda(binary, parameter) as Expression<Func<T, bool>>;

        return expression;
    }
}