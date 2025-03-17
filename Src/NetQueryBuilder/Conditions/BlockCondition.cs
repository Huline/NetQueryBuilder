using System.Linq.Expressions;

namespace NetQueryBuilder.Conditions;

public class BlockCondition : ICondition
{
    private readonly List<ICondition> _children = new();
    private ExpressionType _logicalType;
    private ICondition? _parent;

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

    public ICondition? Parent
    {
        get => _parent;
        set
        {
            _parent = value;
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

    public EventHandler ConditionChanged { get; set; }

    public ICondition GetRoot()
    {
        return Parent == null ? this : Parent.GetRoot();
    }

    public Expression Compile()
    {
        var expressions = Conditions.Select(c => c.Compile()).ToList();

        var result = expressions.First();
        foreach (var expression in expressions.Skip(1)) result = Expression.MakeBinary(LogicalType, result, expression);

        return result;
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
        if (condition.ConditionChanged?.GetInvocationList().Length > 0) condition.ConditionChanged -= ChildConditionChanged;
        ConditionChanged?.Invoke(this, EventArgs.Empty);
    }

    public void Group(IEnumerable<ICondition> childrenToGroup)
    {
        var children = Conditions.Where(c => childrenToGroup.Contains(c)).ToList();
        if (children.Count == 0) return;

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
        if (blocks.Count == 0) return;

        foreach (var block in blocks)
        {
            _children.Remove(block);
            block.ConditionChanged -= ChildConditionChanged;
            foreach (var blockCondition in block.Conditions) blockCondition.ConditionChanged -= block.ChildConditionChanged;
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
        var newLogicalCondition = new LogicalCondition(lastLogicalCondition.PropertyPath, lastLogicalCondition.LogicalType);
        Add(newLogicalCondition);
    }

    private LogicalCondition FindLogicalCondition()
    {
        return Conditions.OfType<LogicalCondition>().FirstOrDefault() ?? Conditions.OfType<BlockCondition>().Select(c => c.FindLogicalCondition()).First();
    }
}