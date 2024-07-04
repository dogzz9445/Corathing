using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.Input;

namespace Corathing.Contracts.Services;

public interface INavigationView
{
    void OnForward(object? parameter = null);
    void OnPreviewGoback(object? parameter = null);
    void OnBack(object? parameter = null);
}
