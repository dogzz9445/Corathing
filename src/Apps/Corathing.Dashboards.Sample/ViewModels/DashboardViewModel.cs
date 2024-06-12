using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Corathing.Contracts.Bases;
using Corathing.Dashboards.Sample.Widgets;
using Corathing.Dashboards.WPF.Controls;

namespace Corathing.Dashboards.Sample.ViewModels;

public partial class DashboardViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<WidgetContext> _widgets;
    [ObservableProperty]
    private bool? _editMode;

    [RelayCommand]
    public void ToggleEditMode()
    {
        EditMode = !EditMode;
    }

    [RelayCommand]
    public void AddWidget()
    {
        Widgets.Add(new EmptyWidgetContext());
    }

    [RelayCommand]
    public void RemoveWidget(WidgetHost host)
    {
        var context = host.DataContext as WidgetContext;
        Widgets.Remove(context);
    }

    [RelayCommand]
    public void ConfigureWidget(WidgetHost host)
    {
        var context = host.DataContext as WidgetContext;
        // Do Something
    }

    public DashboardViewModel()
    {
        Widgets = new ObservableCollection<WidgetContext>();
        EditMode = false;
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);

        foreach (var widget in  Widgets)
        {
            widget.EditMode = EditMode;
        }
    }
}
