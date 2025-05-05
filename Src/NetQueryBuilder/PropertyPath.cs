using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NetQueryBuilder.Operators;
using NetQueryBuilder.Utils;

namespace NetQueryBuilder
{
    public class SelectPropertyPath
    {
        public SelectPropertyPath(PropertyPath propertyPath, bool isSelected = true)
        {
            IsSelected = isSelected;
            Property = propertyPath;
        }

        public bool IsSelected { get; set; }
        public PropertyPath Property { get; }
    }

    public class PropertyPath
    {
        private const char PropertyPathSeparator = '.';
        private readonly IOperatorFactory _operatorFactory;
        private readonly ParameterExpression _parameterExpression;
        private readonly IPropertyStringifier _propertyStringifier;

        internal PropertyPath(
            string propertyFullName,
            Type propertyType,
            Type parentType,
            ParameterExpression parameterExpression,
            IPropertyStringifier propertyStringifier,
            IOperatorFactory operatorFactory)
        {
            _parameterExpression = parameterExpression;
            _propertyStringifier = propertyStringifier;
            _operatorFactory = operatorFactory;
            PropertyFullName = propertyFullName;
            ParentType = parentType;
            PropertyType = propertyType;
            PropertyName = propertyFullName.Split(PropertyPathSeparator).Last();
            Depth = propertyFullName.Split(PropertyPathSeparator).Length - 1;
        }

        public string PropertyFullName { get; }
        public string PropertyName { get; }
        public Type ParentType { get; }
        public Type PropertyType { get; }
        public int Depth { get; }
        public bool HasDeepth => Depth > 0;

        public MemberExpression GetExpression()
        {
            if (_parameterExpression == null)
                throw new InvalidOperationException("Le ParameterExpression n'a pas été défini. Appelez SetParameterExpression d'abord.");

            if (!PropertyFullName.Contains(PropertyPathSeparator.ToString()))
                return Expression.Property(_parameterExpression, PropertyFullName);
            var parts = PropertyFullName.Split(PropertyPathSeparator);
            Expression expr = _parameterExpression;

            foreach (var part in parts) expr = Expression.Property(expr, part);

            return (MemberExpression)expr;
        }

        public object GetDefaultValue()
        {
            return GetDefaultValueForType(PropertyType).Type.GetDefaultValue();
        }

        private static Expression GetDefaultValueForType(Type propertyType)
        {
            if (propertyType == typeof(int) || propertyType == typeof(long) || propertyType == typeof(string) || propertyType == typeof(bool))
                return Expression.Constant(propertyType.GetDefaultValue(), propertyType);
            if (propertyType == typeof(DateTime))
                return Expression.Constant(DateTime.UtcNow);
            throw new Exception("Type de propriété non pris en charge");
        }

        public override bool Equals(object obj)
        {
            if (obj is PropertyPath other) return string.Equals(PropertyFullName, other.PropertyFullName, StringComparison.Ordinal);
            return false;
        }

        public override int GetHashCode()
        {
            return PropertyFullName.GetHashCode();
        }

        public IEnumerable<ExpressionOperator> GetCompatibleOperators()
        {
            return _operatorFactory.GetAllForProperty(this);
        }

        public string DisplayName()
        {
            return _propertyStringifier?.GetName(PropertyFullName) ?? PropertyFullName;
        }

        public string DisplayValue(object context)
        {
            var segments = PropertyFullName.Split(PropertyPathSeparator);
            var currentObject = context;

            foreach (var segment in segments)
            {
                if (currentObject == null) break;

                var propInfo = currentObject.GetType().GetProperty(segment);
                currentObject = propInfo?.GetValue(currentObject);
            }

            return _propertyStringifier?.FormatValue(PropertyFullName, PropertyType, currentObject) ?? currentObject?.ToString() ?? string.Empty;
        }

        internal PathDescriptor GetParts()
        {
            var parts = PropertyFullName.Split(PropertyPathSeparator);
            return new PathDescriptor(parts);
        }

        public string GetNavigationPath()
        {
            return PropertyFullName.Substring(0, PropertyFullName.LastIndexOf('.'));
        }
    }

    internal class PathDescriptor
    {
        private readonly string[] _parts;

        public PathDescriptor(string[] parts)
        {
            _parts = parts;
        }

        public bool HasChild => _parts.Length > 1;

        public string GetCurrentPath()
        {
            return _parts.FirstOrDefault();
        }

        public PathDescriptor GetChildPath()
        {
            return new PathDescriptor(_parts.Skip(1).ToArray());
        }
    }
}