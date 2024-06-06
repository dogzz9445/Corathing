using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Corathing.Organizer.ViewModels;

public partial class MenuItemViewModel : ObservableObject
{
    [ObservableProperty]
    private string _header;

    [ObservableProperty]
    private RelayCommand _command;

    [ObservableProperty]
    private ObservableCollection<MenuItemViewModel> _menuItems;
}
