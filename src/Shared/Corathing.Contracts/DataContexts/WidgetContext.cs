using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

using CommunityToolkit.Mvvm.ComponentModel;

using Corathing.Contracts.Services;

using Microsoft.Extensions.DependencyInjection;

namespace Corathing.Contracts.Bases;

public partial class WidgetContext : ObservableRecipient
{
    protected IServiceProvider? _services;
    protected IAppStateService? _appStateService;

    #region 숨겨진 프로퍼티
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
    private bool? _isTemporal;
    #endregion

    public void Initialize(IServiceProvider services, WidgetState state)
    {
        _services = services;
        _appStateService = _services.GetRequiredService<IAppStateService>();
        IsTemporal = false;

        State = state;
        WidgetId = state.Id;
        WidgetTitle = state.CoreSettings.Title;
        VisibleTitle = state.CoreSettings.VisibleTitle;
        UseDefaultBackgroundColor = state.CoreSettings.UseDefaultBackgroundColor;
        BackgroundColor = state.CoreSettings.BackgroundColor;

        Layout = new WidgetLayout()
        {
            Id = Guid.NewGuid(),
            WidgetStateId = WidgetId,
            Rect = new WidgetLayoutRect()
            {
                X = state.CoreSettings.ColumnIndex,
                Y = state.CoreSettings.RowIndex,
                W = state.CoreSettings.ColumnSpan,
                H = state.CoreSettings.RowSpan,
            }
        };

        OnCreate(services, state);
        ApplyState(state);
    }

    public void ApplyState(WidgetState state)
    {
        State = state;
        WidgetTitle = state.CoreSettings.Title;
        VisibleTitle = state.CoreSettings.VisibleTitle;
        UseDefaultBackgroundColor = state.CoreSettings.UseDefaultBackgroundColor;
        BackgroundColor = state.CoreSettings.BackgroundColor;

        OnStateChanged(State);
    }

    public void SaveState()
    {
        if (State == null)
            return;

        _services?.GetRequiredService<IAppStateService>()
            .UpdateWidget(State);
    }

    public void Destroy()
    {
        OnDestroy();

        _appStateService?.RemoveWidget(WidgetId);
    }

    public virtual void OnCreate(IServiceProvider services, WidgetState state)
    {
    }

    public virtual void OnStateChanged(WidgetState state)
    {
    }

    public virtual void OnMessage()
    {
    }
    public virtual void OnDestroy()
    {
    }

    /// <summary>
    /// Resize 시 발생되는 이벤트
    /// </summary>
    /// <param name="layout"></param>
    public virtual void OnResized(WidgetLayout? layout)
    {
    }
}
