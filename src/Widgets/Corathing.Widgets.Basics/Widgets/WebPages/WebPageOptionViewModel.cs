using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;

using Corathing.Contracts.DataContexts;
using Corathing.Widgets.Basics.DataSources.ExecutableApps;
using Corathing.Widgets.Basics.DataSources.WebSessions;
using Corathing.Widgets.Basics.Widgets.FileOpeners;

namespace Corathing.Widgets.Basics.Widgets.WebPages;

public partial class WebPageOptionViewModel : CustomSettingsContext
{
    [ObservableProperty]
    private int _autoReloadInterval;

    [ObservableProperty]
    private WebSessionDataSourceSelector _webSessionDataSourceSelector;

    protected override void OnCreate(object? defaultOption)
    {
        if (defaultOption is not WebPageOption webPageOption)
        {
            throw new ArgumentException($"Not a valid type for CustomSettings {nameof(WebPageOption)}");
        }
        CustomSettings = webPageOption;
        WebSessionDataSourceSelector = new WebSessionDataSourceSelector(Services);
    }

    protected override void OnContextChanged()
    {
    }

    protected override void OnSettingsChanged(object? option)
    {
    }
}
