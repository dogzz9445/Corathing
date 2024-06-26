
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.Web.WebView2.Wpf;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Corathing.Contracts.Bases;
using Corathing.Widgets.Basics.Widgets.LinkOpeners;
using Corathing.Contracts.Services;
using Microsoft.Extensions.DependencyInjection;
using Corathing.Contracts.Entries;

namespace Corathing.Widgets.Basics.Widgets.Monacos;

[EntryCoraWidget(
    viewType: typeof(MonacoWidget),
    contextType: typeof(MonacoWidgetViewModel),
    name: "Create Monaco",
    description: "Provides a one by one square widget.",
    menuPath: "Default/Monaco",
    menuOrder: 0
    )]
public partial class MonacoWidgetViewModel : WidgetContext
{
    private MonacoController? _monacoController;

    private readonly IApplicationService _applicationService;

    [ObservableProperty]
    private WebView2 _webView;

    public MonacoWidgetViewModel(IServiceProvider services, WidgetState state) : base(services, state)
    {
        ILocalizationService localizationService = services.GetService<ILocalizationService>();
        localizationService.Provide("Corathing.Widgets.Basics.TextEditorName", value => WidgetTitle = value);
        VisibleTitle = false;

        _applicationService = services.GetService<IApplicationService>();
        WebView = new WebView2();
        WebView.Loaded += (s, e) =>
        {
            SetWebView(WebView);
        };
    }

    public void SetWebView(WebView2 webView)
    {
        webView.NavigationCompleted += OnWebViewNavigationCompleted;
        webView.SetCurrentValue(FrameworkElement.UseLayoutRoundingProperty, true);
        webView.SetCurrentValue(WebView2.DefaultBackgroundColorProperty, System.Drawing.Color.Transparent);
        webView.SetCurrentValue(
            WebView2.SourceProperty,
            new Uri(
                System.IO.Path.Combine(
                    System.AppDomain.CurrentDomain.BaseDirectory,
                    @"Assets\Monaco\index.html"
                )
            )
        );

        _monacoController = new MonacoController(_applicationService, webView);
    }

    [RelayCommand]
    public void OnMenuAction(string parameter) { }

    private async Task<bool> InitializeEditorAsync()
    {
        if (_monacoController == null)
        {
            return false;
        }
        await _monacoController.CreateAsync();

        var themeService = _services.GetService<IThemeService>();
        await _monacoController.DefineThemeAsync();
        if (themeService != null)
        {
            themeService.ProvideApplicationTheme(theme =>
            {
                _applicationService.InvokeAsync(OnThemeChanged);
            });
        }
        await _monacoController.SetLanguageAsync(MonacoLanguage.Csharp);
        await _monacoController.SetContentAsync(
            "// This Source Code Form is subject to the terms of the MIT License.\r\n// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.\r\n// Copyright (C) Leszek Pomianowski and WPF UI Contributors.\r\n// All Rights Reserved.\r\n\r\nnamespace Wpf.Ui.Gallery.Models.Monaco;\r\n\r\n[Serializable]\r\npublic record MonacoTheme\r\n{\r\n    public string Base { get; init; }\r\n\r\n    public bool Inherit { get; init; }\r\n\r\n    public IDictionary<string, string> Rules { get; init; }\r\n\r\n    public IDictionary<string, string> Colors { get; init; }\r\n}\r\n"
        );
        return true;
    }

    private async Task<bool> OnThemeChanged()
    {
        if (_monacoController == null)
        {
            return false;
        }

        var themeService = _services.GetService<IThemeService>();
        var theme = themeService.GetAppTheme();
        await _monacoController.SetThemeAsync(theme switch
        {
            ApplicationTheme.Dark => Wpf.Ui.Appearance.ApplicationTheme.Dark,
            ApplicationTheme.Light => Wpf.Ui.Appearance.ApplicationTheme.Light,
            ApplicationTheme.HighContrast => Wpf.Ui.Appearance.ApplicationTheme.HighContrast,
            _ => Wpf.Ui.Appearance.ApplicationTheme.Unknown
        });
        return true;
    }

    private void OnWebViewNavigationCompleted(
        object? sender,
        Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e
    )
    {
        _applicationService.InvokeAsync(InitializeEditorAsync);
    }
}

/// <summary>
/// Interaction logic for MonacoWidget.xaml
/// </summary>
public partial class MonacoWidget
{
    public MonacoWidget()
    {
        InitializeComponent();
    }
}
