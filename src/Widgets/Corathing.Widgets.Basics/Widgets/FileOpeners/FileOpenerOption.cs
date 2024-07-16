using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;

using Corathing.Contracts.Attributes;
using Corathing.Widgets.Basics.DataSources.ExecutableApps;

namespace Corathing.Widgets.Basics.Widgets.FileOpeners;

public enum FileOpenType
{
    Files,
    Folders,
}

public enum IconSourceType
{
    System,
    Svg,
    Image,
}

public class FileOpenerOption
{
    public FileOpenType OpenType { get; set; }
    public List<string> Files { get; set; }
    public List<string> Folders { get; set; }

    [ReferenceCoraDataSourceProperty(typeof(ExecutableAppDataSourceContext))]
    public Guid? ExecutableAppDataSourceId { get; set; }
}
