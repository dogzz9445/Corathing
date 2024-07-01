using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

using CommunityToolkit.Mvvm.ComponentModel;

using Corathing.Contracts.Services;

using Microsoft.Extensions.DependencyInjection;

namespace Corathing.Contracts.Bases;

public partial class WidgetContext : ObservableRecipient
{
    protected readonly IServiceProvider _services;

    #region 숨겨진 프로퍼티
    public Type WidgetType;
    public Guid WidgetId;

    public WidgetState? State { get; set; }
    public WidgetLayout? Layout { get; set; }
    #endregion

    #region 확정된 프로퍼티
    [ObservableProperty]
    private string _defaultTitle;
    [ObservableProperty]
    private string _widgetTitle;
    [ObservableProperty]
    private bool? _visibleTitle;
    [ObservableProperty]
    private string _widgetDescription;
    [ObservableProperty]
    private bool? _useDefaultBackgroundColor;
    [ObservableProperty]
    private string _backgroundColor;
    #endregion

    #region Only Used Properties in Context
    [ObservableProperty]
    private bool? _editMode;

    [ObservableProperty]
    private bool? _isSelecting;
    [ObservableProperty]
    private bool? _isDragging;
    [ObservableProperty]
    private bool? _isResizing;
    #endregion

    public WidgetContext()
    {
        IsSelecting = false;
        IsDragging = false;
        IsResizing = false;
    }

    public WidgetContext(IServiceProvider services, WidgetState state) : this()
    {
        _services = services;

        Update(state);
    }

    public virtual void Update(WidgetState state)
    {
        State = state;
        WidgetId = state.Id;
        WidgetTitle = state.CoreSettings.Title;
        VisibleTitle = state.CoreSettings.VisibleTitle;
        UseDefaultBackgroundColor = state.CoreSettings.UseDefaultBackgroundColor;
        BackgroundColor = state.CoreSettings.BackgroundColor;
    }

    public virtual void OnDestroy()
    {
    }

    // TODO:
    // 이벤트 처리

    /// <summary>
    /// Resize 시 발생되는 이벤트
    /// </summary>
    /// <param name="layout"></param>
    public virtual void OnResized(WidgetLayout? layout)
    {

    }
}
