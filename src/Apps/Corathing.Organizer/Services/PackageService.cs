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
    private readonly Dictionary<string, CoraPackageInfo> _packageInfos;
    private readonly Dictionary<string, CoraPackageGenerator> _packageGenerators;

    private readonly Dictionary<string, CoraWidgetInfo> _widgetInfos;
    private readonly Dictionary<string, CoraWidgetGenerator> _widgetGenerators;

    private readonly Dictionary<string, CoraDataSourceInfo> _dataSourceInfos;
    private readonly Dictionary<string, CoraDataSourceGenerator> _dataSourceGenerator;

    private readonly IServiceProvider _services;

    private NuGet.Common.ILogger _nugetLogger;

    public PackageService(IServiceProvider services)
    {
        _services = services;

        // FIXME:
        _nugetLogger = new NuGet.Common.NullLogger();
        _packageInfos = new Dictionary<string, CoraPackageInfo>();
        _packageGenerators = new Dictionary<string, CoraPackageGenerator>();
        _widgetInfos = new Dictionary<string, CoraWidgetInfo>();
        _widgetGenerators = new Dictionary<string, CoraWidgetGenerator>();
        _dataSourceInfos = new Dictionary<string, CoraDataSourceInfo>();
        _dataSourceGenerator = new Dictionary<string, CoraDataSourceGenerator>();
    }

    // ------------------------------------------------------------------------------------------------------
    //
    // 1. Public Methods that implelemt IPackageService
    //
    // ------------------------------------------------------------------------------------------------------

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public void LoadWidgetsFromDLL(string pathDLL)
    {
        Assembly assembly = Assembly.LoadFrom(pathDLL);
        LoadAssembly(assembly);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
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

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public void UnloadAssembly(PackageState packageState)
    {

    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public List<ICoraWidgetInfo> GetAvailableWidgets()
    {
        return _widgetGenerators.Values.Select(generator => generator.Info).ToList();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public List<CoraWidgetGenerator> GetWidgetGenerators()
    {
        return _widgetGenerators.Values.ToList();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public WidgetContext CreateWidgetContext(string contextTypeFullName)
    {
        if (!_widgetGenerators.TryGetValue(contextTypeFullName, out var widgetContext))
            return null;
        return widgetContext.CreateWidget();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public Type? GetCustomSettingsType(string contextTypeFullName)
    {
        if (!_widgetGenerators.TryGetValue(contextTypeFullName, out var widgetContext))
            return null;
        return widgetContext.Info.WidgetCustomSettingsType;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public IWidgetCustomSettingsContext? CreateWidgetSettingsContext(string contextTypeFullName)
    {
        if (!_widgetGenerators.TryGetValue(contextTypeFullName, out var widgetContext))
            return null;
        return widgetContext.CreateSettingsContext();
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public PackageReferenceState GetPackageReferenceState(Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly);

        return GetPackageReferenceState(assembly.GetName().Name);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public PackageReferenceState GetPackageReferenceState(string? assemblyName)
    {
        ArgumentException.ThrowIfNullOrEmpty(assemblyName);

        _packageGenerators.TryGetValue(assemblyName, out var generator);
        return generator.GetOrCreateReferenceState();
    }


    // ------------------------------------------------------------------------------------------------------
    //
    // 2. Load and Unload Assembly Internal
    //
    // ------------------------------------------------------------------------------------------------------

    private void LoadAssembly(Assembly assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly);

        IResourceDictionaryService resourceDictionaryService = _services.GetRequiredService<IResourceDictionaryService>();
        ILocalizationService localizationService = _services.GetRequiredService<ILocalizationService>();

        // TODO:
        // 도메인 프록시 문제 해결 필요
        //var setup = new AppDomainSetup
        //{
        //    ApplicationBase = AppDomain.CurrentDomain.BaseDirectory,
        //    PrivateBinPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
        //};

        // 어셈블리의 정보로 PackageGenerator를 불러옴
        // Load Assembly PackageGenerator
        var packageGenerator = LoadCoraPackageGenerator(assembly);

        packageGenerator.GetResourceManagers().ForEach(resourceManager => localizationService
            .RegisterStringResourceManager(
                packageGenerator.GetAssemblyName(),
                resourceManager
            ));

        packageGenerator.GetDataTemplates().ForEach(uri =>
            resourceDictionaryService.RegisterResourceDictionary(uri));

        // 어셈블리의 DataSource 들을 불러옴
        // Load Assembly DataSources
        var dataSourceTypes = assembly.GetTypes().Where(t => typeof(DataSourceContext).IsAssignableFrom(t));

        // Load Cora DataSources
        foreach (var dataSourceType in dataSourceTypes)
        {
            var generator = LoadCoraDataSourceGenerator(assembly, dataSourceType);
            ArgumentNullException.ThrowIfNull(generator);

            string? dataSourceFullName = generator.GetDataSourceFullName();
            if (!string.IsNullOrEmpty(dataSourceFullName))
                _dataSourceGenerator.Add(dataSourceFullName, generator);
        }

        // 어셈블리의 Widget 들을 불러옴
        // Load Assembly Widgets
        var widgetTypes = assembly.GetTypes().Where(t => typeof(WidgetContext).IsAssignableFrom(t));

        // Load Cora Widgets
        foreach (var widgetType in widgetTypes)
        {
            var generator = LoadCoraWidgetGenerator(assembly, widgetType);
            ArgumentNullException.ThrowIfNull(generator);

            string? widgetFullName = generator.GetWidgetFullName();
            if (!string.IsNullOrEmpty(widgetFullName))
                _widgetGenerators.Add(widgetFullName, generator);
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

    private CoraPackageGenerator LoadCoraPackageGenerator(Assembly? assembly)
    {
        ArgumentNullException.ThrowIfNull(assembly);

        var coraPackageInfo = new CoraPackageInfo();
        var coraPackageGenerator = new CoraPackageGenerator(_services);
        coraPackageGenerator.PackageInfo = coraPackageInfo;

        var coraPackageAttributes = assembly.GetCustomAttributes<AssemblyCoraPackageNameAttribute>();

        var packageState = new PackageState()
        {
            Id = Guid.NewGuid(),
        };

        // TODO:
        // 20240702
        var packageReferenceState = coraPackageGenerator.GetOrCreateReferenceState();

        // 어셈블리의 DataTeamplate 을 불러옴
        // Load Assembly DataTemplates
        assembly.GetCustomAttributes<AssemblyCoraPackageDataTemplateAttribute>()
            .ToList()
            .ForEach(attribute =>
            coraPackageInfo.DataTemplates.Add(GetDataTemplateFromCoraAttribute(assembly, attribute)));

        // 어셈블리의 Localization 을 불러옴
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
                    coraPackageInfo.ResourceManagers.Add(resourceManager);
                }
            });

        return coraPackageGenerator;
    }

    private CoraDataSourceGenerator LoadCoraDataSourceGenerator(Assembly assembly, Type dataSourceType)
    {
        ArgumentNullException.ThrowIfNull(assembly);
        ArgumentNullException.ThrowIfNull(dataSourceType);

        System.Reflection.MemberInfo info = dataSourceType;
        var attributes = info.GetCustomAttributes(true);

        CoraDataSourceInfo coraDataSourceInfo = new CoraDataSourceInfo();
        CoraDataSourceGenerator generator = new CoraDataSourceGenerator(_services);
        for (int i = 0; i < attributes.Length; i++)
        {
            if (attributes[i] is EntryCoraDataSourceAttribute dataSourceAttribute)
            {
                coraDataSourceInfo.DataSourceType = dataSourceAttribute.DataSourceType;
                coraDataSourceInfo.OptionType = dataSourceAttribute.OptionType;
                coraDataSourceInfo.SettingsContextType = dataSourceAttribute.SettingsContextType;
                coraDataSourceInfo.Name = dataSourceAttribute.Name;
                coraDataSourceInfo.Description = dataSourceAttribute.Description;
                coraDataSourceInfo.DefaultTitle = dataSourceAttribute.DefaultTitle;
            }
            if (attributes[i] is EntryCoraDataSourceDefaultTitleAttribute dafultTitleAttribute)
            {
                if (coraDataSourceInfo.LocalizedDefaultTitles == null)
                    coraDataSourceInfo.LocalizedDefaultTitles = new();
                coraDataSourceInfo.LocalizedDefaultTitles[dafultTitleAttribute.Language] = dafultTitleAttribute.DefaultTitle;
            }
            if (attributes[i] is EntryCoraDataSourceNameAttribute nameAttribute)
            {
                if (coraDataSourceInfo.LocalizedNames == null)
                    coraDataSourceInfo.LocalizedNames = new();
                coraDataSourceInfo.LocalizedNames[nameAttribute.Language] = nameAttribute.Name;
            }
            if (attributes[i] is EntryCoraDataSourceDescriptionAttribute descriptionAttribute)
            {
                if (coraDataSourceInfo.LocalizedDescriptions == null)
                    coraDataSourceInfo.LocalizedDescriptions = new();
                coraDataSourceInfo.LocalizedDescriptions[descriptionAttribute.Language] = descriptionAttribute.Description;
            }
        }

        generator.Info = coraDataSourceInfo;
        return generator;
    }

    private CoraWidgetGenerator LoadCoraWidgetGenerator(Assembly? assembly, Type? widgetType)
    {
        ArgumentNullException.ThrowIfNull(assembly);
        ArgumentNullException.ThrowIfNull(widgetType);

        System.Reflection.MemberInfo info = widgetType;
        var attributes = info.GetCustomAttributes(true);

        CoraWidgetInfo coraWidgetInfo = new CoraWidgetInfo();
        CoraWidgetGenerator generator = new CoraWidgetGenerator(_services);
        for (int i = 0; i < attributes.Length; i++)
        {
            if (attributes[i] is EntryCoraWidgetAttribute entryCoraWidgetAttribute)
            {
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
            }
            if (attributes[i] is EntryCoraWidgetDefaultTitleAttribute defaultTitleAttribute)
            {
                if (coraWidgetInfo.LocalizedDefaultTitles == null)
                    coraWidgetInfo.LocalizedDefaultTitles = new();
                coraWidgetInfo.LocalizedDefaultTitles[defaultTitleAttribute.Language] = defaultTitleAttribute.DefaultTitle;
            }
            if (attributes[i] is EntryCoraWidgetNameAttribute nameAttribute)
            {
                if (coraWidgetInfo.LocalizedNames == null)
                    coraWidgetInfo.LocalizedNames = new();
                coraWidgetInfo.LocalizedNames[nameAttribute.Language] = nameAttribute.Name;
            }
            if (attributes[i] is EntryCoraWidgetDescriptionAttribute descriptionAttriute)
            {
                if (coraWidgetInfo.LocalizedDescriptions == null)
                    coraWidgetInfo.LocalizedDescriptions = new();
                coraWidgetInfo.LocalizedDescriptions[descriptionAttriute.Language] = descriptionAttriute.Description;
            }
            if (attributes[i] is EntryCoraWidgetMenuPathAttribute menuPathAttribute)
            {
                if (coraWidgetInfo.LocalizedMenuPaths == null)
                    coraWidgetInfo.LocalizedMenuPaths = new();
                coraWidgetInfo.LocalizedMenuPaths[menuPathAttribute.Language] = menuPathAttribute.MenuPath;
            }
            if (attributes[i] is EntryCoraWidgetMenuTooltipAttribute menuTooltipAttribute)
            {
                if (coraWidgetInfo.LocalizedMenuTooltips == null)
                    coraWidgetInfo.LocalizedMenuTooltips = new();
                coraWidgetInfo.LocalizedMenuTooltips[menuTooltipAttribute.Language] = menuTooltipAttribute.MenuTooltip;
            }
        }
        generator.Info = coraWidgetInfo;
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
