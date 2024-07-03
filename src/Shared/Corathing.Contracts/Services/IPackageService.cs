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

// TODO:
// 1. Guid 기반으로 변경
// 2. XML 주석 추가
public interface IPackageService
{
    // --------------------------------------------------------------------
    // Assembly
    // --------------------------------------------------------------------

    void LoadDLL(string pathDLL);
    void LoadNugetFromFile(string pathNugetFile);
    Task LoadNugetFromWebAsync(string id, string version, string? nugetFeedUrl = null);
    void UnloadAssembly(Guid packageId);

    // --------------------------------------------------------------------
    // Packages
    // --------------------------------------------------------------------

    PackageReferenceState GetPackageReferenceState(Assembly assembly);
    PackageReferenceState GetPackageReferenceState(string? assemblyName);

    // --------------------------------------------------------------------
    // Data Sources
    // --------------------------------------------------------------------
    List<ICoraDataSourceInfo> GetAvailableDataSources();
    DataSourceContext? CreateDataSourceContext(string contextTypeFullName);
    //DataSourceContext? GetDataSourceContext(Guid dataSourceId);
    Type? GetDataSourceCustomSettingsType(string contextTypeFullName);
    CustomSettingsContext? CreateDataSourceSettingsContext(string contextTypeFullName);
    Type? GetDataSourceSettingsContextType(string? contextTypeFullName);

    // --------------------------------------------------------------------
    // Widgets
    // --------------------------------------------------------------------
    List<ICoraWidgetInfo> GetAvailableWidgets();
    WidgetContext? CreateWidgetContext(string contextTypeFullName);
    Type? GetWidgetCustomSettingsType(string contextTypeFullName);
    CustomSettingsContext? CreateWidgetSettingsContext(string contextTypeFullName);
}
