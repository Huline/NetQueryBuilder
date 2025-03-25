using System.Linq.Expressions;

namespace NetQueryBuilder.Conditions;

public class BlockCondition : ICondition
{
    private readonly List<ICondition> _children = new();
    private Expression? _compiledExpression;
    private LogicalOperator _logicalOperator;

    public BlockCondition(IEnumerable<ICondition> children, LogicalOperator logicalOperator, ICondition? parent = null)
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
    public ICondition? Parent { get; set; }

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
        foreach (var condition in Conditions.Skip(1))
            result = Expression.MakeBinary(ToExpression(condition.LogicalOperator), result, condition.Compile());

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

    public void Add(ICondition condition)
    {
        _children.Add(condition);
        condition.Parent = this;
        condition.ConditionChanged += ChildConditionChanged;
        NotifyConditionChanged();
    }

    public void Remove(ICondition condition)
    {
        _children.Remove(condition);
        condition.Parent = null;
        if (condition.ConditionChanged?.GetInvocationList().Length > 0)
            UnsubscribeChildCondition(condition);
        NotifyConditionChanged();
    }

    public void Group(IEnumerable<ICondition> childrenToGroup)
    {
        var children = Conditions.Where(childrenToGroup.Contains).ToList();
        if (children.Count == 0) return;

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
    }


    public void Ungroup(IEnumerable<ICondition> childrenToUngroup)
    {
        var blocks = Conditions.OfType<BlockCondition>().Where(childrenToUngroup.Contains).ToList();
        if (blocks.Count == 0) return;

        foreach (var block in blocks)
        {
            _children.Remove(block);
            UnsubscribeChildCondition(block);
            foreach (var blockChildCondition in block.Conditions)
                block.UnsubscribeChildCondition(blockChildCondition);
            _children.AddRange(block.Conditions);
            foreach (var condition in block.Conditions)
            {
                condition.Parent = this;
                condition.ConditionChanged += ChildConditionChanged;
            }
        }

        NotifyConditionChanged();
    }

    public void CreateNew()
    {
        var lastLogicalCondition = FindLogicalCondition();
        var newLogicalCondition = new SimpleCondition(lastLogicalCondition.PropertyPath, lastLogicalCondition.LogicalOperator);
        Add(newLogicalCondition);
    }

    private SimpleCondition FindLogicalCondition()
    {
        return Conditions.OfType<SimpleCondition>().FirstOrDefault() ?? Conditions.OfType<BlockCondition>().Select(c => c.FindLogicalCondition()).First();
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