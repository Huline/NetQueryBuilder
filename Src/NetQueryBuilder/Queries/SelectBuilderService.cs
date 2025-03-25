using System.Linq.Expressions;
using System.Reflection;

namespace NetQueryBuilder.Queries;

public class SelectBuilderService<TEntity>
{
    public Expression<Func<TEntity, TEntity>> BuildSelect(IEnumerable<string> propertyPaths)
    {
        var param = Expression.Parameter(typeof(TEntity), "entity");
        var newEntity = Expression.New(typeof(TEntity));

        var propertiesGroupedPaths = propertyPaths
            .Select(path => new PathDescriptor(path, path.Split('.')))
            .GroupBy(x => x.Parts[0]);

        var bindings = propertiesGroupedPaths
            .Select(propertyPathGroup => CreateBindingsForProperties(propertyPathGroup, param))
            .Where(mb => mb != null)
            .OfType<MemberBinding>()
            .ToList();

        var memberInit = Expression.MemberInit(newEntity, bindings);
        return Expression.Lambda<Func<TEntity, TEntity>>(memberInit, param);
    }


    private static MemberBinding? CreateBindingsForProperties(IGrouping<string, PathDescriptor> propertyPathGroup, ParameterExpression param)
    {
        var topPropName = propertyPathGroup.Key;
        var topPropertyInfo = typeof(TEntity).GetProperty(topPropName);
        if (topPropertyInfo == null)
            return null;

        var isSimple = IsSimpleType(topPropertyInfo.PropertyType);

        if (isSimple || HasOnlyOnePath(propertyPathGroup))
            return DirectBindingForProperty(param, topPropertyInfo);

        return BindingsForSubProperty(propertyPathGroup, param, topPropertyInfo);
    }

    private static bool HasOnlyOnePath(IGrouping<string, PathDescriptor> propertyPathGroup)
    {
        return propertyPathGroup.All(x => x.Parts.Length == 1);
    }

    private static MemberBinding BindingsForSubProperty(IGrouping<string, PathDescriptor> group, Expression param, PropertyInfo topPropertyInfo)
    {
        var subPropertyNames = group
            .Where(x => x.Parts.Length > 1)
            .Select(x => string.Join('.', x.Parts.Skip(1)))
            .Distinct();

        var subInstance = Expression.Property(param, topPropertyInfo);
        var complexInit = BuildSubSelect(topPropertyInfo.PropertyType, subInstance, subPropertyNames);
        var assignment = Expression.Bind(topPropertyInfo, complexInit);
        return assignment;
    }

    private static MemberBinding DirectBindingForProperty(Expression param, PropertyInfo topPropertyInfo)
    {
        var memberAccess = Expression.Property(param, topPropertyInfo);
        var assignment = Expression.Bind(topPropertyInfo, memberAccess);
        return assignment;
    }

    private static Expression BuildSubSelect(Type subType, Expression subInstance, IEnumerable<string> propertyPaths)
    {
        var newSub = Expression.New(subType);

        var groupedPaths = propertyPaths
            .Select(path => new PathDescriptor(path, path.Split('.')))
            .GroupBy(x => x.Parts[0]);

        var subBindings = groupedPaths
            .Select(group => CreateSubBindingsForSubProperties(subType, subInstance, group))
            .Where(mb => mb != null)
            .OfType<MemberBinding>()
            .ToList();

        return Expression.MemberInit(newSub, subBindings);
    }

    private static MemberBinding? CreateSubBindingsForSubProperties(Type subType, Expression subInstance, IGrouping<string, PathDescriptor> propertyPathGroup)
    {
        var propName = propertyPathGroup.Key;
        var propInfo = subType.GetProperty(propName);
        if (propInfo == null)
            return null;

        var isSimple = IsSimpleType(propInfo.PropertyType);

        if (isSimple || HasOnlyOnePath(propertyPathGroup))
            return DirectBindingForProperty(subInstance, propInfo);

        return BindingsForSubProperty(propertyPathGroup, subInstance, propInfo);
    }

    // Vérifie si le type est “simple” (type valeur, string, etc.)
    private static bool IsSimpleType(Type type)
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

    private record PathDescriptor(object Original, string[] Parts);
}