﻿using System;
using System.Reflection;
using System.Resources;

using Corathing.Contracts.Services;

namespace Corathing.Contracts.Entries;

public interface ICoraPackageInfo
{
    Guid? AssemblyGuid { get; }
    Assembly PacakageAssembly { get; }

    // Infos
    string Name { get; }
    string Description { get; }
    string Version { get; }
    string Author { get; }
    string Website { get; }
    string License { get; }
    string Repository { get; }
    string Icon { get; }
    string DownloadUrl { get; }
    string InstallScript { get; }
    string UninstallScript { get; }
    string UpdateScript { get; }
    string[] Dependencies { get; }
    string[] OptionalDependencies { get; }
    string[] Conflicts { get; }
    List<string> Tags { get; }

    Dictionary<ApplicationLanguage, string> LocalizedNames { get; }
    Dictionary<ApplicationLanguage, string> LocalizedDescriptions { get; }

    List<Uri> DataTemplates { get; }
    List<ResourceManager> ResourceManagers { get; }

}

public class CoraPackageInfo : ICoraPackageInfo
{
    public Guid? AssemblyGuid { get; set; }
    public Assembly PacakageAssembly { get; set; }

    // Infos
    public string Name { get; set; }
    public string Description { get; set; }
    public string Version { get; set; }
    public string Author { get; set; }
    public string Website { get; set; }
    public string License { get; set; }
    public string Repository { get; set; }
    public string Icon { get; set; }
    public string DownloadUrl { get; set; }
    public string InstallScript { get; set; }
    public string UninstallScript { get; set; }
    public string UpdateScript { get; set; }
    public string[] Dependencies { get; set; }
    public string[] OptionalDependencies { get; set; }
    public string[] Conflicts { get; set; }
    public List<string> Tags { get; set; }

    public Dictionary<ApplicationLanguage, string> LocalizedNames { get; set; }
    public Dictionary<ApplicationLanguage, string> LocalizedDescriptions { get; set; }

    public List<Uri> DataTemplates { get; set; }
    public List<ResourceManager> ResourceManagers { get; set; }

    public CoraPackageInfo()
    {
        Tags = new();
        LocalizedNames = new();
        LocalizedDescriptions = new();
        DataTemplates = new();
        ResourceManagers = new();
    }
}
