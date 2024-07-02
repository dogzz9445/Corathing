using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Corathing.Organizer.WPF.ViewModels;

public partial class DataSourceContextSettingsViewModel : ObservableObject
{
    #region Constructor with IServiceProvider
    private readonly IServiceProvider _services;

    public DataSourceContextSettingsViewModel(IServiceProvider services)
    {
        _services = services;
    }
    #endregion



    [RelayCommand]
    public void Apply()
    {

    }

    [RelayCommand]
    public void GoBack(Window window)
    {
        window.Close();
    }
}
