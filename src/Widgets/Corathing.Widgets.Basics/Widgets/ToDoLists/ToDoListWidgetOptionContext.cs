using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;

using Corathing.Contracts.DataContexts;
using Corathing.Widgets.Basics.DataSources.ToDos;

namespace Corathing.Widgets.Basics.Widgets.ToDoLists;

public partial class ToDoListWidgetOptionContext :
    CustomSettingsContext
{
    [ObservableProperty]
    private ToDoDataSourceSelector? _toDoDataSourceSelector;

    protected override void OnCreate(object? defaultOption)
    {
    }

    protected override void OnContextChanged()
    {
    }

    protected override void OnSettingsChanged(object? option)
    {
    }
}
