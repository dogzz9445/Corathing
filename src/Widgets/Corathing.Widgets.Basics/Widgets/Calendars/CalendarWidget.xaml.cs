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

using Corathing.Contracts.Attributes;
using Corathing.Contracts.Bases;
using Corathing.Contracts.Entries;
using Corathing.Contracts.Services;

using Microsoft.Extensions.DependencyInjection;

using Wpf.Ui.Controls;

namespace Corathing.Widgets.Basics.Widgets.Calendars;

[EntryCoraWidget(
    contextType: typeof(CalendarWidgetViewModel),
    description: "Provides a calendar widget.",
    name: "Create Calendar",
    menuPath: "Default/Calendar",
    menuOrder: 0,
    visibleTitle: false
    )]
public partial class CalendarWidgetViewModel : WidgetContext
{
    [ObservableProperty]
    private string? _filePath;

    [ObservableProperty]
    private string? _fileType;

    public override void OnStateChanged(WidgetState state)
    {
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
