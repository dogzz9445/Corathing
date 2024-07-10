using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using Corathing.Contracts.Attributes;
using Corathing.Contracts.Bases;
using Corathing.Contracts.Services;
using Corathing.Widgets.Basics.DataSources.ExecutableApps;
using Corathing.Widgets.Basics.DataSources.WebSessions;
using Corathing.Widgets.Basics.Widgets.FileOpeners;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Web.WebView2.Wpf;

namespace Corathing.Widgets.Basics.Widgets.WebPages;

[EntryCoraWidget(
    contextType: typeof(WebPageWidgetContext),
    customSettingsType: typeof(WebPageOption),
    customSettingsContextType: typeof(WebPageOptionViewModel),
    name: "Create Web Page",
    description: "Provides a one by one square widget.",
    menuPath: "Default/Web Page",
    menuOrder: 0
    )]
public partial class WebPageWidgetContext : WidgetContext
{
    [ObservableProperty]
    private WebPageOption? _options;

    [ObservableProperty]
    private WebView2 _webView;

    [ObservableProperty]
    private WebSessionDataSourceContext _webSessionDataSource;

    public override void OnCreate(IServiceProvider services, WidgetState state)
    {
        ILocalizationService localizationService = _services.GetService<ILocalizationService>();
        localizationService.Provide("Corathing.Widgets.Basics.WebPageName", value => WidgetTitle = value);

        var appStateService = _services.GetService<IAppStateService>();
        //if (!widgetSystemService.TryGetWidgetOption<WebPageOption>(Uid, out var option))
        //WidgetTitle = option.Name;
    }

    public async override void OnStateChanged(WidgetState state)
    {
        if (state.CustomSettings is not WebPageOption option)
        {
            return;
        }

        if (option.WebSessionDataSourceId != null && option.WebSessionDataSourceId != Guid.Empty)
        {
            var dataSourceService = _services.GetRequiredService<IDataSourceService>();
            var dataSource = dataSourceService.GetDataSourceContext<WebSessionDataSourceContext>(option.WebSessionDataSourceId);
            if (dataSource.DataSourceId != WebSessionDataSource.DataSourceId)
            {
                WebSessionDataSource = dataSource;
                WebView = null;
                if (WebSessionDataSource != null)
                {
                    WebView = await WebSessionDataSource.CreateWebView();
                    WebView.Source = new Uri(option.Url);
                }
            }
        }
    }
}
