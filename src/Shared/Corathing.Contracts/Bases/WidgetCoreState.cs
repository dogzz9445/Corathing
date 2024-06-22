using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Corathing.Contracts.Bases.Interfaces;

namespace Corathing.Contracts.Bases;

public interface IWidgetCoreState
{
    string Name { get; }
    string TypeName { get; }
    string NamespaceName { get; }
    string Title { get; }
    bool VisibleTitle { get; }
}

public class WidgetCoreState : IWidgetCoreState
{
    /// <summary>
    /// Shows the name of the widget.
    /// 화면에 표시되는 위젯의 이름
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Just known type name of the widget in settings file.
    /// This is not for converting type.
    /// This is not affect anything.
    /// 세팅 파일에 설정되는 타입 이름
    /// </summary>
    public string TypeName { get; set; }

    /// <summary>
    /// Just known namespace name of the widget in settings file.
    /// This is not for converting namespace or logic of system.
    /// 세팅 파일에 설정되는 네임스페이스 이름
    /// </summary>
    public string NamespaceName { get; set; }

    /// <summary>
    /// Just known namespace name of the widget in settings file.
    /// This is not for converting namespace or logic of system.
    /// 제목에 대한 설정이 필요하면 사용
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Just known namespace name of the widget in settings file.
    /// This is not for converting namespace or logic of system.
    /// 기본적으로 제목을 보여줄지에 대한 설정
    /// </summary>
    public bool VisibleTitle { get; set; }
}
