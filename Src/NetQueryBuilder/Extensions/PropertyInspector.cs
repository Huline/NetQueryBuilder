using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using NetQueryBuilder.Conditions;
using NetQueryBuilder.Operators;

namespace NetQueryBuilder.Extensions;

public static class PropertyInspector
{
    // Renvoyer tous les chemins de propriétés (propriétés "plates" => p.Address.City, etc.)
    public static IEnumerable<PropertyPath> GetAllPropertyPaths(
        Type type,
        ParameterExpression parameter,
        IOperatorFactory operatorFactory,
        string parentPath = "",
        HashSet<Type>? visitedTypes = null)
    {
        // Initialisation du HashSet si nécéssaire
        visitedTypes ??= new HashSet<Type>();

        // Si on a déjà vu ce type dans la chaîne courante, on arrête
        if (visitedTypes.Contains(type))
            yield break;

        // Ajout du type actuel à l'ensemble
        visitedTypes.Add(type);

        foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            var propertyPath = string.IsNullOrEmpty(parentPath)
                ? prop.Name
                : $"{parentPath}.{prop.Name}";

            // Vérifier s'il s'agit d'un type simple
            if (IsSimpleType(prop.PropertyType))
                yield return new PropertyPath(propertyPath, prop.PropertyType, type, parameter, operatorFactory);
            else if (!prop.PropertyType.IsAssignableTo(typeof(IEnumerable)))
                // Récupérer les sous-propriétés sans redéclencher une boucle
                foreach (var childPath in GetAllPropertyPaths(prop.PropertyType, parameter, operatorFactory, propertyPath, new HashSet<Type>(visitedTypes)))
                    yield return childPath;
        }

        // Une fois ce type traité, on peut le retirer si l’on veut poursuivre d’autres chemins indépendants
        // Mais si l’on souhaite éviter toute revisite sur l’arborescence globale, on peut choisir de ne pas l’enlever.
        visitedTypes.Remove(type);
    }


    // Déterminer si c’est un type “simple” (valeur, string, DateTime, etc.)
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