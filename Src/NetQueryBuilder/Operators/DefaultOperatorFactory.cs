using System;
using System.Collections.Generic;

namespace NetQueryBuilder.Operators
{
    public class DefaultOperatorFactory : IOperatorFactory
    {
        private readonly IExpressionStringifier _expressionStringifier;

        public DefaultOperatorFactory(IExpressionStringifier expressionStringifier)
        {
            _expressionStringifier = expressionStringifier;
        }

        public virtual IEnumerable<ExpressionOperator> GetAllForProperty(PropertyPath propertyPath)
        {
            if (propertyPath.PropertyType == typeof(int))
                return new List<ExpressionOperator>
                {
                    new EqualsOperator(_expressionStringifier),
                    new NotEqualsOperator(_expressionStringifier),
                    new LessThanOperator(_expressionStringifier),
                    new LessThanOrEqualOperator(_expressionStringifier),
                    new GreaterThanOperator(_expressionStringifier),
                    new GreaterThanOrEqualOperator(_expressionStringifier),
                    new InListOperator<int>(_expressionStringifier),
                    new InListOperator<int>(_expressionStringifier, true)
                };
            if (propertyPath.PropertyType == typeof(string))
                return new List<ExpressionOperator>
                {
                    new EqualsOperator(_expressionStringifier), new NotEqualsOperator(_expressionStringifier), new InListOperator<string>(_expressionStringifier), new InListOperator<string>(_expressionStringifier, true)
                };
            if (propertyPath.PropertyType == typeof(bool))
                return new List<ExpressionOperator> { new EqualsOperator(_expressionStringifier), new NotEqualsOperator(_expressionStringifier) };
            if (propertyPath.PropertyType == typeof(DateTime))
                return new List<ExpressionOperator>
                {
                    new EqualsOperator(_expressionStringifier),
                    new NotEqualsOperator(_expressionStringifier),
                    new LessThanOperator(_expressionStringifier),
                    new LessThanOrEqualOperator(_expressionStringifier),
                    new GreaterThanOperator(_expressionStringifier),
                    new GreaterThanOrEqualOperator(_expressionStringifier)
                };
            return new List<ExpressionOperator> { new EqualsOperator(_expressionStringifier), new NotEqualsOperator(_expressionStringifier) };
        }
    }
}