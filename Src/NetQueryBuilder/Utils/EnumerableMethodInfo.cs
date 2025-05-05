using System;
using System.Linq;
using System.Reflection;

namespace NetQueryBuilder.Utils
{
    public static class EnumerableMethodInfo
    {
        public static MethodInfo Select<TSource, TResult>()
        {
            return typeof(Enumerable)
                .GetGenericMethod(
                    nameof(Enumerable.Select), typeof(Func<,>),
                    null,
                    null)
                .MakeGenericMethod(typeof(TSource), typeof(TResult));
        }

        public static MethodInfo Any<TSource>()
        {
            return typeof(Enumerable)
                .GetGenericMethod(
                    nameof(Enumerable.Any),
                    typeof(Func<,>),
                    null,
                    typeof(bool))
                .MakeGenericMethod(typeof(TSource));
        }

        public static MethodInfo Contains<TSource>()
        {
            var method = typeof(Enumerable)
                .GetMethods(BindingFlags.Static | BindingFlags.Public)
                .First(m => m.Name == "Contains" && m.GetParameters().Length == 2);

            var methodGeneric = method.MakeGenericMethod(typeof(TSource));

            return methodGeneric;
        }

        private static MethodInfo GetGenericMethod(
            this Type type,
            string methodName,
            Type genericTypeDefinition,
            params Type[] genericTypeArgs)
        {
            return type
                .GetMethods()
                .Single(method =>
                    method.Name == methodName
                    && method.GetParameters()
                        .Any(p => p.ParameterType.IsGenericInstance(genericTypeDefinition, genericTypeArgs)));
        }
    }
}