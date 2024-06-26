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
using Corathing.Contracts.Entries;
using Corathing.Contracts.Services;
using Corathing.Widgets.Basics.Widgets.WebPages;

using Microsoft.Extensions.DependencyInjection;

namespace Corathing.Widgets.Basics.Widgets.WebQueries;

[EntryCoraWidget(
    viewType: typeof(WebQueryWidget),
    contextType: typeof(WebQueryViewModel),
    name: "Create Web Query",
    description: "Provides a one by one square widget.",
    menuPath: "Default/Web Query",
    menuOrder: 0
    )]
public partial class WebQueryViewModel : WidgetContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OneByOneViewModel"/> class.
    /// </summary>
    public WebQueryViewModel(IServiceProvider services, WidgetState state) : base(services, state)
    {
        ILocalizationService localizationService = services.GetService<ILocalizationService>();
        localizationService.Provide("Corathing.Widgets.Basics.WebQueryName", value => WidgetTitle = value);
    }
}

/// <summary>
/// Interaction logic for WebQueryWidget.xaml
/// </summary>
public partial class WebQueryWidget
{
    public WebQueryWidget()
    {
        InitializeComponent();
    }
}
