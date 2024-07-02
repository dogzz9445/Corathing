using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;

using Corathing.Contracts.Attributes;
using Corathing.Contracts.Bases;
using Corathing.Contracts.DataContexts;

namespace Corathing.Widgets.Basics.DataSources.ExecutableApps;

[EntryCoraDataSource(typeof(ExecutableAppDataSourceContext))]
public class ExecutableAppDataSourceContext : DataSourceContext, IDataSourceContext
{
    public string Title { get; set; }

    public override void OnCreate(IServiceProvider services, DataSourceState state)
    {
        base.OnCreate(services, state);

    }

    public void Update(DataSourceState state)
    {
    }

    public void OnDestroy(DataSourceState state)
    {
    }
}
