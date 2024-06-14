using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corathing.Contracts.Entries;

public class CoraWidgetInfo
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string MenuPath { get; set; }
    public int MenuOrder { get; set; }
    public string MenuTooltip { get; set; }
    public int MinimunColumnSpan { get; set; }
    public int MinimumRowSpan { get; set; }
}
