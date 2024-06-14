using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace Corathing.Contracts.Entries;

public class CoraWidgetModuleBase
{
    public List<ResourceManager> StringResources { get; set; } = new List<ResourceManager>();
}
