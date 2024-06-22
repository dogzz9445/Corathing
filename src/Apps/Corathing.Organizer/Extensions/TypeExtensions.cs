using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corathing.Organizer.Extensions;

public static class TypeExtensions
{
    public static IEnumerable<Type> GetInterfacesAndSelf(this Type type)
    {
        if (type == null)
            throw new ArgumentNullException();
        if (type.IsInterface)
            return new[] { type }.Concat(type.GetInterfaces());
        else
            return type.GetInterfaces();
    }

    public static KeyValuePair<Type, Type>? GetDictionaryKeyValueType(this Type type)
    {
        KeyValuePair<Type, Type>? types = null;
        foreach (var pair in type.GetDictionaryKeyValueTypes())
        {
            if (types == null)
                types = pair;
            else
                return null;
        }
        return types;
    }

    public static IEnumerable<KeyValuePair<Type, Type>> GetDictionaryKeyValueTypes(this Type type)
    {
        foreach (Type intType in type.GetInterfacesAndSelf())
        {
            if (intType.IsGenericType
                && intType.GetGenericTypeDefinition() == typeof(IDictionary<,>))
            {
                var args = intType.GetGenericArguments();
                if (args.Length == 2)
                    yield return new KeyValuePair<Type, Type>(args[0], args[1]);
            }
        }
    }
}
