using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

using Corathing.Contracts.Services;
using Corathing.Organizer.Views;

using Wpf.Ui;

namespace Corathing.Organizer.Services;

public class NavigationDialogService : INavigationDialogService
{
    private INavigationService _uiNavigationService;
    private IContentDialogService _contentDialogService;
    private MainWindow _mainWindow;

    public NavigationDialogService(
        INavigationService uiNavigationService,
        IContentDialogService contentDialogService,
        MainWindow mainWindow,
        IPageService pageService
        )
    {
        _uiNavigationService = uiNavigationService;
        _contentDialogService = contentDialogService;
        _mainWindow = mainWindow;
        _uiNavigationService.SetPageService(pageService);
    }

    public bool GoBack()
    {
        bool isSucceeded = _uiNavigationService.GoBack();
        return isSucceeded;
    }

    public bool Navigate(Type pageType)
    {
        return NavigateWithHierarchy(pageType);
    }

    public bool Navigate(Type pageType, object? dataContext)
    {
        return NavigateWithHierarchy(pageType, dataContext);
    }

    public bool Navigate(string pageIdOrTargetTag)
    {
        return true;
    }

    public bool Navigate(string pageIdOrTargetTag, object? dataContext)
    {
        return true;
    }

    public bool NavigateWithHierarchy(Type pageType)
    {
        return _uiNavigationService.NavigateWithHierarchy(pageType);
    }

    public bool NavigateWithHierarchy(Type pageType, object? dataContext)
    {
        return _uiNavigationService.NavigateWithHierarchy(pageType, dataContext);
    }
}
