using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Corathing.Contracts.Services;

namespace Corathing.Contracts.Attributes;

/// <summary>
/// 사용자가 설정하기 전, 개발자가 지정한 기본 타이틀을 나타냅니다.
/// Default title that the developer specified before the user set it.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class EntryCoraWidgetDefaultTitleAttribute : Attribute
{
    public ApplicationLanguage Language { get; }
    public string DefaultTitle { get; }

    public EntryCoraWidgetDefaultTitleAttribute(ApplicationLanguage language, string defaultTitle)
    {
        Language = language;
        DefaultTitle = defaultTitle;
    }
}
