using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Corathing.Contracts.Bases;
using Corathing.Contracts.Entries;

namespace Corathing.Contracts.Services;

public interface IPackageService
{
    List<CoraWidgetGenerator> GetWidgetGenerators();
    bool TryGetWidgetGenerator(string contextTypeFullName, out CoraWidgetGenerator generator);

    void LoadWidgetsFromDLL(string pathDLL);
    void LoadWidgetsFromNuget(string id, string version, string? nugetFeedUrl = null);
    void UnloadAssembly(PackageState packageState);
}
