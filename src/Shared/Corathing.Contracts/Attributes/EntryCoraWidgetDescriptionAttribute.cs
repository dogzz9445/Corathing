using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Corathing.Contracts.Services;

namespace Corathing.Contracts.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class EntryCoraWidgetDescriptionAttribute : Attribute
{
    public ApplicationLanguage Language { get; }
    public string Description { get; }

    public EntryCoraWidgetDescriptionAttribute(ApplicationLanguage language, string description)
    {
        Language = language;
        Description = description;
    }
}
