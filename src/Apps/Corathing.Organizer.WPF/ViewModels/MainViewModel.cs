using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

using Corathing.Contracts.Messages;
using Corathing.Contracts.Services;
using Corathing.Dashboards.WPF.Services;
using Corathing.Organizer.WPF.Services;

using Microsoft.Extensions.DependencyInjection;

using Smart.Collections.Generic;

using Wpf.Ui.Controls;

using INavigationView = Corathing.Contracts.Services.INavigationView;

namespace Corathing.Organizer.WPF.ViewModels;

/// <summary>
/// Extended <see cref="System.Windows.Controls.MenuItem"/> with <see cref="SymbolRegular"/> properties.
/// </summary>
public class MenuItem : System.Windows.Controls.MenuItem
{
    static MenuItem()
    {
        IconProperty.OverrideMetadata(typeof(MenuItem), new FrameworkPropertyMetadata(null));
    }

    /// <summary>
    /// Gets or sets displayed <see cref="IconElement"/>.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "WpfAnalyzers.DependencyProperty",
        "WPF0012:CLR property type should match registered type",
        Justification = "seems harmless"
    )]
    public new IconElement Icon
    {
        get => (IconElement)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }
}


public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<MenuItem> _trayMenuItems;

    public MainViewModel(IServiceProvider services)
    {
        TrayMenuItems = [new() { Header = "Home", Tag = "tray_home" }];
    }
}
