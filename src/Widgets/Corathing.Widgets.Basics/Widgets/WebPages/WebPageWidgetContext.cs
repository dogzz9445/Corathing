using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using Corathing.Contracts.Attributes;
using Corathing.Contracts.Bases;
using Corathing.Contracts.Services;
using Corathing.Widgets.Basics.DataSources.ExecutableApps;
using Corathing.Widgets.Basics.DataSources.WebSessions;
using Corathing.Widgets.Basics.Widgets.FileOpeners;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Web.WebView2.Core;
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
            if (WebSessionDataSource == null || dataSource.DataSourceId != WebSessionDataSource.DataSourceId)
            {
                WebSessionDataSource = dataSource;
                WebView = null;
                if (WebSessionDataSource != null)
                {
                    WebView = WebSessionDataSource.CreateWebView();
                    await WebView.EnsureCoreWebView2Async();
                    WebView.CoreWebView2.Profile.PreferredColorScheme = CoreWebView2PreferredColorScheme.Dark;
                    WebView.Source = new Uri("https://www.microsoft.com");
                    //WebView.Loaded += (s, e) =>
                    //{
                    //    SetWebView(WebView);
                    //};
                    //WebView.CoreWebView2InitializationCompleted += WebView_CoreWebView2InitializationCompleted;
                    //WebView.Loaded += (s, e) =>
                    //{
                    //    WebView.NavigationCompleted += OnWebViewNavigationCompleted;
                    //    WebView.SetCurrentValue(FrameworkElement.UseLayoutRoundingProperty, true);
                    //    WebView.SetCurrentValue(WebView2.DefaultBackgroundColorProperty, System.Drawing.Color.Transparent);

                    //};
                }
            }
        }
    }

    private void SetWebView(WebView2 webView)
    {
        WebView.Source = new Uri("https://www.microsoft.com");
    }

    private void WebView_CoreWebView2InitializationCompleted(object? sender, CoreWebView2InitializationCompletedEventArgs e)
    {
        IApplicationService applicationService = _services.GetService<IApplicationService>();
        WebView.Source = new Uri("https://www.microsoft.com");
    }

    private void OnWebViewNavigationCompleted(object? sender, CoreWebView2NavigationCompletedEventArgs e)
    {
        IApplicationService applicationService = _services.GetService<IApplicationService>();
        applicationService.InvokeAsync(async () =>
        {
            if (WebView != null)
            {
                WebView.SetCurrentValue(WebView2.SourceProperty, new Uri(Options.Url));
            }
        });
    }
}
