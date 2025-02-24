using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Corathing.Contracts.Attributes;
using Corathing.Widgets.Basics.DataSources.ExecutableApps;

namespace Corathing.Widgets.Basics.Widgets.Openers;

public enum OpenerType
{
    Files,
    Folders,
    Links,
}

public enum IconSourceType
{
    System,
    Svg,
    Image,
}

public class OpenerOption
{
    public OpenerType OpenerType { get; set; }
    public List<string>? Files { get; set; }
    public List<string>? Folders { get; set; }
    public List<string>? Links { get; set; }

    [ReferenceCoraDataSourceProperty(typeof(ExecutableAppDataSourceContext))]
    public Guid? ExecutableAppDataSourceId { get; set; }
}
