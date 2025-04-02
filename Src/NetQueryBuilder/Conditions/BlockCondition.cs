using System.Linq.Expressions;
using NetQueryBuilder.Operators;

namespace NetQueryBuilder.Conditions;

public class BlockCondition : ICondition
{
    private readonly List<ICondition> _children = new();
    private Expression? _compiledExpression;
    private LogicalOperator _logicalOperator;

    public BlockCondition(IEnumerable<ICondition> children, LogicalOperator logicalOperator, BlockCondition? parent = null)
    {
        _children.AddRange(children);
        foreach (var condition in _children)
        {
            condition.Parent = this;
            condition.ConditionChanged += ChildConditionChanged;
        }

        _logicalOperator = logicalOperator;
        Parent = parent;
    }

    public IReadOnlyCollection<ICondition> Conditions => _children.AsReadOnly();
    public EventHandler? ConditionChanged { get; set; }
    public BlockCondition? Parent { get; set; }

    public LogicalOperator LogicalOperator
    {
        get => _logicalOperator;
        set
        {
            _logicalOperator = value;
            NotifyConditionChanged();
        }
    }


    public ICondition GetRoot()
    {
        return Parent == null ? this : Parent.GetRoot();
    }

    public Expression? Compile()
    {
        if (_compiledExpression != null)
            return _compiledExpression;

        if (Conditions.Count == 0)
            return null;
        var result = Conditions.First().Compile();
        if (result is null)
            return null;

        foreach (var condition in Conditions.Skip(1))
        {
            var compiled = condition.Compile();
            if(compiled is not null)
                result = Expression.MakeBinary(ToExpression(condition.LogicalOperator), result, compiled);
        }

        return result;
    }

    private static ExpressionType ToExpression(LogicalOperator logicalOperator)
    {
        return logicalOperator switch
        {
            LogicalOperator.And => ExpressionType.AndAlso,
            LogicalOperator.Or => ExpressionType.OrElse,
            _ => throw new ArgumentOutOfRangeException(nameof(logicalOperator), logicalOperator, null)
        };
    }

    public ICondition Add(ICondition condition)
    {
        _children.Add(condition);
        condition.Parent = this;
        condition.ConditionChanged += ChildConditionChanged;
        NotifyConditionChanged();
        return condition;
    }

    public void Remove(ICondition condition)
    {
        _children.Remove(condition);
        condition.Parent = null;
        if (condition.ConditionChanged?.GetInvocationList().Length > 0)
            UnsubscribeChildCondition(condition);
        NotifyConditionChanged();
    }

    public BlockCondition? Group(IEnumerable<ICondition> childrenToGroup)
    {
        var children = Conditions.Where(childrenToGroup.Contains).ToList();
        if (children.Count == 0) return null;

        var block = new BlockCondition(children, children.First().LogicalOperator, this);

        foreach (var child in children)
        {
            _children.Remove(child);
            UnsubscribeChildCondition(child);
            child.Parent = block;
        }

        _children.Add(block);
        block.ConditionChanged += ChildConditionChanged;

        NotifyConditionChanged();
        return block;
    }


    public void Ungroup(IEnumerable<ICondition> childrenToUngroup)
    {
        var conditions = Conditions.Where(childrenToUngroup.Contains).ToList();
        if(this.Parent == null)
            return; 
        if (conditions.Count == 0) return;

        foreach (var condition in conditions)
        {
            _children.Remove(condition);
            UnsubscribeChildCondition(condition);
            condition.Parent = this.Parent;
            this.Parent._children.Add(condition);
            condition.ConditionChanged -= ChildConditionChanged;
            condition.ConditionChanged += Parent.ChildConditionChanged;
        }

        if (_children.Count == 0)
        {
            Parent.Remove(this);
        }

        NotifyConditionChanged();
    }

    public SimpleCondition CreateNew<TOperator>(PropertyPath property, object? valueOverride = null)
    where TOperator : ExpressionOperator
    {
        return CreateNew(property, property.GetCompatibleOperators().OfType<TOperator>().FirstOrDefault(), valueOverride);
    }
    
    
    public SimpleCondition CreateNew(PropertyPath property, ExpressionOperator? operatorOverride = null, object? valueOverride = null)
    {
        var newLogicalCondition = new SimpleCondition(property, LogicalOperator.And);
        if (operatorOverride != null)
            newLogicalCondition.Operator = operatorOverride;
        if (valueOverride != null)
            newLogicalCondition.Value = valueOverride;
        return (Add(newLogicalCondition) as SimpleCondition)!;
    }
    
    public SimpleCondition CreateNew()
    {
        var lastLogicalCondition = FindLogicalCondition();
        if (lastLogicalCondition == null)
        {
            throw new InvalidOperationException("No logical condition found to create a new one. Use CreateNew(PropertyPath property) instead.");
        }
        var newLogicalCondition = new SimpleCondition(lastLogicalCondition.PropertyPath, lastLogicalCondition.LogicalOperator);
        return (Add(newLogicalCondition) as SimpleCondition)!;
    }

    private SimpleCondition? FindLogicalCondition()
    {
        return Conditions.OfType<SimpleCondition>().FirstOrDefault() ?? Conditions.OfType<BlockCondition>().Select(c => c.FindLogicalCondition()).FirstOrDefault();
    }

    private void ChildConditionChanged(object? sender, EventArgs args)
    {
        NotifyConditionChanged();
    }

    private void UnsubscribeChildCondition(ICondition child)
    {
        child.ConditionChanged -= ChildConditionChanged;
    }

    private void NotifyConditionChanged()
    {
        _compiledExpression = null;
        _compiledExpression = Compile();
        ConditionChanged?.Invoke(this, EventArgs.Empty);
    }
}