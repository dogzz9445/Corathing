using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;

using Corathing.Contracts.Bases;

namespace Corathing.Contracts.DataContexts;

public partial class DataSourceContext : ObservableRecipient
{
    protected IServiceProvider _services;

    #region 숨겨진 프로퍼티
    public Type DataSourceType;
    public Guid DataSourceId;

    public DataSourceState? State { get; set; }
    #endregion

    public DataSourceContext()
    {
    }

    public void ApplyState(DataSourceState state)
    {
        State = state;
        DataSourceId = state.Id;

        OnStateChanged(state);
    }

    public virtual void OnCreate(IServiceProvider services, DataSourceState state)
    {
        _services = services;
        ApplyState(state);
    }

    public virtual void OnStateChanged(DataSourceState state)
    {

    }

    public virtual void OnMessage()
    {

    }

    public virtual void OnDestroy()
    {

    }
}
