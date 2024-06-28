using System;

using Corathing.Contracts.Services;

namespace Corathing.Contracts.Attributes;

[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public class EntryCoraDataSourceDefaultTitleAttribute : Attribute
{
    public ApplicationLanguage Language { get; }
    public string DefaultTitle { get; }

    public EntryCoraDataSourceDefaultTitleAttribute(ApplicationLanguage language, string defaultTitle)
    {
        Language = language;
        DefaultTitle = defaultTitle;
    }
}
