using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;

using Corathing.Contracts.Bases;
using Corathing.Contracts.Messages;
using Corathing.Contracts.Services;

using Microsoft.Extensions.DependencyInjection;

namespace Corathing.Contracts.DataContexts;

public partial class DataSourceContext : ObservableRecipient
{
    protected IServiceProvider _services;

    #region 숨겨진 프로퍼티
    public Guid DataSourceId;

    public DataSourceState? State { get; set; }
    #endregion

    [ObservableProperty]
    private string _name;

    public void Initialize(IServiceProvider services, DataSourceState state)
    {
        _services = services;

        State = state;
        DataSourceId = state.Id;

        OnCreate(services, state);
        ApplyState(state);
    }

    public void ApplyState(DataSourceState state)
    {
        // FIXME:
        // State를 Copy 하도록 변경
        State = state;
        Name = state.CoreSettings.Title;

        OnStateChanged(state);
    }

    public void SaveState()
    {
        if (State == null)
            return;
        _services?.GetRequiredService<IAppStateService>().UpdateDataSource(State);
    }

    public void Destroy()
    {
        OnDestroy();

        _services?.GetRequiredService<IStorageService>().DeleteEntityFolder(State);
        _services?.GetRequiredService<IAppStateService>().RemoveDataSource(State);
    }

    public virtual void OnCreate(IServiceProvider services, DataSourceState state)
    {
    }

    public virtual void OnStart()
    {
    }

    public virtual void OnStateChanged(DataSourceState state)
    {
    }

    private void OnCustomSettingsChanged(object recipient, CustomSettingsChangedMessage message)
    {

    }

    public virtual void OnMessage()
    {
    }

    public virtual void OnDestroy()
    {
    }
}
