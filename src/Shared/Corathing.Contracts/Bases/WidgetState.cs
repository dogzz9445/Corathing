using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corathing.Contracts.Bases;

public interface IWidgetCoreState
{
    string TypeName { get; }
    int RowIndex { get; }
    int ColumnIndex { get; }
    int RowSpan { get; }
    int ColumnSpan { get; }

    // 설정 가능한 변수
    string Title { get; }
    public string Tags { get; set; }
    bool VisibleTitle { get; }
    bool UseDefaultBackgroundColor { get; }
    string BackgroundColor { get; }
}

public class WidgetCoreState : IWidgetCoreState
{
    /// <summary>
    /// Just known type name of the widget in settings file.
    /// This is not for converting type.
    /// This is not affect anything.
    /// 세팅 파일에 설정되는 타입 이름
    /// </summary>
    public string TypeName { get; set; }

    public int RowIndex { get; set; }
    public int ColumnIndex { get; set; }
    public int RowSpan { get; set; }
    public int ColumnSpan { get; set; }

    /// <summary>
    /// Shows the name of the widget.
    /// 화면에 표시되는 위젯의 이름
    /// </summary>
    public string Title { get; set; }

    public string Tags { get; set; }

    /// <summary>
    /// Just known namespace name of the widget in settings file.
    /// This is not for converting namespace or logic of system.
    /// 기본적으로 제목을 보여줄지에 대한 설정
    /// </summary>
    public bool VisibleTitle { get; set; }
    public bool UseDefaultBackgroundColor { get; set; }
    public string BackgroundColor { get; set; }
}

public interface IWidgetState : IEntity
{
    PackageReferenceState PackageReference { get; }
    WidgetCoreState CoreSettings { get; }
    object? CustomSettings { get; }
}

public class WidgetState : IWidgetState
{
    public Guid Id { get; set; }
    public PackageReferenceState PackageReference { get; set; }
    public WidgetCoreState CoreSettings { get; set; }
    public object? CustomSettings { get; set; }
}
