using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using CommunityToolkit.Mvvm.ComponentModel;

namespace Corathing.Organizer.ViewModels;

public partial class MenuItemViewModel : ObservableObject
{
    [ObservableProperty]
    private string _header;

    [ObservableProperty]
    private ICommand _command;

    [ObservableProperty]
    private ObservableCollection<MenuItemViewModel> _menuItems;
}
