using System;

using Corathing.Contracts.Services;

namespace Corathing.Contracts.Attributes;

/// <summary>
/// 메뉴에 정의되는 이름
/// Definition of the name in the menu.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class EntryCoraWidgetMenuPathAttribute : Attribute
{
    public ApplicationLanguage Language { get; }
    public string MenuPath { get; }

    public EntryCoraWidgetMenuPathAttribute(ApplicationLanguage language, string menuPath)
    {
        Language = language;
        MenuPath = menuPath;
    }
}
