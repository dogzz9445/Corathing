using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

using Corathing.Contracts.Attributes;
using Corathing.Contracts.Bases;
using Corathing.Contracts.DataContexts;
using Corathing.Contracts.Entries;
using Corathing.Contracts.Services;
using Corathing.Contracts.Utils.Generators;

using Microsoft.Extensions.DependencyInjection;

using NuGet.Packaging;
using NuGet.Packaging.Core;
using NuGet.Protocol;
using NuGet.Protocol.Core.Types;
using NuGet.Versioning;

namespace Corathing.Organizer.Services;

public class PackageService : IPackageService
{
    /// <summary>
    /// Gets or sets the available widgets.
    /// </summary>
    /// <value>The available widgets.</value>
    private readonly Dictionary<string, CoraWidgetGenerator> _widgetGenerators;
    private readonly Dictionary<string, CoraPackageGenerator> _packageGenerators;
    private readonly Dictionary<string, CoraDataSourceGenerator> _dataSourceGenerator;

    private readonly IServiceProvider _services;

    private NuGet.Common.ILogger _nugetLogger;

    public PackageService(IServiceProvider services)
    {
        _services = services;

        // FIXME:
        _nugetLogger = new NuGet.Common.NullLogger();
        _widgetGenerators = new Dictionary<string, CoraWidgetGenerator>();
        _packageGenerators = new Dictionary<string, CoraPackageGenerator>();
        _dataSourceGenerator = new Dictionary<string, CoraDataSourceGenerator>();
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

    public WidgetContext CreateWidgetContext(string contextTypeFullName)
    {
        if (!_widgetGenerators.TryGetValue(contextTypeFullName, out var widgetContext))
            return null;
        return widgetContext.CreateWidget();
    }

    public Type? GetCustomSettingsType(string contextTypeFullName)
    {
        if (!_widgetGenerators.TryGetValue(contextTypeFullName, out var widgetContext))
            return null;
        return widgetContext.Info.WidgetCustomSettingsType;
    }

    public IWidgetCustomSettingsContext? CreateWidgetSettingsContext(string contextTypeFullName)
    {
        if (!_widgetGenerators.TryGetValue(contextTypeFullName, out var widgetContext))
            return null;
        return widgetContext.CreateCustomSettingsContext();
    }

    public List<ICoraWidgetInfo> GetAvailableWidgets()
    {
        return _widgetGenerators.Values.Select(generator => generator.Info).ToList();
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
        IResourceDictionaryService resourceDictionaryService = _services.GetRequiredService<IResourceDictionaryService>();
        ILocalizationService localizationService = _services.GetRequiredService<ILocalizationService>();

        var coraPackageInfo = new CoraPackageInfo();
        var coraPackageGenerator = new CoraPackageGenerator(_services);


        var coraPackageAttribute = assembly.GetCustomAttribute<AssemblyCoraPackageNameAttribute>();

        var packageState = new PackageState()
        {
            Id = Guid.NewGuid(),
        };

        var packageReferenceState = new PackageReferenceState()
        {
            AssemblyName = assembly.GetName().Name,
            AssemblyVersion = assembly.GetName().Version.ToString()
        };

        // Load Assembly DataTemplates
        assembly.GetCustomAttributes<AssemblyCoraPackageDataTemplateAttribute>()
            .ToList()
            .ForEach(attribute =>
                resourceDictionaryService.RegisterResourceDictionary(GetDataTemplateFromCoraAttribute(assembly, attribute)
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
                    localizationService
                        .RegisterStringResourceManager(
                            assembly.GetType().Name,
                            resourceManager
                        );
                }
            });

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
            var generator = LoadCoraWidgetGenerator(assembly, type);
            if (generator != null)
                _widgetGenerators.Add(generator.Info.WidgetContextType.FullName, generator);
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

    private CoraWidgetGenerator LoadCoraWidgetGenerator(Assembly? assembly, Type? type)
    {
        if (assembly == null || type == null)
        {
            // TODO:
            // Change Exception Type
            throw new Exception();
        }
        System.Reflection.MemberInfo info = type;
        var attributes = info.GetCustomAttributes(true);

        CoraWidgetInfo coraWidgetInfo = new CoraWidgetInfo();
        CoraWidgetGenerator generator = new CoraWidgetGenerator(_services);
        for (int i = 0; i < attributes.Length; i++)
        {
            if (attributes[i] is EntryCoraWidgetAttribute entryCoraWidgetAttribute)
            {
                coraWidgetInfo.WidgetViewType = entryCoraWidgetAttribute.ViewType;
                coraWidgetInfo.WidgetContextType = entryCoraWidgetAttribute.ContextType;
                coraWidgetInfo.WidgetCustomSettingsType = entryCoraWidgetAttribute.CustomSettingsType;
                coraWidgetInfo.WidgetCustomSettingsContextType = entryCoraWidgetAttribute.CustomSettingsContextType;
                coraWidgetInfo.Name = entryCoraWidgetAttribute.Name;
                coraWidgetInfo.Description = entryCoraWidgetAttribute.Description;
                coraWidgetInfo.DefaultTitle = entryCoraWidgetAttribute.Title;
                coraWidgetInfo.DefaultVisibleTitle = entryCoraWidgetAttribute.VisibleTitle;
                coraWidgetInfo.MenuPath = entryCoraWidgetAttribute.MenuPath;
                coraWidgetInfo.MenuOrder = entryCoraWidgetAttribute.MenuOrder;
                coraWidgetInfo.MenuTooltip = entryCoraWidgetAttribute.MenuTooltip;
                coraWidgetInfo.MaximumColumnSpan = entryCoraWidgetAttribute.MaximumColumnSpan;
                coraWidgetInfo.MaximumRowSpan = entryCoraWidgetAttribute.MaximumRowSpan;
                coraWidgetInfo.DefaultColumnSpan = entryCoraWidgetAttribute.DefaultColumnSpan;
                coraWidgetInfo.DefaultRowSpan = entryCoraWidgetAttribute.DefaultRowSpan;
                coraWidgetInfo.MinimumColumnSpan = entryCoraWidgetAttribute.MinimumColumnSpan;
                coraWidgetInfo.MinimumRowSpan = entryCoraWidgetAttribute.MinimumRowSpan;
                generator.Info = coraWidgetInfo;
            }
            if (attributes[i] is EntryCoraWidgetDefaultTitleAttribute entryCoraWidgetTitleAttribute)
            {
                if (coraWidgetInfo.LocalizedTitles == null)
                    coraWidgetInfo.LocalizedTitles = new Dictionary<ApplicationLanguage, string>();
                coraWidgetInfo.LocalizedTitles[entryCoraWidgetTitleAttribute.Language] = entryCoraWidgetTitleAttribute.DefaultTitle;
            }

        }
        return generator;
    }

    private Uri GetDataTemplateFromCoraAttribute(Assembly? assembly, AssemblyCoraPackageDataTemplateAttribute? dataTemplateSource)
    {
        if (assembly == null ||
            dataTemplateSource == null ||
            string.IsNullOrEmpty(dataTemplateSource.DataTemplateSource)
            )
        {
            // TODO:
            // Change Exception Type
            throw new Exception("DataTemplateSource is empty");
        }
        return new Uri(
                dataTemplateSource.IsAbsolute ?
                    dataTemplateSource.DataTemplateSource :
                    $"pack://application:,,,/{assembly.GetName().Name};component/{dataTemplateSource.DataTemplateSource}",
                UriKind.Absolute);
    }
}
