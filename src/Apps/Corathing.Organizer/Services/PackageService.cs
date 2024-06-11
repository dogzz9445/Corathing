﻿using System;
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
    private readonly List<CoraWidgetGenerator> _availableWidgets = new List<CoraWidgetGenerator>();
    private readonly IServiceProvider _services;

    private NuGet.Common.ILogger _nugetLogger;

    public PackageService(IServiceProvider services)
    {
        _services = services;

        // FIXME:
        _nugetLogger = new NuGet.Common.NullLogger();
    }


    public void RegisterWidgets(List<CoraWidgetGenerator> widgets)
    {
        _availableWidgets.AddRange(widgets);
    }
    public List<CoraWidgetGenerator> GetAvailableWidgets()
    {
        return _availableWidgets;
    }

    public void LoadAssembly(Assembly assembly)
    {
        var types = assembly.GetTypes().Where(t => typeof(WidgetContext).IsAssignableFrom(t));
        // TODO:
        // 도메인 프록시 문제 해결 필요
        //var setup = new AppDomainSetup
        //{
        //    ApplicationBase = AppDomain.CurrentDomain.BaseDirectory,
        //    PrivateBinPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
        //};
        ;
        foreach (var type in types)
        {
            System.Reflection.MemberInfo info = type;
            var attributes = info.GetCustomAttributes(true);

            for (int i = 0; i < attributes.Length; i++)
            {
                if (!(attributes[i] is EntryCoraWidgetAttribute))
                    continue;

                var attribute = ((EntryCoraWidgetAttribute)attributes[i]);
                attribute.Configure(_services);
                _availableWidgets.Add(attribute.Generator);

                App.Current.Resources.MergedDictionaries.Add(
                    new ResourceDictionary()
                    {
                        Source = new Uri($"pack://application:,,,/{assembly.GetName().Name};component/{attribute.Generator.DataTemplateSource}", UriKind.Absolute)
                    }
                    );
            }
        }
    }

    public void LoadWidgetsFromDLL(string pathDLL)
    {
        Assembly assembly = Assembly.LoadFrom(pathDLL);
        LoadAssembly(assembly);
    }

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

    public Assembly LoadAssemblyFromNugetFile(string filename)
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
}
