using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Corathing.Contracts.Bases;

namespace Corathing.Contracts.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class WidgetContextEntryAttribute : Attribute
{
    public IServiceProvider Services { get; set; }

    public WidgetGenerator WidgetGenerator { get; private set; }

    public WidgetContextEntryAttribute(string name,
        string description,
        string menuPath,
        int menuOrder,
        Type targetType,
        Type optionType = null)
    {
        WidgetGenerator = new WidgetGenerator(
            name: name,
            description: description,
            menuPath: menuPath,
            menuOrder: menuOrder,
            targetType: targetType
        );
    }

    public void RegisterServices(IServiceProvider services)
    {
        Services = services;
        WidgetGenerator.CreateWidgetInternal = () =>
            (WidgetContext)Activator.CreateInstance(WidgetGenerator.TargetType, Services);
    }
}
