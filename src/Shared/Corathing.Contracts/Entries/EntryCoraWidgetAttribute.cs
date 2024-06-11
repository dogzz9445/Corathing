using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Corathing.Contracts.Bases;

namespace Corathing.Contracts.Entries;

[AttributeUsage(AttributeTargets.Class)]
public class EntryCoraWidgetAttribute : Attribute
{
    public IServiceProvider Services { get; set; }

    public CoraWidgetGenerator Generator { get; private set; }

    public EntryCoraWidgetAttribute(
        Type viewType,
        Type contextType,
        string? dataTemplateSource = null,
        Type? optionType = null,
        // Specific
        string? name = null,
        string? description = null,
        // MenuInfo
        string? menuPath = null,
        int menuOrder = 0
        )
    {
        var menuInfo = new CoraWidgetMenuInfo()
        {
            MenuPath = menuPath,
            MenuOrder = menuOrder,
            MenuTooltip = description,
        };

        Generator = new CoraWidgetGenerator(
            viewType: viewType,
            contextType: contextType,
            dataTemplateSource: dataTemplateSource,
            optionType: optionType
            )
        {
            MenuInfo = menuInfo,
        };

    }

    public void Configure(IServiceProvider services)
    {
        Generator.Services = services;
    }
}
