using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;

namespace Corathing.Widgets.Basics.Widgets.WebPages;

public enum WebPageViewMode
{
    Mobile,
    Desktop,
}

public enum SessionScope
{
    Application,
    Project,
    Workflow,
    Widget,
}

public enum SessionPersistence
{
    Persistent,

}

public class WebPageOption
{
    public Guid? WebSessionDataSourceId { get; set; }
}
