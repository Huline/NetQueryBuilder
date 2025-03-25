using System.Linq.Expressions;

namespace NetQueryBuilder.Utils;

internal static class TypeExtensions
{
    internal static T GetDefaultValue<T>()
    {
        var e = Expression.Lambda<Func<T>>(Expression.Default(typeof(T)));
        return e.Compile()();
    }

    internal static object GetDefaultValue(this Type type)
    {
        ArgumentNullException.ThrowIfNull(type);
        var e = Expression.Lambda<Func<object>>(Expression.Convert(Expression.Default(type), typeof(object)));
        return e.Compile()();
    }

    internal static bool IsGenericInstance(this Type type, Type genTypeDef, params Type?[] args)
    {
        if (type.GetGenericTypeDefinition() != genTypeDef)
            return false;

        var typeArgs = type.GetGenericArguments();

        if (typeArgs.Length != args.Length)
            return false;

        // Go through the arguments passed in, interpret nulls as "any type"
        for (var i = 0; i != args.Length; i++)
        {
            if (args[i] == null)
                continue;
            if (args[i] != typeArgs[i])
                return false;
        }

        return true;
    }
}