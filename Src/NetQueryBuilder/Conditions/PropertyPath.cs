using System.Linq.Expressions;
using NetQueryBuilder.Extensions;
using NetQueryBuilder.Operators;

namespace NetQueryBuilder.Conditions;

public class PropertyPath
{
    private readonly IOperatorFactory _operatorFactory;
    private readonly ParameterExpression _parameterExpression;


    public PropertyPath(
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

        // Création de l'expression d'accès au membre
        return Expression.Property(_parameterExpression, PropertyName);

        // // Obtention de l'opérateur binaire par défaut pour ce type de propriété
        // var defaultOperator = GetCompatibleOperators()
        //     .OfType<BinaryOperator>()
        //     .First();
        //     
        // // Création d'une valeur par défaut pour ce type
        // var defaultValue = GetDefaultValueForType(PropertyType);
        //
        // // Création de l'expression binaire complète
        // return Expression.MakeBinary(
        //     defaultOperator.ExpressionType,
        //     memberExpression,
        //     defaultValue
        // );
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