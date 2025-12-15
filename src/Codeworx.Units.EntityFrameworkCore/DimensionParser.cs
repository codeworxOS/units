
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using Codeworx.Units.Primitives;

namespace Codeworx.Units.EntityFrameworkCore
{
    public static class DimensionParser
    {
        private static ConcurrentDictionary<Type, Func<string, decimal, IUnitBase>> tmp = new ConcurrentDictionary<Type, Func<string, decimal, IUnitBase>>();

        internal static T Get<T>(string unitKey, decimal value)
            where T : IUnitBase
        {
            var func = tmp.GetOrAdd(typeof(T), GetParseMethod);

            return func(unitKey, value) is T val ? val : throw new InvalidOperationException($"Invalid Conversion for Type {typeof(T)}, Key: {unitKey}, Value: {value}");
        }

        private static Func<string, decimal, IUnitBase> GetParseMethod(Type type)
        {
            var methodInfo = type.GetMethods(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public).Single(FindParseMethod);

            var param1 = Expression.Parameter(typeof(string), "key");
            var param2 = Expression.Parameter(typeof(decimal), "value");

            var method = Expression.Call(null, methodInfo, param1, param2);

            return (Expression.Lambda(method, param1, param2).Compile() as Func<string, decimal, IUnitBase>)!;
        }

        private static bool FindParseMethod(System.Reflection.MethodInfo d)
        {
            if (d.Name != "Parse")
                return false;

            var parameters = d.GetParameters();
            if (parameters.Length != 2)
                return false;

            if (parameters[0].ParameterType != typeof(string) || parameters[1].ParameterType != typeof(decimal))
                return false;

            return true;
        }
    }
}