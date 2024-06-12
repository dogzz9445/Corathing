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
using Corathing.Widgets.Basics.Widgets.Notes;

using Microsoft.Extensions.DependencyInjection;

namespace Corathing.Widgets.Basics.Widgets.Timers;

[EntryCoraWidget(
    viewType: typeof(TimerWidget),
    contextType: typeof(TimerWidgetViewModel),
    dataTemplateSource: "Widgets/Timers/DataTemplates.xaml",
    name: "Create Timer",
    description: "Provides a one by one square widget.",
    menuPath: "Default/Timer",
    menuOrder: 0
    )]
public partial class TimerWidgetViewModel : WidgetContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OneByOneViewModel"/> class.
    /// </summary>
    public TimerWidgetViewModel(IServiceProvider services) : base(services)
    {
        ILocalizationService localizationService = services.GetService<ILocalizationService>();
        localizationService.Provide("Corathing.Widgets.Basics.TimerName", value => WidgetTitle = value);
    }
}

/// <summary>
/// Interaction logic for TimerWidget.xaml
/// </summary>
public partial class TimerWidget
{
    public TimerWidget()
    {
        InitializeComponent();
    }
}
