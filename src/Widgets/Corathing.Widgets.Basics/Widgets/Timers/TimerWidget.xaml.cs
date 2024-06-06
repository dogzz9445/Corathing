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

using Corathing.Contracts.Attributes;
using Corathing.Contracts.Bases;
using Corathing.Widgets.Basics.Widgets.Notes;

namespace Corathing.Widgets.Basics.Widgets.Timers;

[WidgetContextEntry(
    name: "Create Timer",
    description: "Provides a one by one square widget.",
    menuPath: "Default/Timer",
    menuOrder: 0,
    targetType: typeof(TimerWidgetViewModel)
    )]
public partial class TimerWidgetViewModel : WidgetContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OneByOneViewModel"/> class.
    /// </summary>
    public TimerWidgetViewModel(IServiceProvider services) : base(services)
    {
        WidgetTitle = $"Timer";
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
