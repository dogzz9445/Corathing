using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;

using Corathing.Contracts.Attributes;
using Corathing.Contracts.Bases;
using Corathing.Contracts.DataContexts;

namespace Corathing.Widgets.Basics.DataSources.WebSessions;

[EntryCoraDataSource(typeof(WebSessionDataSourceContext))]
public class WebSessionDataSourceContext : DataSourceContext, IDataSourceContext
{
    public virtual void OnCreate(IServiceProvider services, DataSourceState state)
    {
        base.OnCreate(services, state);

    }

    public void OnDestroy(DataSourceState state)
    {
    }

    public void Apply(DataSourceState state)
    {
        throw new NotImplementedException();
    }
}
