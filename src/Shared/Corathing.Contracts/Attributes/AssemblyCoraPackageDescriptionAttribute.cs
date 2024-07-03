using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Corathing.Contracts.Services;

namespace Corathing.Contracts.Attributes;

[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public class AssemblyCoraPackageDescriptionAttribute : Attribute
{
    public ApplicationLanguage Language { get; }
    public string Description { get; }

    public AssemblyCoraPackageDescriptionAttribute(ApplicationLanguage language, string description)
    {
        Language = language;
        Description = description;
    }
}
