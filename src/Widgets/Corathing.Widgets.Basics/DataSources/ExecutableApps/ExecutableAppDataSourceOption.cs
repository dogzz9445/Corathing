using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corathing.Widgets.Basics.DataSources.ExecutableApps;

public class ExecutableAppDataSourceOption
{
    public string Title { get; set; }
    public List<string> ExecutableFiles { get; set; }
    public string CommandLineArguments { get; set; }
}
