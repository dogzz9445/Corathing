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

namespace Corathing.Widgets.Basics.Widgets.ToDoLists;

[EntryCoraWidget(
    viewType: typeof(ToDoListWidget),
    contextType: typeof(ToDoListViewModel),
    name: "Create To Do List",
    description: "Provides a one by one square widget.",
    menuPath: "Default/To Do List",
    menuOrder: 0
    )]
public partial class ToDoListViewModel : WidgetContext
{
    private ObservableCollection<Job> _jobs;
    public ObservableCollection<Job> Jobs
    {
        get => _jobs;
        set
        {

            if (EqualityComparer<ObservableCollection<Job>?>.Default.Equals(_jobs, value))
                return;
            OnPropertyChanging(nameof(Jobs));
            _jobs = value;
            var itemsView = (IEditableCollectionView)CollectionViewSource.GetDefaultView(_jobs);
            itemsView.NewItemPlaceholderPosition = NewItemPlaceholderPosition.AtEnd;
            OnPropertyChanged(nameof(Jobs));
        }
    }

    [RelayCommand]
    public void AddNewJob()
    {
        Jobs.Add(new ToDoJob());
    }

    [RelayCommand]
    public void RemoveJob(ToDoJob job)
    {
        Jobs.Remove(job);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OneByOneViewModel"/> class.
    /// </summary>
    public ToDoListViewModel(IServiceProvider services, WidgetState state) : base(services, state)
    {
        ILocalizationService localizationService = services.GetService<ILocalizationService>();
        localizationService.Provide("Corathing.Widgets.Basics.ToDoListName", value => WidgetTitle = value);

        Jobs = new ObservableCollection<Job>();
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
