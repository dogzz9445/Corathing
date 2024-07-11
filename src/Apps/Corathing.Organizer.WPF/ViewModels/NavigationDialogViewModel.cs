using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using Corathing.Contracts.Services;
using Corathing.Dashboards.WPF.Services;
using Corathing.Organizer.WPF.Services;

using Microsoft.Extensions.DependencyInjection;

using Smart.Collections.Generic;

namespace Corathing.Organizer.WPF.ViewModels;

public partial class NavigationDialogViewModel : ObservableObject
{
    private readonly IServiceProvider _services;

    [ObservableProperty]
    private ObservableCollection<NavigationItem> _navigationItems;

    public NavigationDialogViewModel(IServiceProvider services)
    {
        _services = services;
        NavigationItems = new ObservableCollection<NavigationItem>();

        LocalizationService.Instance.PropertyChanged += (s, e) => OnPropertyChanged("Localization");
        WeakReferenceMessenger.Default.Register<NavigationStackChangedMessage>(this, OnNavigationStackChanged);
    }

    [RelayCommand]
    public void GoBack()
    {
        INavigationDialogService navigationDialogService
            = _services.GetRequiredService<INavigationDialogService>();
        navigationDialogService.GoBack();
    }


    private void OnNavigationStackChanged(object recipient, NavigationStackChangedMessage message)
    {
        var navigationItem = message.Value;
        if (navigationItem == null)
        {
            NavigationItems.Clear();
        }
        else if (NavigationItems.Count > navigationItem.Index)
        {
            NavigationItems.RemoveWhere((item) => item.Index > navigationItem.Index);
        }
        else
        {
            NavigationItems.Add(navigationItem);
        }
    }
}
