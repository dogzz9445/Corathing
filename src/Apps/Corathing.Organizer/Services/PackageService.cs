using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using Corathing.Contracts.Bases;
using Corathing.Contracts.Entries;

using Corathing.Contracts.Services;
using NuGet.Protocol.Core.Types;
using NuGet.Protocol;
using NuGet.Versioning;
using NuGet.Packaging.Core;
using NuGet.Packaging;
using Microsoft.Extensions.DependencyInjection;
using System.Resources;
using Microsoft.Extensions.Localization;
using Corathing.Dashboards.WPF.Services;
using Corathing.Contracts.Attributes;

namespace Corathing.Organizer.Services;


public class ProxyDomain : MarshalByRefObject
{
    public Assembly? GetAssembly(string assemblyPath)
    {
        try
        {
            return Assembly.Load(assemblyPath);
        }
        catch (Exception)
        {
            return null;
        }
    }
}
public class PackageService : IPackageService
{
    /// <summary>
    /// Gets or sets the available widgets.
    /// </summary>
    /// <value>The available widgets.</value>
    private readonly Dictionary<string, CoraWidgetGenerator> _widgetGenerators = new Dictionary<string, CoraWidgetGenerator>();

    private readonly IServiceProvider _services;

    private NuGet.Common.ILogger _nugetLogger;

    public PackageService(IServiceProvider services)
    {
        _services = services;

        // FIXME:
        _nugetLogger = new NuGet.Common.NullLogger();
    }


    // ------------------------------------------------------------------------------------------------------
    //
    // 2. Nuget Package Download and Load
    //
    // ------------------------------------------------------------------------------------------------------


    public List<CoraWidgetGenerator> GetWidgetGenerators()
    {
        return _widgetGenerators.Values.ToList();
    }

    public bool TryGetWidgetGenerator(string contextTypeFullName, out CoraWidgetGenerator generator)
    {
        return _widgetGenerators.TryGetValue(contextTypeFullName, out generator);
    }

    public void UnloadAssembly(PackageState packageState)
    {

    }

    public void LoadWidgetsFromDLL(string pathDLL)
    {
        Assembly assembly = Assembly.LoadFrom(pathDLL);
        LoadAssembly(assembly);
    }

    public void LoadWidgetsFromNuget(string id, string version, string? nugetFeedUrl = null)
    {
        //bool cacheHasFinded = false;
        //var assembly = FindLocalPackagesResource();
        //if (cacheHasFinded)
        //{
        //}
        var assembly = LoadAssemblyFromNugetWebAsymc(id, version, nugetFeedUrl).Result;
        //var assembly = LoadAssemblyFromNugetFile(id, version, nugetFeedUrl).Result;
        LoadAssembly(assembly);
    }


    // ------------------------------------------------------------------------------------------------------
    //
    // 2. Load and Unload Assembly Internal
    //
    // ------------------------------------------------------------------------------------------------------

    private void LoadAssembly(Assembly assembly)
    {
        // Load Assembly DataTemplates
        assembly.GetCustomAttributes<AssemblyCoraPackageDataTemplateAttribute>()
            .ToList()
            .ForEach(attribute =>
                App.Current.Resources.MergedDictionaries.Add(GetDataTemplateFromCoraAttribute(assembly, attribute)
                ));

        // Load Assembly Localization
        assembly.GetCustomAttributes<AssemblyCoraPackageResourceManagerAttribute>()
            .ToList()
            .ForEach(attribute =>
            {
                if (attribute == null ||
                    attribute.ResourceManagerParentType == null ||
                    string.IsNullOrEmpty(attribute.NameofResourceManager))
                    return;

                var property = attribute.ResourceManagerParentType
                    .GetProperty(attribute.NameofResourceManager);
                if (property == null)
                    return;

                var propertyValue = property.GetValue(null, null);
                if (propertyValue is ResourceManager resourceManager)
                {
                    LocalizationService.Instance
                        .RegisterStringResourceManager(
                            assembly.GetType().Name,
                            resourceManager
                        );
                }
            });

        PackageState packageState = new PackageState()
        {
            Id = Guid.NewGuid(),
        };

        PackageReferenceState packageReferenceState = new PackageReferenceState()
        {
            AssemblyName = assembly.GetName().Name,
            AssemblyVersion = assembly.GetName().Version.ToString()
        };

        var types = assembly.GetTypes().Where(t => typeof(WidgetContext).IsAssignableFrom(t));
        // TODO:
        // 도메인 프록시 문제 해결 필요
        //var setup = new AppDomainSetup
        //{
        //    ApplicationBase = AppDomain.CurrentDomain.BaseDirectory,
        //    PrivateBinPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
        //};

        // Load Cora Widgets
        foreach (var type in types)
        {
            // TODOTODOTODOTODO
            // TODOTODO
            System.Reflection.MemberInfo info = type;
            var attributes = info.GetCustomAttributes(true);

            for (int i = 0; i < attributes.Length; i++)
            {
                if (!(attributes[i] is EntryCoraWidgetAttribute))
                    continue;

                var attribute = ((EntryCoraWidgetAttribute)attributes[i]);
                attribute.Configure(_services);
                _widgetGenerators.Add(attribute.Generator.ContextType.FullName, attribute.Generator);
            }
        }
    }


