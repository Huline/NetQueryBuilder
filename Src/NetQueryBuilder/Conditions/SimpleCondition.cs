using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NetQueryBuilder.Operators;

namespace NetQueryBuilder.Conditions
{
    public class SimpleCondition : ICondition
    {
        private Expression? _compiledExpression;
        private Expression _left;
        private LogicalOperator _logicalOperator;
        private ExpressionOperator _operator;
        private PropertyPath _propertyPath;
        private Expression _right;

        private object? _value;

        public SimpleCondition(PropertyPath propertyPath, LogicalOperator logicalOperator, BlockCondition? parent = null)
            : this(propertyPath, logicalOperator, propertyPath.GetCompatibleOperators().First(), propertyPath.GetDefaultValue(), parent)
        {
        }

        public SimpleCondition(PropertyPath propertyPath, LogicalOperator logicalOperator, ExpressionOperator @operator, object value, BlockCondition? parent = null)
        {
            _propertyPath = propertyPath;
            _logicalOperator = logicalOperator;
            _operator = @operator;
            _value = value;
            _left = PropertyPath.GetExpression();
            _right = Expression.Constant(Value);
            Parent = parent;
        }

        public PropertyPath PropertyPath
        {
            get => _propertyPath;
            set
            {
                _propertyPath = value;
                _left = PropertyPath.GetExpression();
                _value = PropertyPath.GetDefaultValue();
                _right = Expression.Constant(_value);
                NotifyConditionChanged();
            }
        }

        public object? Value
        {
            get => _value;
            set
            {
                _value = value;
                _right = Expression.Constant(Value);
                NotifyConditionChanged();
            }
        }

        public ExpressionOperator Operator
        {
            get => _operator;
            set
            {
                _operator = value;
                _value = _operator.GetDefaultValue(PropertyPath.PropertyType, _value);
                _right = Expression.Constant(Value);
                NotifyConditionChanged();
            }
        }

        public LogicalOperator LogicalOperator
        {
            get => _logicalOperator;
            set
            {
                _logicalOperator = value;
                NotifyConditionChanged();
            }
        }

        public BlockCondition? Parent { get; set; }

        public EventHandler? ConditionChanged { get; set; }

        public Expression? Compile()
        {
            if (_compiledExpression != null)
                return _compiledExpression;
            return _operator.ToExpression(_left, _right);
        }

        public ICondition GetRoot()
        {
            return Parent == null ? this : Parent.GetRoot();
        }

        public IEnumerable<ExpressionOperator> AvailableOperatorsForCurrentProperty()
        {
            return PropertyPath.GetCompatibleOperators();
        }

        private void NotifyConditionChanged()
        {
            _compiledExpression = null;
            _compiledExpression = Compile();
            ConditionChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}