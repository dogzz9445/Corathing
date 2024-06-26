using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Corathing.Contracts.Bases;

namespace Corathing.Contracts.Entries;

/// <summary>
/// Cora 위젯에 대한 설정
/// 위젯 및 옵션에 대한 타입 설정
/// 제목, 너비, 높이에 대한 기본 값을 설정
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class EntryCoraWidgetAttribute : Attribute
{
    public IServiceProvider Services { get; set; }

    public CoraWidgetGenerator Generator { get; private set; }

    public EntryCoraWidgetAttribute(
        Type viewType,
        Type contextType,
        Type? optionType = null,
        // Information
        string? name = null,
        string? description = null,
        bool visibleTitle = true,
        string? title = null,
        // MenuInfo
        string? menuPath = null,
        int menuOrder = 0,
        // LayoutInfo
        int maximumColumnSpan = 10,
        int maximumRowSpan = 10,
        int defaultColumnSpan = 2,
        int defaultRowSpan = 2,
        int minimumColumnSpan = 1,
        int minimumRowSpan = 1
        )
    {

        Generator = new CoraWidgetGenerator(
            viewType: viewType,
            contextType: contextType,
            optionType: optionType
            )
        {
            Info = new CoraWidgetInfo()
            {
                Name = name,
                Description = description,
                Title = title,
                VisibleTitle = visibleTitle,

                MenuPath = menuPath,
                MenuOrder = menuOrder,
                MenuTooltip = description,

                MaximumColumnSpan = maximumColumnSpan,
                MaximumRowSpan = maximumRowSpan,
                DefaultColumnSpan = defaultColumnSpan,
                DefaultRowSpan = defaultRowSpan,
                MinimunColumnSpan = minimumColumnSpan,
                MinimumRowSpan = minimumRowSpan,
            }
        };
    }

    public void Configure(IServiceProvider services)
    {
        Generator.Services = services;
    }
}
