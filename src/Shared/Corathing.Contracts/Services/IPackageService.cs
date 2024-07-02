using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Corathing.Contracts.Bases;
using Corathing.Contracts.DataContexts;
using Corathing.Contracts.Entries;

namespace Corathing.Contracts.Services;

public interface IPackageService
{
    void LoadWidgetsFromDLL(string pathDLL);
    void LoadWidgetsFromNuget(string id, string version, string? nugetFeedUrl = null);
    void UnloadAssembly(PackageState packageState);

    List<ICoraWidgetInfo> GetAvailableWidgets();

    WidgetContext CreateWidgetContext(string contextTypeFullName);
    Type? GetCustomSettingsType(string contextTypeFullName);
    IWidgetCustomSettingsContext? CreateWidgetSettingsContext(string contextTypeFullName);
    PackageReferenceState GetPackageReferenceState(Assembly assembly);
    PackageReferenceState GetPackageReferenceState(string? assemblyName);

}
