using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Corathing.Contracts.Utils.Helpers;

public static class JsonHelper
{
    public static T DeepCopy<T>(object obj) where T : class
    {
        var json = JsonSerializer.Serialize<T>((T)obj);
        return JsonSerializer.Deserialize<T>(json);
    }

    public static object DeepCopy(object obj, Type type)
    {
        var json = JsonSerializer.Serialize(obj, type);
        return JsonSerializer.Deserialize(json, type);
    }
}
