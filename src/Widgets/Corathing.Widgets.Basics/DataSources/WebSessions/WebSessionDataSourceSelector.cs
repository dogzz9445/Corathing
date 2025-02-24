using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Corathing.Contracts.DataContexts;
using Corathing.Contracts.Services;

using Microsoft.Extensions.DependencyInjection;

namespace Corathing.Widgets.Basics.DataSources.WebSessions;

public partial class WebSessionDataSourceSelector : DataSourceSelector<WebSessionDataSourceContext>
{
    public WebSessionDataSourceSelector(IServiceProvider services, Guid? guid = null) :
        base(
        services: services,
        guid: guid,
        selectDefaultCreateIfEmpty: true
        )
    {
    }
}
