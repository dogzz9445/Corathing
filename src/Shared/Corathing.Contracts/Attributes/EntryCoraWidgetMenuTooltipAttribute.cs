using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Corathing.Contracts.Services;

namespace Corathing.Contracts.Attributes;

/// <summary>
/// 메뉴에 나타날 때의 툴팁을 정의합니다.
/// Menu tooltip definition.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class EntryCoraWidgetMenuTooltipAttribute : Attribute
{
    public ApplicationLanguage Language { get; }
    public string MenuTooltip { get; }

    public EntryCoraWidgetMenuTooltipAttribute(ApplicationLanguage language, string menuTooltip)
    {
        Language = language;
        MenuTooltip = menuTooltip;
    }
}
