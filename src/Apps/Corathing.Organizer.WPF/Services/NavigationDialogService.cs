using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Navigation;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;

using Corathing.Contracts.Messages;
using Corathing.Contracts.Services;
using Corathing.Dashboards.WPF.Bindings;
using Corathing.Organizer.WPF.Controls;
using Corathing.Organizer.WPF.Views;

using Microsoft.EntityFrameworkCore.Metadata.Internal;

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
    private Frame? _navigationHost;
    private bool _isNavigating = false;

    public Stack<NavigationItem> _stackedUserControl;
    private NavigationItem? _current = null;

    public NavigationDialogService(
        MainWindow mainWindow
        )
    {
        _stackedUserControl = new Stack<NavigationItem>();
        _navigationHost = mainWindow.NavigationDialog;
        _navigationHost.Visibility = System.Windows.Visibility.Collapsed;
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
            _navigationHost.Content = _current.View;
            _current.View.OnBack(parameter);
            return true;
        }
        WeakReferenceMessenger.Default.Send(new NavigationStackChangedMessage(null));
        _current = null;
        _isNavigating = false;
        _navigationHost.Content = null;
        _navigationHost.Visibility = System.Windows.Visibility.Collapsed;
        return false;
    }

    public bool Navigate<T>(object? parameter = null) where T : INavigationView
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
        _navigationHost.Visibility = System.Windows.Visibility.Visible;
        _navigationHost.Content = _current.View;
        _current.View.OnForward(parameter);
        WeakReferenceMessenger.Default.Send(new NavigationStackChangedMessage(_current));
        return true;
    }

    //private INavigationService _uiNavigationService;
    //private NavigationDialogView _navigationView;
    //public bool GoBack()
    //{
    //    bool isSucceeded = _uiNavigationService.GoBack();
    //    return isSucceeded;
    //}

    //public async Task<bool> Navigate(Type pageType, CancellationToken cancellationToken = default)
    //{
    //    return await NavigateWithHierarchy(pageType);
    //}

    //public async Task<bool> Navigate(Type pageType, object? dataContext, CancellationToken cancellationToken = default)
    //{
    //    return await NavigateWithHierarchy(pageType, dataContext);
    //}

    //public async Task<bool> Navigate(string pageIdOrTargetTag, CancellationToken cancellationToken = default)
    //{
    //    return true;
    //}

    //public async Task<bool> Navigate(string pageIdOrTargetTag, object? dataContext, CancellationToken cancellationToken = default)
    //{
    //    return true;
    //}

    //public async Task<bool> NavigateWithHierarchy(Type pageType, CancellationToken cancellationToken = default)
    //{
    //    _navigationView.Show();
    //    return true;
    //}

    //public async Task<bool> NavigateWithHierarchy(Type pageType, object? dataContext, CancellationToken cancellationToken = default)
    //{
    //    _navigationView.Show();
    //    return _uiNavigationService.NavigateWithHierarchy(typeof(MultiLevelNavigationPage), dataContext);
    //}
}
