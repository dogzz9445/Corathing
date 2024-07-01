using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Corathing.Contracts.Entries;

using Microsoft.Extensions.DependencyInjection;

namespace Corathing.Contracts.Utils.Generators;

public class CoraPackageGenerator
{
    public ICoraPackageInfo PackageInfo { get; private set; }
    public List<CoraDataSourceGenerator> DataSources { get; private set; }
    public List<CoraWidgetGenerator> Widgets { get; private set; }

    public CoraPackageGenerator(IServiceProvider services)
    {
        PackageInfo = services.GetService<ICoraPackageInfo>();
        DataSources = new List<CoraDataSourceGenerator>();
        Widgets = new List<CoraWidgetGenerator>();
    }
}
