using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Corathing.Contracts.Services;

namespace Corathing.Contracts.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class EntryCoraDataSourceNameAttribute : Attribute
{
    public ApplicationLanguage Language { get; }
    public string Name { get; }

    public EntryCoraDataSourceNameAttribute(ApplicationLanguage language, string name)
    {
        Language = language;
        Name = name;
    }
}
