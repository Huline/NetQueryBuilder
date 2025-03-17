using System.Linq.Expressions;

namespace NetQueryBuilder.Queries;

public class SelectBuilderService<TEntity>
{
    public Expression<Func<TEntity, TEntity>> BuildSelect(IEnumerable<string> propertyPaths)
    {
        var param = Expression.Parameter(typeof(TEntity), "entity");
        var newEntity = Expression.New(typeof(TEntity));
        var bindings = new List<MemberBinding>();

        /*
         * Étape 1 : Regrouper les propriétés par leur "premier segment".
         * Exemple :
         *   "FirstName" -> groupe "FirstName",
         *   "Address.City" -> groupe "Address" avec sous-propriété "City",
         *   "Address.Country.Name" -> groupe "Address" avec sous-propriété "Country.Name".
         */
        var groupedPaths = propertyPaths
            .Select(path => new { _original = path, parts = path.Split('.') })
            .GroupBy(x => x.parts[0]); // clé = premier segment (p. ex. "Address" ou "FirstName")

        foreach (var group in groupedPaths)
        {
            // Nom de la propriété de premier niveau
            var topPropName = group.Key;
            var topPropertyInfo = typeof(TEntity).GetProperty(topPropName);
            if (topPropertyInfo == null)
                continue; // la propriété n’existe pas dans TEntity

            // Identifier si c’est un type simple (int, string, etc.) ou un type complexe
            var isSimple = IsSimpleType(topPropertyInfo.PropertyType);

            // Si c’est un type simple ou qu’il n’y a pas de sous-propriétés, on crée un binding direct
            if (isSimple || group.All(x => x.parts.Length == 1))
            {
                // entity => entity.<topPropName>
                var memberAccess = Expression.Property(param, topPropertyInfo);
                var assignment = Expression.Bind(topPropertyInfo, memberAccess);
                bindings.Add(assignment);
            }
            else
            {
                // Type complexe : construire un objet de ce type avec seulement les propriétés demandées
                // Récupérer la liste des chemins restants (tout ce qui est après "Address.")
                var subPropertyNames = group
                    .Where(x => x.parts.Length > 1)
                    .Select(x => string.Join('.', x.parts.Skip(1))) // ex: "City" ou "Country.Name"
                    .Distinct();

                // On construit l’expression pour le type complexe
                // Exemple : new Address { City = entity.Address.City, Country = ... }
                var subInstance = Expression.Property(param, topPropertyInfo);
                var complexInit = BuildSubSelect(topPropertyInfo.PropertyType, subInstance, subPropertyNames);

                var assignment = Expression.Bind(topPropertyInfo, complexInit);
                bindings.Add(assignment);
            }
        }

        var memberInit = Expression.MemberInit(newEntity, bindings);
        return Expression.Lambda<Func<TEntity, TEntity>>(memberInit, param);
    }

    /// <summary>
    ///     Construit une initialisation d’objet pour un type “subType”,
    ///     en décidant quelles propriétés (ou sous-propriétés) on associe.
    /// </summary>
    private Expression BuildSubSelect(Type subType, Expression subInstance, IEnumerable<string> propertyPaths)
    {
        var newSub = Expression.New(subType);
        var subBindings = new List<MemberBinding>();

        // On regroupe de la même manière que pour BuildSelect (compile les chemins par premier segment)
        var groupedPaths = propertyPaths
            .Select(path => new { original = path, parts = path.Split('.') })
            .GroupBy(x => x.parts[0]);

        foreach (var group in groupedPaths)
        {
            var propName = group.Key;
            var propInfo = subType.GetProperty(propName);
            if (propInfo == null)
                continue;

            var isSimple = IsSimpleType(propInfo.PropertyType);

            if (isSimple || group.All(x => x.parts.Length == 1))
            {
                // Accès direct : newSub.PropName = subInstance.PropName
                var memberAccess = Expression.Property(subInstance, propInfo);
                var assignment = Expression.Bind(propInfo, memberAccess);
                subBindings.Add(assignment);
            }
            else
            {
                // Si c’est un type complexe avec sous-propriétés
                var deeperPaths = group
                    .Where(x => x.parts.Length > 1)
                    .Select(x => string.Join('.', x.parts.Skip(1)))
                    .Distinct();

                var deeperInstance = Expression.Property(subInstance, propInfo);
                var deeperInit = BuildSubSelect(propInfo.PropertyType, deeperInstance, deeperPaths);
                var assignment = Expression.Bind(propInfo, deeperInit);
                subBindings.Add(assignment);
            }
        }

        return Expression.MemberInit(newSub, subBindings);
    }

    // Vérifie si le type est “simple” (type valeur, string, etc.)
    private bool IsSimpleType(Type type)
    {
        return type.IsPrimitive
               || type.IsEnum
               || type == typeof(string)
               || type == typeof(decimal)
               || type == typeof(int)
               || type == typeof(DateTime)
               || type == typeof(DateTimeOffset)
               || type == typeof(TimeSpan)
               || type == typeof(Guid);
    }
}