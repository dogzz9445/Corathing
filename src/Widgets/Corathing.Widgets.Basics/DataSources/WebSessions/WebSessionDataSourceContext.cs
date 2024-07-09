using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;

using Corathing.Contracts.Attributes;
using Corathing.Contracts.Bases;
using Corathing.Contracts.DataContexts;
using Corathing.Contracts.Services;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;

namespace Corathing.Widgets.Basics.DataSources.WebSessions;

[EntryCoraDataSource(typeof(WebSessionDataSourceContext))]
public class WebSessionDataSourceContext : DataSourceContext
{
    private WebView2 _webview;
    private CoreWebView2Environment _environment;
    private CoreWebView2CookieManager _cookieManager;
    private CoreWebView2CreationProperties _creationProperties;

    public override async void OnCreate(IServiceProvider services, DataSourceState state)
    {
        if (state.CustomSettigns is not WebSessionDataSourceOption option)
        {
            // TODO:
            // Change Exception Type
            throw new Exception();
        }
        _environment = await CoreWebView2Environment.CreateAsync(
            userDataFolder: services.GetService<IStorageService>().GetEntityFolder(state)
        );
        _creationProperties = new CoreWebView2CreationProperties
        {
            Language = services.GetService<ILocalizationService>().GetAppCulture().Name,
            //BrowserExecutableFolder = option.BrowserExecutableFolder,
            UserDataFolder = services.GetService<IStorageService>().GetEntityFolder(state),
        };

        await _webview.EnsureCoreWebView2Async(_environment);
        //CoreWebView2CookieManager cookieManager = CoreWebView2CookieManager.GetForUser(state.WebView.CoreWebView2);

    }

    public override void OnDestroy()
    {
    }

    public void DeleteAllCookies()
    {
        _cookieManager.DeleteAllCookies();
    }

    public async Task<WebView2> CreateWebView()
    {
        WebView2 webView = new WebView2();
        webView.DefaultBackgroundColor = System.Drawing.Color.Transparent;
        webView.CreationProperties = _creationProperties;
        webView.NavigationCompleted += OnWebViewNavigationCompleted;
        return webView;
    }

    private void OnWebViewNavigationCompleted(object? sender, CoreWebView2NavigationCompletedEventArgs e)
    {
    }
}
