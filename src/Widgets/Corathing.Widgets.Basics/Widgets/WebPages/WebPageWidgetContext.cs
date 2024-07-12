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

using static System.Windows.Forms.AxHost;

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

    private string _sourceUrl;

    [ObservableProperty]
    private WebSessionDataSourceContext _webSessionDataSource;

    private CancellationTokenSource _reloadRoutineTokenSource;

    public override void OnCreate(IServiceProvider services, WidgetState state)
    {
        _reloadRoutineTokenSource = new CancellationTokenSource();

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
        var themeService = _services.GetRequiredService<IThemeService>();
        SetTheme(themeService.GetAppTheme());
        SetAutoReloadInterval(option.AutoReloadInterval);
        SetUrl(option.Url ?? WebPageOption.DefaultUrl);

        if (option.WebSessionDataSourceId != null && option.WebSessionDataSourceId != Guid.Empty)
        {
            var dataSourceService = _services.GetRequiredService<IDataSourceService>();
            var dataSource = dataSourceService.GetDataSourceContext<WebSessionDataSourceContext>(option.WebSessionDataSourceId);
            if (WebSessionDataSource == null || dataSource.DataSourceId != WebSessionDataSource.DataSourceId)
            {
                WebSessionDataSource = dataSource;
                if (WebView != null)
                {
                    //WebView.Loaded -= WebView_Loaded;
                    WebView.Dispose();
                    WebView = null;
                }
                WebView = null;
                if (WebSessionDataSource != null)
                {
                    WebView = WebSessionDataSource.CreateWebView();
                    await WebView.EnsureCoreWebView2Async();
                    themeService.ProvideApplicationTheme(theme => SetTheme(theme));
                    WebView.Source = new Uri(option.Url ?? WebPageOption.DefaultUrl);
                    //WebView.Loaded += WebView_Loaded;
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

    private void SetAutoReloadInterval(int autoReloadInterval)
    {
        if (State.CustomSettings is not WebPageOption option)
            return;

        _reloadRoutineTokenSource.Cancel();

        if (autoReloadInterval <= 0)
            return;

        _reloadRoutineTokenSource = new CancellationTokenSource();
        _ = ReloadRoutine(_reloadRoutineTokenSource.Token);
    }

    public override void OnDestroy()
    {
        _reloadRoutineTokenSource.Cancel();
    }

    private void WebView_Loaded(object? sender, RoutedEventArgs e)
    {
        SetWebView();
    }

    private async Task ReloadRoutine(CancellationToken token)
    {
        if (State == null)
            return;
        if (State.CustomSettings is not WebPageOption option)
            return;

        while (!token.IsCancellationRequested)
        {
            await Task.Delay(option.AutoReloadInterval * 1000, token);
            WebView?.Reload();
        }
    }

    private void SetUrl(string url)
    {
        if (_sourceUrl == url)
            return;
        _sourceUrl = url;
        if (WebView == null)
            return;
        WebView.Source = new Uri(url);
    }

    private void SetTheme(ApplicationTheme theme)
    {
        if (WebView == null || State == null)
            return;
        if (State.CustomSettings is not WebPageOption option)
            return;

        WebView.CoreWebView2.Profile.PreferredColorScheme = option.IndependentTheme switch
        {
            WebPageTheme.App => theme switch
            {
                ApplicationTheme.Unknown => CoreWebView2PreferredColorScheme.Auto,
                ApplicationTheme.Light => CoreWebView2PreferredColorScheme.Light,
                ApplicationTheme.Dark => CoreWebView2PreferredColorScheme.Dark,
                _ => CoreWebView2PreferredColorScheme.Auto
            },
            WebPageTheme.Light => CoreWebView2PreferredColorScheme.Light,
            WebPageTheme.Dark => CoreWebView2PreferredColorScheme.Dark,
            _ => CoreWebView2PreferredColorScheme.Auto
        };
    }

    private void SetWebView()
    {
        if (WebView == null || State == null)
            return;
        if (State.CustomSettings is not WebPageOption option)
            return;

        WebView.Source = new Uri(option.Url ?? WebPageOption.DefaultUrl);
    }

    //private void WebView_CoreWebView2InitializationCompleted(object? sender, CoreWebView2InitializationCompletedEventArgs e)
    //{
    //    IApplicationService applicationService = _services.GetService<IApplicationService>();
    //    WebView.Source = new Uri("https://www.microsoft.com");
    //}

    //private void OnWebViewNavigationCompleted(object? sender, CoreWebView2NavigationCompletedEventArgs e)
    //{
    //    IApplicationService applicationService = _services.GetService<IApplicationService>();
    //    applicationService.InvokeAsync(async () =>
    //    {
    //        if (WebView != null)
    //        {
    //            WebView.SetCurrentValue(WebView2.SourceProperty, new Uri(Options.Url));
    //        }
    //    });
    //}
}
