using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

using CommunityToolkit.Mvvm.ComponentModel;

namespace Corathing.Contracts.Bases;

public partial class WidgetContext : ObservableRecipient
{
    protected readonly IServiceProvider _services;

    #region 숨겨진 프로퍼티
    [ObservableProperty]
    private Type _widgetType;
    [ObservableProperty]
    private Guid _widgetId;
    #endregion

    #region 확정된 프로퍼티 건드리지 말기
    [ObservableProperty]
    private WidgetState? _state;
    [ObservableProperty]
    private WidgetLayout? _layout;
    #endregion

    #region 확정된 프로퍼티
    [ObservableProperty]
    private string _widgetTitle;
    [ObservableProperty]
    private bool? _visibleTitle;
    [ObservableProperty]
    private bool? _editMode;
    [ObservableProperty]
    private int? _minColumns;
    [ObservableProperty]
    private int? _minRows;
    #endregion

    [ObservableProperty]
    private bool? _isSelecting;
    [ObservableProperty]
    private bool? _isDragging;
    [ObservableProperty]
    private bool? _isResizing;
    [ObservableProperty]
    private bool? _isEditing;

    public WidgetContext()
    {
        EditMode = true;

        MinColumns = 2;
        MinRows = 2;

        IsSelecting = false;
        IsDragging = false;
        IsResizing = false;
        IsEditing = false;
    }

    public WidgetContext(IServiceProvider services, WidgetState state) : this()
    {
        _services = services;

        WidgetId = state.Id;
        State = state;
        WidgetTitle = state.CoreSettings.Title;
        VisibleTitle = state.CoreSettings.VisibleTitle;
    }

    public virtual void OnDestroy()
    {
    }
}
