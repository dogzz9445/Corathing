using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;

using Corathing.Contracts.Attributes;
using Corathing.Contracts.Bases;
using Corathing.Contracts.Entries;
using Corathing.Contracts.Services;
using Corathing.Widgets.Basics.Widgets.Timers;
using Corathing.Widgets.Basics.Widgets.ToDoLists.Models;
using CommunityToolkit.Mvvm.Collections;

namespace Corathing.Widgets.Basics.Widgets.ToDoLists;

[EntryCoraWidget(
    contextType: typeof(ToDoListViewModel),
    name: "Create To Do List",
    description: "Provides a one by one square widget.",
    menuPath: "Default/To Do List",
    menuOrder: 0
    )]
public partial class ToDoListViewModel : WidgetContext
{
    private ObservableGroupedCollection<JobType, Job>? _groupedJobs;
    public ObservableGroupedCollection<JobType, Job>? GroupedJobs
    {
        get => _groupedJobs;
        set
        {
            if (EqualityComparer<ObservableGroupedCollection<JobType, Job>?>.Default.Equals(_groupedJobs, value))
                return;
            OnPropertyChanging(nameof(GroupedJobs));
            _groupedJobs = value;
            var itemsView = (IEditableCollectionView)CollectionViewSource.GetDefaultView(_groupedJobs);
            itemsView.NewItemPlaceholderPosition = NewItemPlaceholderPosition.AtEnd;
            OnPropertyChanged(nameof(GroupedJobs));
        }
    }

    public override void OnCreate(IServiceProvider services, WidgetState state)
    {
        ILocalizationService localizationService = _services.GetService<ILocalizationService>();
        localizationService.Provide("Corathing.Widgets.Basics.ToDoListName", value => WidgetTitle = value);

        GroupedJobs = new ObservableGroupedCollection<JobType, Job>();
    }

    [RelayCommand]
    public void AddNewJob()
    {
        GroupedJobs?.AddItem(JobType.Normal, new ToDoJob());
    }

    [RelayCommand]
    public void RemoveJob(ToDoJob job)
    {
        GroupedJobs?.RemoveItem(job.JobType, job);
    }

    [RelayCommand]
    public void MarkJob(ToDoJob job)
    {
        GroupedJobs?.RemoveItem(job.JobType, job);
        job.IsCompleted = true;
        GroupedJobs?.AddItem(JobType.IsCompleted, new ToDoJob());
    }

    [RelayCommand]
    public void UnmarkJob(ToDoJob job)
    {
        GroupedJobs?.RemoveItem(job.JobType, job);
        job.IsCompleted = false;
        GroupedJobs?.AddItem(JobType.IsCompleted, new ToDoJob());
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
