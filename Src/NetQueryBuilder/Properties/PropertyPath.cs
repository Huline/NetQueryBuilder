using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using NetQueryBuilder.Operators;
using NetQueryBuilder.Utils;

namespace NetQueryBuilder.Properties
{
    /// <summary>
    ///     Represents a path to a specific property in an object graph, including its name, type, and additional metadata.
    /// </summary>
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

        /// <summary>
        ///     Constructs a MemberExpression representing the path to a property in the object graph
        ///     defined by the PropertyFullName of the current PropertyPath instance.
        /// </summary>
        /// <returns>
        ///     A MemberExpression representing the property access expression. If the property path
        ///     contains nested properties, the expression will represent the entire path. For example,
        ///     for a PropertyFullName 'PropA.PropB.PropC', the result would represent accessing
        ///     'PropC' in 'PropB' of 'PropA'.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        ///     Thrown if the ParameterExpression associated with the PropertyPath is null. Ensure the
        ///     instance is properly initialized by calling SetParameterExpression.
        /// </exception>
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

        /// <summary>
        ///     Retrieves the default value for the property type represented by the current PropertyPath instance.
        /// </summary>
        /// <returns>
        ///     The default value for the type of the property associated with this PropertyPath instance.
        ///     For reference types, this will typically be null. For value types, this will be the default
        ///     value (e.g., 0 for integers, false for booleans).
        /// </returns>
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

        /// <summary>
        ///     Retrieves a collection of ExpressionOperators that are compatible with the current instance of PropertyPath.
        ///     The compatibility is determined based on the type and characteristics of the associated property.
        /// </summary>
        /// <returns>
        ///     An IEnumerable of ExpressionOperator objects that can be used with the property represented by this
        ///     PropertyPath instance.
        /// </returns>
        public IEnumerable<ExpressionOperator> GetCompatibleOperators()
        {
            return _operatorFactory.GetAllForProperty(this);
        }

        public string DisplayName()
        {
            return _propertyStringifier?.GetName(PropertyFullName) ?? PropertyFullName;
        }

        /// <summary>
        ///     Retrieves the value of a property represented by the current PropertyPath instance
        ///     from the given context object and formats it as a string using the configured
        ///     IPropertyStringifier implementation, if available.
        /// </summary>
        /// <param name="context">
        ///     The root object from which the property's value will be extracted.
        ///     This object must have a structure that corresponds to the PropertyFullName
        ///     represented by the PropertyPath instance.
        /// </param>
        /// <returns>
        ///     A string representation of the property's value. If the property's value cannot
        ///     be retrieved or formatted, an empty string is returned. If a custom IPropertyStringifier
        ///     is configured, it will format the value; otherwise, the value is converted to a string
        ///     using its default `ToString` implementation.
        /// </returns>
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

        /// <summary>
        ///     Splits the PropertyFullName of the current PropertyPath instance into its hierarchical components,
        ///     returning a PathDescriptor that represents these parts of the object graph.
        /// </summary>
        /// <returns>
        ///     A PathDescriptor containing the individual parts of the PropertyFullName, separated by the PropertyPathSeparator.
        ///     Each part represents a portion of the overall property path hierarchy.
        /// </returns>
        internal PathDescriptor GetParts()
        {
            var parts = PropertyFullName.Split(PropertyPathSeparator);
            return new PathDescriptor(parts);
        }

        /// <summary>
        ///     Retrieves the navigation path within the object hierarchy by removing the last segment
        ///     of the full property path.
        /// </summary>
        /// <returns>
        ///     The navigation path as a string, excluding the leaf property. If the full property path
        ///     is 'Parent.Child.Property', the result will be 'Parent.Child'. If there are no navigable
        ///     components in the property path, an empty string is returned.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Thrown if the property path does not include any hierarchy separators, indicating that
        ///     the navigation path cannot be derived.
        /// </exception>
        public string GetNavigationPath()
        {
            return PropertyFullName.Substring(0, PropertyFullName.LastIndexOf('.'));
        }
    }
}