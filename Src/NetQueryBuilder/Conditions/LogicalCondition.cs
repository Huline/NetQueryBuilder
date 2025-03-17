using System.Linq.Expressions;
using NetQueryBuilder.Operators;

namespace NetQueryBuilder.Conditions;

public class LogicalCondition : ICondition
{
    private Expression _left;
    private ExpressionType _logicalType;
    private ExpressionOperator _operator;
    private PropertyPath _propertyPath;
    private Expression _right;

    public LogicalCondition(PropertyPath propertyPath, ExpressionType logicalType, ICondition? parent = null)
        : this(propertyPath, logicalType, propertyPath.GetCompatibleOperators().First(), propertyPath.GetDefaultValue(), parent)
    {
    }

    public LogicalCondition(PropertyPath propertyPath, ExpressionType logicalType, ExpressionOperator @operator, object value, ICondition? parent = null)
    {
        PropertyPath = propertyPath;
        LogicalType = logicalType;
        Operator = @operator;
        Value = value;
        Parent = parent;
        Left = PropertyPath.GetExpression();
        Right = Expression.Constant(Value);
    }

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

    public Expression Left
    {
        get => _left;
        set
        {
            _left = value;
            ConditionChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public Expression Right
    {
        get => _right;
        set
        {
            _right = value;
            ConditionChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public ExpressionOperator Operator
    {
        get => _operator;
        set
        {
            _operator = value;
            ConditionChanged?.Invoke(this, EventArgs.Empty);
        }
    }

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

    public EventHandler ConditionChanged { get; set; }

    public Expression Compile()
    {
        return Expression.MakeBinary(Operator.ExpressionType, _left, _right);
    }

    public ICondition GetRoot()
    {
        return Parent == null ? this : Parent.GetRoot();
    }

    public IEnumerable<ExpressionOperator> AvailableOperatorsForCurrentProperty()
    {
        return PropertyPath.GetCompatibleOperators();
    }
}