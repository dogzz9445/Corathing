using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    private bool _isShowDone;

    [ObservableProperty]
    private bool _isShowTask;

    [ObservableProperty]
    private ToDoDataSourceSelector? _toDoDataSourceSelector;

    protected override void OnCreate(object? defaultOption)
    {
        if (defaultOption is not ToDoListWidgetOption toDoListWidgetOption)
        {
            throw new ArgumentException($"Not a valid type for CustomSettings {nameof(ToDoListWidgetOption)}");
        }
        ToDoDataSourceSelector = new ToDoDataSourceSelector(Services);
        ToDoDataSourceSelector.PropertyChanged += (sender, args) =>
        {
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(ToDoDataSourceSelector)));
        };
        CustomSettings = toDoListWidgetOption;
    }

    protected override void OnContextChanged()
    {
        if (CustomSettings is not ToDoListWidgetOption toDoListWidgetOption)
        {
            throw new ArgumentException($"Not a valid type for CustomSettings {nameof(ToDoListWidgetOption)}");
        }
        toDoListWidgetOption.IsShowDone = IsShowDone;
        toDoListWidgetOption.IsShowTask = IsShowTask;

        if (ToDoDataSourceSelector != null &&
            ToDoDataSourceSelector.SelectedDataSourceContext != null)
        {
            toDoListWidgetOption.ToDoDataSourceId = ToDoDataSourceSelector.SelectedDataSourceContext.DataSourceId;
        }
        CustomSettings = toDoListWidgetOption;
    }

    protected override void OnSettingsChanged(object? option)
    {
        if (option is not ToDoListWidgetOption toDoListWidgetOption)
        {
            throw new ArgumentException($"Not a valid type for CustomSettings {nameof(ToDoListWidgetOption)}");
        }
        IsShowDone = toDoListWidgetOption.IsShowDone;
        IsShowTask = toDoListWidgetOption.IsShowTask;
        ToDoDataSourceSelector?.Select(toDoListWidgetOption.ToDoDataSourceId);
    }
}
