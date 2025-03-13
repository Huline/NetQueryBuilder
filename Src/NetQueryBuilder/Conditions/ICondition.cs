using System.Collections;
using System.Linq.Dynamic.Core;
using System.Linq.Dynamic.Core.CustomTypeProviders;
using System.Linq.Expressions;
using System.Reflection;
using NetQueryBuilder.Extensions;
using NetQueryBuilder.Operators;

namespace NetQueryBuilder.Conditions;

public interface ICondition
{
    internal ICondition? Parent { get; set; }
    ICondition GetRoot();
    EventHandler ConditionChanged { get; set; }
    ExpressionType LogicalType { get; set; }
    Expression Compile();
}

public class LogicalCondition : ICondition
{
    private PropertyPath _propertyPath;
    public PropertyPath PropertyPath 
    {
        get => _propertyPath;
        set
        {
            _propertyPath = value;
            ConditionChanged?.Invoke(this, EventArgs.Empty);
        }
    }
    public object Value { get; set; }
    private ExpressionType _logicalType;
    public ExpressionType LogicalType
    {
        get => _logicalType;
        set
        {
            _logicalType = value;
            ConditionChanged?.Invoke(this, EventArgs.Empty);
        }
    }
    public ICondition? Parent { get; set; }
    private Expression _left;
    public Expression Left
    {
        get => _left;
        set
        {
            _left = value;
            ConditionChanged?.Invoke(this, EventArgs.Empty);
        }
    }
    private Expression _right;
    public Expression Right
    {
        get => _right;
        set
        {
            _right = value;
            ConditionChanged?.Invoke(this, EventArgs.Empty);
        }
    }
    private ExpressionType _comparaisonType;
    public ExpressionType ComparaisonType
    {
        get => _comparaisonType;
        set
        {
            _comparaisonType = value;
            ConditionChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public EventHandler ConditionChanged { get; set; }

    public Expression Compile()
    {
        return Expression.MakeBinary(ComparaisonType, _left, _right);
    }
    
    public ICondition GetRoot()
    {
        return Parent == null ? this : Parent.GetRoot();
    }
}

public class PropertyPath
{
    public string PropertyName { get; }
    public Type ParentType { get; }
    public Type PropertyType { get; }


    public PropertyPath(string propertyName, Type propertyType, Type parentType)
    {
        PropertyName = propertyName;
        ParentType = parentType;
        PropertyType = propertyType;
    }


    public BinaryExpression GetExpression(ParameterExpression parameterExpression)
    {
        if (parameterExpression == null)
            throw new InvalidOperationException("Le ParameterExpression n'a pas été défini. Appelez SetParameterExpression d'abord.");
            
        // Création de l'expression d'accès au membre
        var memberExpression = Expression.Property(parameterExpression, PropertyName);
        
        // Obtention de l'opérateur binaire par défaut pour ce type de propriété
        var defaultOperator = GetOperators(PropertyType)
            .OfType<BinaryOperator>()
            .First();
            
        // Création d'une valeur par défaut pour ce type
        var defaultValue = GetDefaultValueForType(PropertyType);
        
        // Création de l'expression binaire complète
        return Expression.MakeBinary(
            defaultOperator.ExpressionType,
            memberExpression,
            defaultValue
        );

    }

    // Méthode utilitaire pour obtenir une valeur par défaut pour un type donné
    private static Expression GetDefaultValueForType(Type propertyType)
    {
        return propertyType switch
        {
            Type type when
                type == typeof(int)
                || type == typeof(long)
                || type == typeof(string)
                || type == typeof(bool) => Expression.Constant(propertyType.GetDefaultValue(), propertyType),
            Type type when
                type == typeof(DateTime) => Expression.Constant(DateTime.UtcNow),
            _ => throw new Exception("Type de propriété non pris en charge")
        };
    }


    private static List<ExpressionOperator> GetOperators(Type operandType)
    {
        return operandType switch
        {
            Type type when type == typeof(int) => new List<ExpressionOperator>
            {
                new EqualsOperator(),
                new NotEqualsOperator(),
                new LessThanOperator(),
                new LessThanOrEqualOperator(),
                new GreaterThanOperator(),
                new GreaterThanOrEqualOperator(),
                new InListOperator<int>(),
                new InListOperator<int>(true)
            },
            Type type when type == typeof(string) => new List<ExpressionOperator>
            {
                new EqualsOperator(),
                new NotEqualsOperator(),
                // new EfLikeOperator(),
                // new EfLikeOperator(true),
                new InListOperator<string>(),
                new InListOperator<string>(true)
            },
            Type type when type == typeof(bool) => new List<ExpressionOperator>
            {
                new EqualsOperator(),
                new NotEqualsOperator()
            },
            Type type when type == typeof(DateTime) => new List<ExpressionOperator>
            {
                new EqualsOperator(),
                new NotEqualsOperator(),
                new LessThanOperator(),
                new LessThanOrEqualOperator(),
                new GreaterThanOperator(),
                new GreaterThanOrEqualOperator()
            },
            _ => new List<ExpressionOperator>
            {
                new EqualsOperator(),
                new NotEqualsOperator()
            }
        };
    }

    public override bool Equals(object? obj)
    {
        if (obj is PropertyPath other)
        {
            return string.Equals(PropertyName, other.PropertyName, StringComparison.Ordinal);
        }
        return false;
    }
    
    public override int GetHashCode()
    {
        return PropertyName.GetHashCode(StringComparison.Ordinal);
    }
}

public class BlockCondition : ICondition
{
    private ICondition? _parent;
    public ICondition? Parent
    {
        get => _parent;
        set
        {
            _parent = value;
            ConditionChanged?.Invoke(this, EventArgs.Empty);
        }
    }
    public IReadOnlyCollection<ICondition> Conditions => _children.AsReadOnly();
    private List<ICondition> _children = new();
    private ExpressionType _logicalType;
    public ExpressionType LogicalType
    {
        get => _logicalType;
        set
        {
            _logicalType = value;
            ConditionChanged?.Invoke(this, EventArgs.Empty);
        }
    }
    public EventHandler ConditionChanged { get; set; }

    public BlockCondition(IEnumerable<ICondition> children, ExpressionType expressionType, ICondition? parent = null)
    {
        _children.AddRange(children);
        foreach (var condition in _children)
        {
            condition.Parent = this;
            condition.ConditionChanged += ChildConditionChanged;
        }
        _logicalType = expressionType;
        Parent = parent;
    }

    public ICondition GetRoot()
    {
        return Parent == null ? this : Parent.GetRoot();
    }
    public void Add(ICondition condition)
    {
        _children.Add(condition);
        condition.Parent = this;
        condition.ConditionChanged += ChildConditionChanged;
        ConditionChanged?.Invoke(this, EventArgs.Empty);
    }
    
    public void Remove(ICondition condition)
    {
        _children.Remove(condition);
        condition.Parent = null;
        if(condition.ConditionChanged?.GetInvocationList().Length > 0)
        {
            condition.ConditionChanged -= ChildConditionChanged;
        }
        ConditionChanged?.Invoke(this, EventArgs.Empty);
    }
    
    public Expression Compile()
    {
        var expressions = Conditions.Select(c => c.Compile()).ToList();

        Expression result = expressions.First();
        foreach (var expression in expressions.Skip(1))
        {
            result = Expression.MakeBinary(LogicalType, result, expression);
        }

        return result;
    }
    
    public void Group(IEnumerable<ICondition> childrenToGroup)
    {
        var children = Conditions.Where(c => childrenToGroup.Contains(c)).ToList();
        if (children.Count == 0)
        {
            return;
        }

        var block = new BlockCondition(children, children.First().LogicalType, this);

        foreach (var child in children)
        {
            _children.Remove(child);
            child.ConditionChanged -= ChildConditionChanged;
            child.Parent = block;
        }

        _children.Add(block);
        block.ConditionChanged += ChildConditionChanged;
        
        ConditionChanged?.Invoke(this, EventArgs.Empty);
    }

    private void ChildConditionChanged(object? sender, EventArgs args)
    {
        ConditionChanged?.Invoke(this, EventArgs.Empty);
    }

    public void Ungroup(IEnumerable<ICondition> childrenToUngroup)
    {
        var blocks = Conditions.OfType<BlockCondition>().Where(b => childrenToUngroup.Contains(b)).ToList();
        if (blocks.Count == 0)
        {
            return;
        }

        foreach (var block in blocks)
        {
            _children.Remove(block);
            block.ConditionChanged -= ChildConditionChanged;
            foreach (var blockCondition in block.Conditions)
            {
                blockCondition.ConditionChanged -= block.ChildConditionChanged;
            }
            _children.AddRange(block.Conditions);
            foreach (var condition in block.Conditions)
            {
                condition.Parent = this;
                condition.ConditionChanged += ChildConditionChanged;
            }
        }
        ConditionChanged?.Invoke(this, EventArgs.Empty);
    }

    public void CreateNew()
    {
        var lastLogicalCondition = FindLogicalCondition();
        var newLogicalCondition = new LogicalCondition
        {
            Parent = this,
            LogicalType = lastLogicalCondition.LogicalType,
            ComparaisonType = lastLogicalCondition.ComparaisonType,
            Left = lastLogicalCondition.Left.Copy(),
            Right = lastLogicalCondition.Right.Copy()
        };
        Add(newLogicalCondition);
    }
    private LogicalCondition FindLogicalCondition()
    {
        return Conditions.OfType<LogicalCondition>().FirstOrDefault() ?? Conditions.OfType<BlockCondition>().Select(c => c.FindLogicalCondition()).First();
    }
}

public interface IQuery
{
    BlockCondition Condition { get; }
    LambdaExpression Lambda { get; }
    ParameterExpression Parameter { get; }
    IEnumerable<PropertyPath> SelectedPropertyPaths { get; set; }
    
    IEnumerable<PropertyPath> AvailableProperties();
    void Compile();
    Task<IEnumerable> Execute(IEnumerable<PropertyPath>? selectedProperties);
}
public interface IQuery<TEntity>: IQuery where TEntity: class
{
}

public abstract class Query: IQuery
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

public abstract class Query<TEntity>: Query, IQuery<TEntity> where TEntity : class
{
    public Query()
    {
        Parameter = Expression.Parameter(
            typeof(TEntity),
            typeof(TEntity).Name.ToLower());

        Lambda = CreateRelationalPredicate<TEntity>(
            typeof(TEntity).GetProperties().First().Name,
            Parameter,
            typeof(TEntity).GetProperties().First().PropertyType.GetDefaultValue(),
            ExpressionType.Equal);
        if(Lambda.Body is BinaryExpression binaryExpression)
        {
            Condition = new BlockCondition(new []
            {
                new LogicalCondition()
                {
                    Left = binaryExpression.Left,
                    Right = binaryExpression.Right,
                    ComparaisonType = binaryExpression.NodeType
                }
            }, ExpressionType.And);}
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
        if(Lambda.Body is BinaryExpression binaryExpression)
        {
            Condition = new BlockCondition(new []
            {
                new LogicalCondition()
                {
                    Left = binaryExpression.Left,
                    Right = binaryExpression.Right,
                    ComparaisonType = binaryExpression.NodeType
                }
            }, ExpressionType.And);}
    }

    
    public override IEnumerable<PropertyPath> AvailableProperties()
    {
        return typeof(TEntity).GetProperties().Select(p => new PropertyPath(
                p.Name,
                p.PropertyType, 
                typeof(TEntity)))
            .ToList();

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