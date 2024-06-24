using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Corathing.Contracts.Services;

namespace Corathing.Organizer.Services;

public class NavigationService : INavigationService
{
    public bool GoBack()
    {
        throw new NotImplementedException();
    }

    public bool Navigate(Type pageType)
    {
        throw new NotImplementedException();
    }

    public bool Navigate(Type pageType, object? dataContext)
    {
        throw new NotImplementedException();
    }

    public bool Navigate(string pageIdOrTargetTag)
    {
        throw new NotImplementedException();
    }

    public bool Navigate(string pageIdOrTargetTag, object? dataContext)
    {
        throw new NotImplementedException();
    }

    public bool NavigateWithHierarchy(Type pageType)
    {
        throw new NotImplementedException();
    }

    public bool NavigateWithHierarchy(Type pageType, object? dataContext)
    {
        throw new NotImplementedException();
    }
}
