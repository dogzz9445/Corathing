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
    [ObservableProperty]
    private JobType _jobType;
}

public partial class AddingJob : Job
{
}

public enum JobType
{
    Normal,
    IsCompleted,
    Placeholder
}

public partial class ToDoJob : Job
{
    [DefaultValue(false)]
    [ObservableProperty]
    private bool _isCompleted;

    [ObservableProperty]
    private string? _detail;
}
