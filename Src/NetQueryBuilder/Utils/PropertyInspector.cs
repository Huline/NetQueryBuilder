using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using NetQueryBuilder.Operators;

namespace NetQueryBuilder.Utils
{
    internal static class PropertyInspector
    {
        internal static IEnumerable<PropertyPath> GetAllPropertyPaths(Type type,
            ParameterExpression parameter,
            IPropertyStringifier propertyStringifier,
            IOperatorFactory operatorFactory,
            string parentPath = "",
            HashSet<Type> visitedTypes = null)
        {
            if (visitedTypes == null) visitedTypes = new HashSet<Type>();

            if (!visitedTypes.Add(type))
                yield break;

            foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var propertyPath = string.IsNullOrEmpty(parentPath)
                    ? prop.Name
                    : $"{parentPath}.{prop.Name}";

                if (IsSimpleType(prop.PropertyType))
                    yield return new PropertyPath(propertyPath, prop.PropertyType, type, parameter, propertyStringifier, operatorFactory);
                else if (!typeof(IEnumerable).IsAssignableFrom(prop.PropertyType))
                    foreach (var childPath in GetAllPropertyPaths(prop.PropertyType, parameter, propertyStringifier, operatorFactory, propertyPath, new HashSet<Type>(visitedTypes)))
                        yield return childPath;
            }

            visitedTypes.Remove(type);
        }

        private static bool IsSimpleType(Type type)
        {
            return type.IsPrimitive
                   || type.IsEnum
                   || type == typeof(string)
                   || type == typeof(decimal)
                   || type == typeof(int)
                   || type == typeof(float)
                   || type == typeof(bool)
                   || type == typeof(DateTime)
                   || type == typeof(DateTimeOffset)
                   || type == typeof(TimeSpan)
                   || type == typeof(Guid);
        }
    }
}