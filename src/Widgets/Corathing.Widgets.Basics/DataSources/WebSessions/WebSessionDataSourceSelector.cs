using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Corathing.Contracts.DataContexts;

namespace Corathing.Widgets.Basics.DataSources.WebSessions;

public partial class WebSessionDataSourceSelector : DataSourceSelector<WebSessionDataSourceContext>
{
    public WebSessionDataSourceSelector(IServiceProvider services, Guid? guid = null) : base(services, guid)
    {
    }
}
