using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Corathing.Contracts.Services;

namespace Corathing.Contracts.Entries;

public interface ICoraWidgetInfo
{
    // Types
    public Type WidgetViewType { get; }
    public Type WidgetContextType { get; }
    public Type WidgetCustomSettingsType { get; }
    public Type WidgetCustomSettingsContextType { get; }

    bool DefaultVisibleTitle { get; }

    // Information
    string Name { get; }
    string Description { get; }
    string DefaultTitle { get; }
    string MenuTooltip { get; }
    Dictionary<ApplicationLanguage, string> LocalizedNames { get; }
    Dictionary<ApplicationLanguage, string> LocalizedDescriptions { get; }
    Dictionary<ApplicationLanguage, string> LocalizedTitles { get; }
    Dictionary<ApplicationLanguage, string> LocalizedMenuTooltips { get; }
    List<string> Tags { get; }

    // Menu
    string MenuPath { get; }
    int MenuOrder { get; }

    // Layouts
    int MinimumColumnSpan { get; }
    int MinimumRowSpan { get; }
    int MaximumColumnSpan { get; }
    int MaximumRowSpan { get; }
    int DefaultColumnSpan { get; }
    int DefaultRowSpan { get; }
}

public class CoraWidgetInfo : ICoraWidgetInfo
{
    // Types
    public Type WidgetViewType { get; set; }
    public Type WidgetContextType { get; set; }
    public Type WidgetCustomSettingsType { get; set; }
    public Type WidgetCustomSettingsContextType { get; set; }

    public bool DefaultVisibleTitle { get; set; }

    // Information
    public string Name { get; set; }
    public string Description { get; set; }
    public string DefaultTitle { get; set; }
    public string MenuTooltip { get; set; }
    public Dictionary<ApplicationLanguage, string> LocalizedNames { get; set; }
    public Dictionary<ApplicationLanguage, string> LocalizedDescriptions { get; set; }
    public Dictionary<ApplicationLanguage, string> LocalizedTitles { get; set; }
    public Dictionary<ApplicationLanguage, string> LocalizedMenuTooltips { get; set; }
    public List<string> Tags { get; set; }

    // Menu
    public string MenuPath { get; set; }
    public int MenuOrder { get; set; }

    // Layouts
    public int MinimumColumnSpan { get; set; }
    public int MinimumRowSpan { get; set; }
    public int MaximumColumnSpan { get; set; }
    public int MaximumRowSpan { get; set; }
    public int DefaultColumnSpan { get; set; }
    public int DefaultRowSpan { get; set; }
}
