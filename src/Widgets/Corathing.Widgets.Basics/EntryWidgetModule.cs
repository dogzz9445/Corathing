using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Corathing.Contracts.Entries;
using Corathing.Widgets.Basics.Resources;

namespace Corathing.Widgets.Basics;

[EntryCoraPackage(typeof(EntryWidgetModule))]
public class EntryWidgetModule : CoraWidgetModuleBase
{
    public EntryWidgetModule()
    {
        StringResources.Add(BasicWidgetStringResources.ResourceManager);
    }
}
