using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corathing.Contracts.Entries;

public interface ICoraWidgetInfo
{
    // Information
    string Name { get; }
    string Description { get; }
    string Title { get; }
    bool VisibleTitle { get; }

    // Menu
    string MenuPath { get; }
    int MenuOrder { get; }
    string MenuTooltip { get; }

    // Layouts
    int MinimunColumnSpan { get; }
    int MinimumRowSpan { get; }
    int MaximumColumnSpan { get; }
    int MaximumRowSpan { get; }
    int DefaultColumnSpan { get; }
    int DefaultRowSpan { get; }
}

public class CoraWidgetInfo : ICoraWidgetInfo
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string Title { get; set; }
    public bool VisibleTitle { get; set; }
    public string MenuPath { get; set; }
    public int MenuOrder { get; set; }
    public string MenuTooltip { get; set; }
    public int MinimunColumnSpan { get; set; }
    public int MinimumRowSpan { get; set; }
    public int MaximumColumnSpan { get; set; }
    public int MaximumRowSpan { get; set; }
    public int DefaultColumnSpan { get; set; }
    public int DefaultRowSpan { get; set; }
}
