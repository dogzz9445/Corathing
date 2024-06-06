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
using Corathing.Widgets.Basics.Widgets.Timers;

namespace Corathing.Widgets.Basics.Widgets.ToDoLists;

[WidgetContextEntry(
    name: "Create To Do List",
    description: "Provides a one by one square widget.",
    menuPath: "Default/To Do List",
    menuOrder: 0,
    targetType: typeof(ToDoListViewModel)
    )]
public partial class ToDoListViewModel : WidgetContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OneByOneViewModel"/> class.
    /// </summary>
    public ToDoListViewModel(IServiceProvider services) : base(services)
    {
        WidgetTitle = $"ToDoList";
    }
}

/// <summary>
/// Interaction logic for ToDoListWidget.xaml
/// </summary>
public partial class ToDoListWidget
{
    public ToDoListWidget()
    {
        InitializeComponent();
    }
}
