using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NetQueryBuilder.Operators;
using NetQueryBuilder.Properties;

namespace NetQueryBuilder.Conditions
{
    public class SimpleCondition : ICondition
    {
        private Expression _compiledExpression;
        private bool _isCacheValid;
        private Expression _left;
        private LogicalOperator _logicalOperator;
        private ExpressionOperator _operator;
        private PropertyPath _propertyPath;
        private Expression _right;

        private object _value;

        public SimpleCondition(PropertyPath propertyPath, LogicalOperator logicalOperator, BlockCondition parent = null)
            : this(propertyPath, logicalOperator, propertyPath.GetCompatibleOperators().First(), propertyPath.GetDefaultValue(), parent)
        {
        }

        public SimpleCondition(PropertyPath propertyPath, LogicalOperator logicalOperator, ExpressionOperator @operator, object value, BlockCondition parent = null)
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
                InvalidateCache();
                NotifyConditionChanged();
            }
        }

        public object Value
        {
            get => _value;
            set
            {
                _value = value;
                _right = Expression.Constant(Value);
                InvalidateCache();
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
                InvalidateCache();
                NotifyConditionChanged();
            }
        }

        public LogicalOperator LogicalOperator
        {
            get => _logicalOperator;
            set
            {
                _logicalOperator = value;
                InvalidateCache();
                NotifyConditionChanged();
            }
        }

        public BlockCondition Parent { get; set; }

        public EventHandler ConditionChanged { get; set; }

        public Expression Compile()
        {
            // Return cached expression if still valid
            if (_isCacheValid && _compiledExpression != null)
                return _compiledExpression;

            // Recompile and cache
            _compiledExpression = _operator.ToExpression(_left, _right);
            _isCacheValid = true;
            return _compiledExpression;
        }

        public ICondition GetRoot()
        {
            return Parent == null ? this : Parent.GetRoot();
        }

        public IEnumerable<ExpressionOperator> AvailableOperatorsForCurrentProperty()
        {
            return PropertyPath.GetCompatibleOperators();
        }

        private void InvalidateCache()
        {
            _isCacheValid = false;
        }

        private void NotifyConditionChanged()
        {
            ConditionChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}