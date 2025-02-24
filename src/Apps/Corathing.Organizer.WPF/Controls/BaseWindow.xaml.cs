using System;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Windows.Interop;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using Corathing.Contracts.Services;
using Corathing.Dashboards.WPF.Services;
using Corathing.Organizer.WPF.Services;

using MahApps.Metro.Controls;

using Microsoft.Extensions.DependencyInjection;

using Smart.Collections.Generic;

namespace Corathing.Organizer.WPF.Controls;

public partial class BaseWindowViewModel : ObservableObject
{
    private readonly IServiceProvider _services;

    [ObservableProperty]
    private ObservableCollection<NavigationItem> _navigationItems;

    public BaseWindowViewModel(IServiceProvider services)
    {
        _services = services;
        NavigationItems = new ObservableCollection<NavigationItem>();

        LocalizationService.Instance.PropertyChanged += (s, e) => OnPropertyChanged("Localization");
        WeakReferenceMessenger.Default.Register<NavigationStackChangedMessage>(this, OnNavigationStackChanged);

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
    [RelayCommand]
    public void Goback()
    {
        INavigationDialogService navigationDialogService
            = _services.GetRequiredService<INavigationDialogService>();
        navigationDialogService.GoBack();
    }
}

/// <summary>
/// Interaction logic for BaseWindow.xaml
/// </summary>
public partial class BaseWindow : MetroWindow
{
    const int DWMWA_TRANSITIONS_FORCEDISABLED = 3;

    [DllImport("dwmapi", PreserveSig = true)]
    static extern int DwmSetWindowAttribute(IntPtr hWnd, int attr, ref int value, int attrLen);

    public BaseWindowViewModel ViewModel { get; set; }

    public BaseWindow(IServiceProvider services)
    {
        IntPtr windowHandle = new WindowInteropHelper(this).Handle;
        if (Environment.OSVersion.Version.Major >= 6)
        {
            int value = 1;  // TRUE to disable
            DwmSetWindowAttribute(windowHandle,
                                  DWMWA_TRANSITIONS_FORCEDISABLED,
                                  ref value,
                                  Marshal.SizeOf(value));
        }

        InitializeComponent();

        DataContext = ViewModel = services.GetRequiredService<BaseWindowViewModel>();
    }

    public void SetDialogView(INavigationView view)
    {
        DialogFrame.Content = view;
    }
}
