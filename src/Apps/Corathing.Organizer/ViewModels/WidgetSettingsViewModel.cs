﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Corathing.Organizer.ViewModels;

public partial class WidgetSettingsViewModel : ObservableObject
{
    public WidgetSettingsViewModel(IServiceProvider services)
    {

    }
    [RelayCommand]
    public void Close(Window window)
    {
        window.Close();
    }
}