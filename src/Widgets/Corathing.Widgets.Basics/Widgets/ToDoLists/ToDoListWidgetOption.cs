using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Corathing.Contracts.Attributes;
using Corathing.Widgets.Basics.DataSources.ExecutableApps;
using Corathing.Widgets.Basics.Widgets.Openers;

namespace Corathing.Widgets.Basics.Widgets.ToDoLists;

public enum ToDoVisualizationType
{
    List,
    Markdown,
}

public class ToDoListWidgetOption
{
    public bool IsShowDone { get; set; }
    public bool IsShowTask { get; set; }

    public Guid? ToDoDataSourceId { get; set; }
}
