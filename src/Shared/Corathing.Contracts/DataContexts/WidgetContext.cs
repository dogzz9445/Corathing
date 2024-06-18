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

    #region 확정된 프로퍼티
    [ObservableProperty]
    private string _widgetTitle;
    [ObservableProperty]
    private bool? _visibleTitle;
    [ObservableProperty]
    private bool? _editMode;
    #endregion

    [ObservableProperty]
    private bool? _isSelecting;
    [ObservableProperty]
    private bool? _isDragging;
    [ObservableProperty]
    private bool? _isResizing;
    [ObservableProperty]
    private bool? _isEditing;
    [ObservableProperty]
    private WidgetLayout? _layout;

    public WidgetContext()
    {
        WidgetTitle = "Widget";
        VisibleTitle = true;
        EditMode = true;

        IsSelecting = false;
        IsDragging = false;
        IsResizing = false;
        IsEditing = false;
    }

    public WidgetContext(IServiceProvider services) : this()
    {
        _services = services;
    }

    public virtual void OnDestroy()
    {
    }
}
