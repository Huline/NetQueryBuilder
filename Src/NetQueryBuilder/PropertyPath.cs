using System.Linq.Expressions;
using NetQueryBuilder.Extensions;
using NetQueryBuilder.Operators;

namespace NetQueryBuilder;

public class PropertyPath
{
    private readonly IOperatorFactory _operatorFactory;
    private readonly ParameterExpression _parameterExpression;


    internal PropertyPath(
        string propertyName,
        Type propertyType,
        Type parentType,
        ParameterExpression parameterExpression,
        IOperatorFactory operatorFactory)
    {
        _parameterExpression = parameterExpression;
        _operatorFactory = operatorFactory;
        PropertyName = propertyName;
        ParentType = parentType;
        PropertyType = propertyType;
    }

    public string PropertyName { get; }
    public Type ParentType { get; }
    public Type PropertyType { get; }


    public MemberExpression GetExpression()
    {
        if (_parameterExpression == null)
            throw new InvalidOperationException("Le ParameterExpression n'a pas été défini. Appelez SetParameterExpression d'abord.");

        if (!PropertyName.Contains('.'))
            return Expression.Property(_parameterExpression, PropertyName);
        var parts = PropertyName.Split('.');
        Expression expr = _parameterExpression;

        foreach (var part in parts) expr = Expression.Property(expr, part);

        return (MemberExpression)expr;
    }

    public object GetDefaultValue()
    {
        return GetDefaultValueForType(PropertyType).Type.GetDefaultValue();
    }

    // Méthode utilitaire pour obtenir une valeur par défaut pour un type donné
    private static Expression GetDefaultValueForType(Type propertyType)
    {
        return propertyType switch
        {
            Type type when
                type == typeof(int)
                || type == typeof(long)
                || type == typeof(string)
                || type == typeof(bool) => Expression.Constant(propertyType.GetDefaultValue(), propertyType),
            Type type when
                type == typeof(DateTime) => Expression.Constant(DateTime.UtcNow),
            _ => throw new Exception("Type de propriété non pris en charge")
        };
    }

    public override bool Equals(object? obj)
    {
        if (obj is PropertyPath other) return string.Equals(PropertyName, other.PropertyName, StringComparison.Ordinal);
        return false;
    }

    public override int GetHashCode()
    {
        return PropertyName.GetHashCode(StringComparison.Ordinal);
    }

    public IEnumerable<ExpressionOperator> GetCompatibleOperators()
    {
        return _operatorFactory.GetAllForProperty(this);
    }
}