using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Corathing.Contracts.DataContexts;

namespace Corathing.Widgets.Basics.DataSources.ToDos;

public class ToDoDataSourceSelector : DataSourceSelector<ToDoDataSourceContext>
{
    public ToDoDataSourceSelector(IServiceProvider services, Guid? guid = null) :
        base(
        services: services,
        guid: guid
        )
    {
    }
}
