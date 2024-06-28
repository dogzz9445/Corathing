using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Corathing.Contracts.Services;

namespace Corathing.Contracts.Attributes;

[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public class AssemblyCoraPackageNameAttribute : Attribute
{
    public ApplicationLanguage Language { get; }
    public string Name { get; }

    public AssemblyCoraPackageNameAttribute(ApplicationLanguage language, string name)
    {
        Language = language;
        Name = name;
    }
}
