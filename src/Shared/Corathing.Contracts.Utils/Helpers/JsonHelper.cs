using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Corathing.Contracts.Utils.Helpers;

public static class JsonHelper
{
    public static object? DeepCopy(object? obj, Type type)
    {
        var json = JsonSerializer.Serialize(obj, type);
        return JsonSerializer.Deserialize(json, type);
    }
}
