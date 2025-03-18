using System.Linq.Expressions;

namespace NetQueryBuilder.Conditions;

public class BlockCondition : ICondition
{
    private readonly List<ICondition> _children = new();
    private Expression? _compiledExpression;
    private ExpressionType _logicalType;

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

    public IReadOnlyCollection<ICondition> Conditions => _children.AsReadOnly();
    public EventHandler ConditionChanged { get; set; }
    public ICondition? Parent { get; set; }

    public ExpressionType LogicalType
    {
        get => _logicalType;
        set
        {
            _logicalType = value;
            NotifyConditionChanged();
        }
    }


    public ICondition GetRoot()
    {
        return Parent == null ? this : Parent.GetRoot();
    }

    public Expression Compile()
    {
        if (_compiledExpression != null)
            return _compiledExpression;
        var expressions = Conditions.Select(c => c.Compile()).ToList();

        var result = expressions.First();
        foreach (var expression in expressions.Skip(1))
            result = Expression.MakeBinary(LogicalType, result, expression);

        return result;
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
        if (condition.ConditionChanged.GetInvocationList().Length > 0)
            UnsubscribeChildCondition(condition);
        NotifyConditionChanged();
    }

    public void Group(IEnumerable<ICondition> childrenToGroup)
    {
        var children = Conditions.Where(childrenToGroup.Contains).ToList();
        if (children.Count == 0) return;

        var block = new BlockCondition(children, children.First().LogicalType, this);

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
        var newLogicalCondition = new LogicalCondition(lastLogicalCondition.PropertyPath, lastLogicalCondition.LogicalType);
        Add(newLogicalCondition);
    }

    private LogicalCondition FindLogicalCondition()
    {
        return Conditions.OfType<LogicalCondition>().FirstOrDefault() ?? Conditions.OfType<BlockCondition>().Select(c => c.FindLogicalCondition()).First();
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
        ConditionChanged.Invoke(this, EventArgs.Empty);
    }
}