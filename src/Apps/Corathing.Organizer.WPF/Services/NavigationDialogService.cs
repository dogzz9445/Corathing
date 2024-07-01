using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

using Corathing.Contracts.Services;
using Corathing.Organizer.WPF.Controls;
using Corathing.Organizer.WPF.Views;

using Wpf.Ui;
using Wpf.Ui.Controls;

namespace Corathing.Organizer.WPF.Services;

public class NavigationDialogService : INavigationDialogService
{
    private INavigationService _uiNavigationService;
    private NavigationDialogView _navigationView;
    public NavigationDialogService(
        INavigationService uiNavigationService,
        NavigationDialogView navigationDialogView,
        IPageService pageService
        )
    {
        _uiNavigationService = uiNavigationService;
        _navigationView = navigationDialogView;
    }

    public bool GoBack()
    {
        bool isSucceeded = _uiNavigationService.GoBack();
        return isSucceeded;
    }

    public async Task<bool> Navigate(Type pageType, CancellationToken cancellationToken = default)
    {
        return await NavigateWithHierarchy(pageType);
    }

    public async Task<bool> Navigate(Type pageType, object? dataContext, CancellationToken cancellationToken = default)
    {
        return await NavigateWithHierarchy(pageType, dataContext);
    }

    public async Task<bool> Navigate(string pageIdOrTargetTag, CancellationToken cancellationToken = default)
    {
        return true;
    }

    public async Task<bool> Navigate(string pageIdOrTargetTag, object? dataContext, CancellationToken cancellationToken = default)
    {
        return true;
    }

    public async Task<bool> NavigateWithHierarchy(Type pageType, CancellationToken cancellationToken = default)
    {
        _navigationView.Show();
        return true;
    }

    public async Task<bool> NavigateWithHierarchy(Type pageType, object? dataContext, CancellationToken cancellationToken = default)
    {
        _navigationView.Show();
        return _uiNavigationService.NavigateWithHierarchy(typeof(MultiLevelNavigationPage), dataContext);
    }
}
