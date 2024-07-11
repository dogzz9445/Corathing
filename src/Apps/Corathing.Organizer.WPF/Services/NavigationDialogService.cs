using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;

using Corathing.Contracts.DataContexts;
using Corathing.Contracts.Messages;
using Corathing.Contracts.Services;
using Corathing.Dashboards.WPF.Bindings;
using Corathing.Organizer.WPF.Controls;
using Corathing.Organizer.WPF.Views;

using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.DependencyInjection;

using Wpf.Ui;
using Wpf.Ui.Controls;

using INavigationView = Corathing.Contracts.Services.INavigationView;
using UserControl = System.Windows.Controls.UserControl;

namespace Corathing.Organizer.WPF.Services;

public class NavigationStackChangedMessage : ValueChangedMessage<NavigationItem?>
{
    public NavigationStackChangedMessage(NavigationItem? item) : base(item)
    {
    }
}

public class NavigationItem
{
    public string Header { get; set; } = string.Empty;
    public object Tag { get; set; } = string.Empty;
    public int Index { get; set; }
    public INavigationView View { get; set; }
}

public class NavigationDialogService : INavigationDialogService
{
    private readonly IServiceProvider _services;
    private bool _isNavigating = false;

    public Stack<NavigationItem> _stackedUserControl;
    private NavigationItem? _current = null;

    private MainWindow? _mainWindow;
    private BaseWindow? _baseWindow;

    public NavigationDialogService(
        IServiceProvider services,
        MainWindow mainWindow
        )
    {
        _services = services;
        _mainWindow = mainWindow;
        _stackedUserControl = new Stack<NavigationItem>();
        _isNavigating = false;
    }

    public bool Navigate(int index)
    {
        // TODO:
        //  수정 필요
        //if (index < _stackedUserControl.Count)
        //{
        //    _current = _stackedUserControl.ElementAt(index);
        //    WeakReferenceMessenger.Default.Send(new NavigationStackChangedMessage(_current));
        //    _navigationHost.Content = _current.View;
        //    return true;
        //}
        return false;
    }

    public bool GoBack(object? parameter = null)
    {
        if (_stackedUserControl.Count > 0)
        {
            _current = _stackedUserControl.Pop();
            WeakReferenceMessenger.Default.Send(new NavigationStackChangedMessage(_current));
            _baseWindow.SetDialogView(_current.View);
            _current.View.OnBack(parameter);
            return true;
        }
        WeakReferenceMessenger.Default.Send(new NavigationStackChangedMessage(null));
        _current = null;
        _isNavigating = false;
        CloseDialg();
        return false;
    }

    public async Task<bool> Navigate<T>(object? parameter = null) where T : INavigationView
    {
        _isNavigating = true;
        if (_current != null)
        {
            _stackedUserControl.Push(_current);
        }
        var view = Activator.CreateInstance<T>();
        _current = new NavigationItem
        {
            Header = (view as Page).Title,
            Tag = (view as Page).Tag,
            Index = _stackedUserControl.Count,
            View = view
        };

        await WaitUntilOpenDialog();

        _baseWindow.SetDialogView(_current.View);
        _current.View.OnForward(parameter);
        WeakReferenceMessenger.Default.Send(new NavigationStackChangedMessage(_current));
        return true;
    }

    public async Task<bool> Navigate(Type? viewType, object? parameter = null)
    {
        var view = Activator.CreateInstance(viewType);
        _isNavigating = true;
        if (_current != null)
        {
            _stackedUserControl.Push(_current);
        }
        _current = new NavigationItem
        {
            Header = (view as Page).Title,
            Tag = (view as Page).Tag,
            Index = _stackedUserControl.Count,
            View = view as INavigationView
        };

        await WaitUntilOpenDialog();

        _baseWindow.SetDialogView(_current.View);
        _current.View.OnForward(parameter);
        WeakReferenceMessenger.Default.Send(new NavigationStackChangedMessage(_current));
        return true;
    }

    public async Task WaitUntilOpenDialog()
    {
        if (_baseWindow == null)
        {
            OpenDialogAsync();
        }
        while (_baseWindow == null)
        {
            await Task.Delay(10);
        }
        while (!_baseWindow.IsLoaded)
        {
            await Task.Delay(10);
        }
    }

    private async void OpenDialogAsync()
    {
        if (_baseWindow != null)
        {
            return;
        }
        await Task.Yield();
        _baseWindow = _services.GetRequiredService<BaseWindow>();
        _baseWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        _baseWindow.Owner = _mainWindow;
        _baseWindow.Width = _mainWindow.Width;
        _baseWindow.Height = _mainWindow.Height;
        _baseWindow.ShowDialog();
    }

    private void CloseDialg()
    {
        _baseWindow.Close();
        _baseWindow = null;
    }

    public Task<bool> NavigateDataSourceSettings(Type? dataSourceType, object? dataSourceContext = null)
    {
        if (dataSourceContext != null)
        {
            if (dataSourceContext is not DataSourceContext)
            {
                throw new ArgumentException();
            }
            return Navigate(typeof(DataSourceSettingsView), dataSourceContext);
        }
        return Navigate(typeof(DataSourceSettingsView), dataSourceType);
    }
}
