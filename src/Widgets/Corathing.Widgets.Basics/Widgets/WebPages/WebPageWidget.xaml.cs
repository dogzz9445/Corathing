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

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Web.WebView2.Wpf;
using Microsoft.Extensions.DependencyInjection;

using Corathing.Contracts.Attributes;
using Corathing.Contracts.Bases;
using Corathing.Widgets.Basics.Widgets.ToDoLists;
using Corathing.Contracts.Services;
using Corathing.Contracts.Entries;

namespace Corathing.Widgets.Basics.Widgets.WebPages;

public enum WebPageViewMode
{
    Mobile,
    Desktop,
}

public enum SessionScope
{
    Application,
    Project,
    Workflow,
    Widget,
}

public enum SessionPersistence
{
    Persistent,

}

public partial class WebPageOption : ObservableObject
{
    [ObservableProperty]
    private string _url;

    [ObservableProperty]
    private SessionScope _sessionScope;

    [ObservableProperty]
    private SessionPersistence _sessionPersistence;

    [ObservableProperty]
    private WebPageViewMode _viewMode;

    [ObservableProperty]
    private int? autoReloadSeconds;
}

[EntryCoraWidget(
    contextType: typeof(WebPageViewModel),
    name: "Create Web Page",
    description: "Provides a one by one square widget.",
    menuPath: "Default/Web Page",
    menuOrder: 0
    )]
public partial class WebPageViewModel : WidgetContext
{
    [ObservableProperty]
    private WebPageOption? _options;

    [ObservableProperty]
    private WebView2 _webView;

    public override void OnCreate(WidgetState state)
    {
        ILocalizationService localizationService = _services.GetService<ILocalizationService>();
        localizationService.Provide("Corathing.Widgets.Basics.WebPageName", value => WidgetTitle = value);

        var appStateService = _services.GetService<IAppStateService>();
        //if (!widgetSystemService.TryGetWidgetOption<WebPageOption>(Uid, out var option))
        //WidgetTitle = option.Name;
    }

}

/// <summary>
/// Interaction logic for WebPageWidget.xaml
/// </summary>
public partial class WebPageWidget
{
    public WebPageWidget()
    {
        InitializeComponent();
    }
}
