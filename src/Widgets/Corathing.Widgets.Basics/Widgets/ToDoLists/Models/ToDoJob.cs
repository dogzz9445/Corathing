using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;

namespace Corathing.Widgets.Basics.Widgets.ToDoLists.Models;

public partial class Job : ObservableObject
{
}

public partial class AddingJob : Job
{
}

public partial class ToDoJob : Job
{
    [DefaultValue(false)]
    [ObservableProperty]
    private bool _isCompleted;

    [ObservableProperty]
    private string? _detail;
}
