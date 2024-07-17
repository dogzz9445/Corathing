using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Corathing.Contracts.Attributes;
using Corathing.Contracts.Bases;
using Corathing.Contracts.Services;

using Microsoft.Extensions.DependencyInjection;

namespace Corathing.Widgets.Basics.Widgets.WebQueries;

[EntryCoraWidget(
    contextType: typeof(WebQueryWidgetContext),
    name: "Create Web Query",
    description: "Provides a one by one square widget.",
    menuPath: "Default/Web Query",
    menuOrder: 0
    )]
public partial class WebQueryWidgetContext : WidgetContext
{
    public override void OnCreate(IServiceProvider services, WidgetState state)
    {
        ILocalizationService localizationService = _services.GetService<ILocalizationService>();
        localizationService.Provide("Corathing.Widgets.Basics.WebQueryName", value => WidgetTitle = value);
    }
}
