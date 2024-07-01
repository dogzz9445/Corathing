using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Corathing.Contracts.Bases;
using Corathing.Contracts.Entries;

namespace Corathing.Contracts.Attributes;

/// <summary>
/// Cora 위젯에 대한 설정
/// 위젯 및 옵션에 대한 타입 설정
/// 제목, 너비, 높이에 대한 기본 값을 설정
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class EntryCoraWidgetAttribute : Attribute
{
    public Type ViewType { get; }
    public Type ContextType { get; }
    public Type? CustomSettingsType { get; }
    public Type? CustomSettingsContextType { get; }

    // Information
    public string? Name { get; }
    public string? Description { get; }
    public bool VisibleTitle { get; }
    public string? Title { get; }

    // MenuInfo
    public string? MenuPath { get; }
    public int MenuOrder { get; }
    public string? MenuTooltip { get; }

    // LayoutInfo
    public int MaximumColumnSpan { get; }
    public int MaximumRowSpan { get; }
    public int DefaultColumnSpan { get; }
    public int DefaultRowSpan { get; }
    public int MinimumColumnSpan { get; }
    public int MinimumRowSpan { get; }

    public EntryCoraWidgetAttribute(
        Type viewType,
        Type contextType,
        Type? customSettingsType = null,
        Type? customSettingsContextType = null,
        // Information
        string? name = null,
        string? description = null,
        bool visibleTitle = true,
        string? title = null,
        // MenuInfo
        string? menuPath = null,
        int menuOrder = 0,
        string menuTooltip = "",
        // LayoutInfo
        int maximumColumnSpan = 16,
        int maximumRowSpan = 16,
        int defaultColumnSpan = 2,
        int defaultRowSpan = 2,
        int minimumColumnSpan = 1,
        int minimumRowSpan = 1
        )
    {
        ViewType = viewType;
        ContextType = contextType;
        CustomSettingsType = customSettingsType;
        CustomSettingsContextType = customSettingsContextType;

        Name = name;
        Description = description;
        Title = title;
        VisibleTitle = visibleTitle;

        MenuPath = menuPath;
        MenuOrder = menuOrder;
        MenuTooltip = menuTooltip;

        MaximumColumnSpan = maximumColumnSpan;
        MaximumRowSpan = maximumRowSpan;
        DefaultColumnSpan = defaultColumnSpan;
        DefaultRowSpan = defaultRowSpan;
        MinimumColumnSpan = minimumColumnSpan;
        MinimumRowSpan = minimumRowSpan;
    }
}
