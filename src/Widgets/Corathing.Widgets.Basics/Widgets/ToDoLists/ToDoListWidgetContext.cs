using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using CommunityToolkit.Mvvm.Collections;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Corathing.Contracts.Attributes;

using Corathing.Contracts.Bases;

using Corathing.Contracts.Services;
using Corathing.UI.ObjectModel;
using Corathing.Widgets.Basics.DataSources.ToDos;

using Microsoft.Extensions.DependencyInjection;

namespace Corathing.Widgets.Basics.Widgets.ToDoLists;

public partial class ToDoContext : ObservableObject
{
    public ToDo ToDo { get; set; }

    [ObservableProperty]
    private bool _isDone;

    [ObservableProperty]
    private ToDoTaskType _taskType;

    [ObservableProperty]
    private string? _job;
}

[EntryCoraWidget(
    contextType: typeof(ToDoListWidgetContext),
    customSettingsType: typeof(ToDoListWidgetOption),
    customSettingsContextType: typeof(ToDoListWidgetOptionContext),
    name: "Create To Do List",
    description: "Create To Do List.",
    menuPath: "Default/To Do List",
    menuOrder: 0
    )]
public partial class ToDoListWidgetContext : WidgetContext
{
    private ObservableItemCollection<ToDoContext>? _toDos;
    public ObservableItemCollection<ToDoContext>? ToDos
    {
        get => _toDos;
        set
        {
            if (EqualityComparer<ObservableItemCollection<ToDoContext>?>.Default.Equals(_toDos, value))
                return;
            OnPropertyChanging(nameof(ToDos));
            _toDos = value;
            var itemsView = (IEditableCollectionView)CollectionViewSource.GetDefaultView(_toDos);
            itemsView.NewItemPlaceholderPosition = NewItemPlaceholderPosition.AtEnd;
            OnPropertyChanged(nameof(ToDos));
        }
    }

    [ObservableProperty]
    private ToDoDataSourceContext? _selectedToDoDataSourceContext;

    public override void OnCreate(IServiceProvider services, WidgetState state)
    {
        ILocalizationService localizationService = _services.GetService<ILocalizationService>();
        localizationService.Provide("Corathing.Widgets.Basics.ToDoListName", value => WidgetTitle = value);

        ToDos = new ObservableItemCollection<ToDoContext>();
        ToDos.ItemPropertyChanged += ToDos_ItemPropertyChanged;
    }

    private void ToDos_ItemPropertyChanged(object? sender, ItemPropertyChangedEventArgs args)
    {
        if (args.Sender is not ToDoContext todo)
        {
            return;
        }
        todo.ToDo = todo.ToDo.Copy(
            isDone: todo.IsDone,
            taskType: todo.TaskType,
            job: todo.Job
            );
        SelectedToDoDataSourceContext?.Update(todo.ToDo);
    }

    [RelayCommand]
    public void AddNewJob()
    {
        var todo = new ToDoContext()
        {
            ToDo = new ToDo()
        };

        ToDos.Add(todo);
        SelectedToDoDataSourceContext?.Add(todo.ToDo);
    }

    [RelayCommand]
    public void RemoveJob(ToDoContext todo)
    {
        SelectedToDoDataSourceContext?.Remove(todo.ToDo);
    }

    [RelayCommand]
    public void MarkJob(ToDoContext todo)
    {
        todo.IsDone = true;
        todo.ToDo = todo.ToDo.Copy(isDone: true);
        SelectedToDoDataSourceContext?.Update(todo.ToDo);
    }

    [RelayCommand]
    public void UnmarkJob(ToDoContext todo)
    {
        todo.IsDone = false;
        todo.ToDo = todo.ToDo.Copy(isDone: false);
        SelectedToDoDataSourceContext?.Update(todo.ToDo);
    }
}
