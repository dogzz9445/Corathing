﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Corathing.Contracts.Services;

namespace Corathing.Contracts.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class EntryCoraWidgetNameAttribute : Attribute
{
    public ApplicationLanguage Language { get; }
    public string Name { get; }

    public EntryCoraWidgetNameAttribute(ApplicationLanguage language, string name)
    {
        Language = language;
        Name = name;
    }
}
