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
using Corathing.Contracts.Bases;
using Corathing.Contracts.Entries;
using Corathing.Widgets.Basics.Widgets.FileOpeners;

using Wpf.Ui.Controls;

namespace Corathing.Widgets.Basics.Widgets.Calendars;

[EntryCoraWidget(
    viewType: typeof(CalendarWidget),
    contextType: typeof(CalendarWidgetViewModel),
    dataTemplateSource: "Widgets/Calendars/DataTemplates.xaml",
    description: "Provides a calendar widget.",
    name: "Create Calendar",
    menuPath: "Default/Calendar",
    menuOrder: 0
    )]
public partial class CalendarWidgetViewModel : WidgetContext
{
    [ObservableProperty]
    private string? _filePath;

    [ObservableProperty]
    private string? _fileType;

    /// <summary>
    /// Initializes a new instance of the <see cref="OneByOneViewModel"/> class.
    /// </summary>
    public CalendarWidgetViewModel(
        IServiceProvider services)
        : base(services)
    {
        WidgetTitle = $"Calendar";
        VisibleTitle = false;

        var x = new DataTemplate()
        {
            DataType = typeof(CalendarWidgetViewModel),
            VisualTree = new FrameworkElementFactory(typeof(CalendarWidget))
        };
    }
}

/// <summary>
/// Interaction logic for CalendarWidget.xaml
/// </summary>
public partial class CalendarWidget
{
    public CalendarWidget()
    {
        InitializeComponent();
    }
}
