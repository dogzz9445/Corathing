using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Corathing.Contracts.Services;

namespace Corathing.Organizer.ViewModels;

public partial class NavigationDialogViewModel(
    INavigationDialogService navigationDialogServie
    ): ObservableObject
{
    [RelayCommand]
    public void GoBack()
    {
        _ = navigationDialogServie.GoBack();
    }
}
