using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corathing.Dashboards.WPF.Bindings;

public enum LayoutChangedType
{
    None,
    New,
    Remove,
    Drag,
    Resize,

}

public class LayoutChangedEventArgs : EventArgs
{
    public LayoutChangedType ChangedType;
}
