using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using Corathing.Contracts.DataContexts;
using Corathing.Contracts.Messages;
using Corathing.Contracts.Services;

using Microsoft.Extensions.DependencyInjection;

namespace Corathing.Widgets.Basics.DataSources.ExecutableApps;

public partial class ExecutableAppDataSourceSelector : DataSourceSelector<ExecutableAppDataSourceContext>
{
    public ExecutableAppDataSourceSelector(IServiceProvider services, Guid? guid = null) : base(services, guid)
    {
    }
}
