using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Corathing.Contracts.Utils.Helpers;

public static class NameHelper
{
    public static string GenerateUniqueName(string? baseName = null, IEnumerable<string> usedNames = null)
    {
        for (int i = 1; i <= 1000; i++)
        {
            string res = $"{baseName} {i}";
            if (usedNames != null && !usedNames.Contains(res))
            {
                return res;
            }
        }
        return $"{baseName} 1000";
    }
    public static string GenerateCopyName(string itemName, List<string> usedNames)
    {
        return GenerateUniqueName($"{itemName} Copy", usedNames);
    }
}
