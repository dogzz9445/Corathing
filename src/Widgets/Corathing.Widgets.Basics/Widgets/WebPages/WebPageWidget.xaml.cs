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
using Corathing.Contracts.Bases;
using Corathing.Widgets.Basics.Widgets.ToDoLists;
using CommunityToolkit.Mvvm.ComponentModel;
using Corathing.Contracts.Services;
using Microsoft.Web.WebView2.Wpf;
using Corathing.Contracts.Attributes;
using Microsoft.Extensions.DependencyInjection;

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

[WidgetContextEntry(
    name: "Create Web Page",
    description: "Provides a one by one square widget.",
    menuPath: "Default/Web Page",
    menuOrder: 0,
    targetType: typeof(WebPageViewModel)
    )]
public partial class WebPageViewModel : WidgetContext
{
    [ObservableProperty]
    private WebPageOption? _options;

    [ObservableProperty]
    private WebView2 _webView;

    /// <summary>
    /// Initializes a new instance of the <see cref="OneByOneViewModel"/> class.
    /// </summary>
    public WebPageViewModel(IServiceProvider services) : base(services)
    {
        var appStateService = services.GetService<IAppStateService>();
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
