using System;
using System.Resources;

using Corathing.Contracts.Bases;
using Corathing.Contracts.Entries;

using Microsoft.Extensions.DependencyInjection;

namespace Corathing.Contracts.Utils.Generators;

public class CoraPackageGenerator
{
    public ICoraPackageInfo PackageInfo { get; set; }
    public List<CoraDataSourceGenerator> DataSources { get; }
    public List<CoraWidgetGenerator> Widgets { get; }

    private Guid? cachedId = null;
    private PackageState? _cachedPackageState = null;
    private PackageReferenceState? _cachedPackageReferenceState = null;

    public CoraPackageGenerator(IServiceProvider services)
    {
        DataSources = new List<CoraDataSourceGenerator>();
        Widgets = new List<CoraWidgetGenerator>();
    }

    /// <summary>
    /// TODO:
    /// PackageState 가 이미 설정 파일에 정의 되어있을 경우에
    /// 대한 처리가 필요함
    /// </summary>
    /// <param name="state"></param>
    /// <returns></returns>
    public PackageState GetOrCreateState()
    {
        if (_cachedPackageState != null)
            return _cachedPackageState;

        _cachedPackageState = new PackageState()
        {
            Id = Guid.NewGuid(),
            AssemblyName = GetAssemblyName(),
            AssemblyVersion = GetAssemblyVersion(),
            Title = PackageInfo.Name,
            Description = PackageInfo.Description,
        };

        return _cachedPackageState;
    }

    public PackageReferenceState GetOrCreateReferenceState()
    {
        if (_cachedPackageReferenceState != null)
            return _cachedPackageReferenceState;

        _cachedPackageReferenceState = new PackageReferenceState()
        {
            PackageId = GetOrCreateState().Id,
            PackageTitle = PackageInfo.Name,
            AssemblyName = GetAssemblyName(),
            AssemblyVersion = GetAssemblyVersion(),
        };

        return _cachedPackageReferenceState;
    }

    public List<Uri> GetDataTemplates()
    {
        return PackageInfo.DataTemplates;
    }


    public List<ResourceManager> GetResourceManagers()
    {
        return PackageInfo.ResourceManagers;
    }

    /// <summary>
    /// 위젯 컨텍스트의 어셈블리 이름을 가져옵니다.
    /// example:
    ///   "Corathing.Widgets.Basics"
    /// </summary>
    /// <returns><see cref="string"/>Widget context assembly name</returns>
    public string? GetAssemblyName()
    {
        ArgumentNullException.ThrowIfNull(PackageInfo.PacakageAssembly);

        return PackageInfo.PacakageAssembly.GetName().Name;
    }

    public string? GetAssemblyVersion()
    {
        ArgumentNullException.ThrowIfNull(PackageInfo.PacakageAssembly);

        var version = PackageInfo.PacakageAssembly.GetName().Version;
        ArgumentNullException.ThrowIfNull(version);

        return version.ToString();
    }

}
