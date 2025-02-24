using System;
using System.Collections;
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
using Corathing.Widgets.Basics.DataSources.ExecutableApps;
using Corathing.Widgets.Basics.DataSources.ToDos;
using Corathing.Widgets.Basics.Widgets.Openers;

using GongSolutions.Wpf.DragDrop;

using Microsoft.Extensions.DependencyInjection;

using IDropTarget = GongSolutions.Wpf.DragDrop.IDropTarget;

namespace Corathing.Widgets.Basics.Widgets.ToDoLists;

public partial class ToDoContext : ObservableObject
{
    public ToDo ToDo { get; set; }

    private bool _isDone;
    public bool IsDone
    {
        get => _isDone;
        set
        {
            if (SetProperty(ref _isDone, value))
            {
                TaskType = IsDone ? ToDoTaskType.DONE : ToDoTaskType.TODO;
            }
        }
    }

    private ToDoTaskType _taskType;
    public ToDoTaskType TaskType
    {
        get => _taskType;
        set
        {
            if (SetProperty(ref _taskType, value))
            {
                IsDone = TaskType == ToDoTaskType.DONE;
                IsDropDownUnchecked = true;
                IsDropDownUnchecked = false;
            }
        }
    }

    [ObservableProperty]
    private string? _job;

    [ObservableProperty]
    private bool _isDropDownUnchecked;
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
public partial class ToDoListWidgetContext : WidgetContext, IDropTarget, IDragPreviewItemsSorter, IDropTargetItemsSorter
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

    [ObservableProperty]
    private bool _isShowDone;

    [ObservableProperty]
    private bool _isShowTask;

    public override void OnCreate(IServiceProvider services, WidgetState state)
    {
        ToDos = new ObservableItemCollection<ToDoContext>();
        ToDos.ItemPropertyChanged += ToDos_ItemPropertyChanged;
        IsShowDone = true;
        IsShowTask = true;
    }

    public override void OnStateChanged(WidgetState state)
    {
        if (state.CustomSettings is not ToDoListWidgetOption option)
        {
            return;
        }
        IsShowDone = option.IsShowDone;
        IsShowTask = option.IsShowTask;

        if (option.ToDoDataSourceId != null && option.ToDoDataSourceId != Guid.Empty)
        {
            var dataSourceService = _services.GetRequiredService<IDataSourceService>();
            SelectedToDoDataSourceContext = dataSourceService.GetDataSourceContext<ToDoDataSourceContext>(option.ToDoDataSourceId);
        }
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

    public void DragOver(IDropInfo dropInfo)
    {
        if (dropInfo.Data is not ToDoContext) return;
        if (dropInfo.TargetItem is not ToDoContext) return;

        dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
        dropInfo.Effects = System.Windows.DragDropEffects.Copy;
    }

    public void Drop(IDropInfo dropInfo)
    {
        if (dropInfo.Data is not ToDoContext sourceItem) return;
        if (dropInfo.TargetItem is not ToDoContext target) return;

    }

    // TODO:
    public IEnumerable SortDragPreviewItems(IEnumerable items)
    {
        var allItems = items.Cast<object>().ToList();
        if (allItems.Count > 0)
        {
            if (allItems[0] is ToDoContext)
            {
                return allItems.OrderBy(x => ((ToDoContext)x));
            }
        }

        return allItems;
    }

    public IEnumerable SortDropTargetItems(IEnumerable items)
    {
        throw new NotImplementedException();
    }
}
