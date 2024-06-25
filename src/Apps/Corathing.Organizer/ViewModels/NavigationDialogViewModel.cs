using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Corathing.Contracts.Services;

using Microsoft.Extensions.DependencyInjection;

namespace Corathing.Organizer.ViewModels;

public partial class NavigationDialogViewModel : ObservableObject
{
    [RelayCommand]
    public void GoBack()
    {
        INavigationDialogService navigationDialogServie = App.Current.Services.GetService<INavigationDialogService>();
        _ = navigationDialogServie.GoBack();
    }
}