    // ------------------------------------------------------------------------------------------------------
    //
    // 3. Nuget Package Download and Load
    //
    // ------------------------------------------------------------------------------------------------------


    public async Task<Assembly> LoadAssemblyFromNugetWebAsymc(string id, string version, string? nugetFeedUrl = null, CancellationToken cancellationToken = default)
    {
        var repository = Repository.Factory.GetCoreV3(nugetFeedUrl ?? "https://api.nuget.org/v3/index.json");
        var downloadResource = await repository.GetResourceAsync<DownloadResource>();
        if (!NuGetVersion.TryParse(version, out var nuGetVersion))
        {
            throw new Exception($"invalid version {version} for nuget package {id}");
        }
        using var downloadResourceResult = await downloadResource.GetDownloadResourceResultAsync(
            new PackageIdentity(id, nuGetVersion),
            new PackageDownloadContext(new SourceCacheContext()),
            globalPackagesFolder: Path.GetTempPath(),
            logger: _nugetLogger,
            token: cancellationToken);

        if (downloadResourceResult.Status != DownloadResourceResultStatus.Available)
        {
            throw new Exception($"Download of NuGet package failed. DownloadResult Status: {downloadResourceResult.Status}");
        }

        var reader = downloadResourceResult.PackageReader;

        var archive = new ZipArchive(downloadResourceResult.PackageStream);

        var lib = reader.GetLibItems().First()?.Items.First();

        var entry = archive.GetEntry(lib);

        using var decompressed = new MemoryStream();
        entry.Open().CopyTo(decompressed);

        var assemblyLoadContext = new System.Runtime.Loader.AssemblyLoadContext(null, isCollectible: true);
        decompressed.Position = 0;
        return assemblyLoadContext.LoadFromStream(decompressed);
    }

    private Assembly LoadAssemblyFromNugetFile(string filename)
    {
        using var nugetStream = File.OpenRead(filename);
        using var archiveReader = new PackageArchiveReader(nugetStream);

        var lib = archiveReader.GetLibItems().First()?.Items.First();

        var entry = archiveReader.GetEntry(lib);

        using var decompressed = new MemoryStream();
        entry.Open().CopyTo(decompressed);

        var assemblyLoadContext = new System.Runtime.Loader.AssemblyLoadContext(null, isCollectible: true);
        decompressed.Position = 0;
        return assemblyLoadContext.LoadFromStream(decompressed);
    }

    // ------------------------------------------------------------------------------------------------------
    //
    // 4. Read Custom Attributes from Assembly
    //
    // ------------------------------------------------------------------------------------------------------


    private ResourceDictionary GetDataTemplateFromCoraAttribute(Assembly assembly, AssemblyCoraPackageDataTemplateAttribute dataTemplateSource)
    {
        if (string.IsNullOrEmpty(dataTemplateSource.DataTemplateSource))
        {
            // TODO:
            // Change Exception Type
            throw new Exception("DataTemplateSource is empty");
        }
        return new ResourceDictionary()
        {
            Source = new Uri(
                dataTemplateSource.IsAbsolute ?
                    dataTemplateSource.DataTemplateSource :
                    $"pack://application:,,,/{assembly.GetName().Name};component/{dataTemplateSource.DataTemplateSource}",
                UriKind.Absolute)
        };
    }

}
