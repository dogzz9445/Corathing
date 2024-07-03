using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

using Corathing.Contracts.Bases;

namespace Corathing.Contracts.Configurations;

public class AppPackageState
{
    public Guid Id { get; set; }
    public List<PackageState> Packages { get; set; }
    [JsonIgnore]
    public Dictionary<Guid, PackageState> CachedPackages { get; set; }

    public AppPackageState()
    {
        Packages = new List<PackageState>();
        CachedPackages = new Dictionary<Guid, PackageState>();
    }

    public void RemovePackage(PackageState package)
    {
        CachedPackages.Remove(package.Id);
        Packages.Remove(package);
    }

    public void UpdatePackage(PackageState package)
    {
        if (CachedPackages.ContainsKey(package.Id))
        {
            var oldPackage = CachedPackages[package.Id];
            if (oldPackage != null)
            {
                Packages.RemoveAll(item => item.Id == package.Id);
            }
        }
        CachedPackages[package.Id] = package;
        Packages.Add(package);
    }

    public void RefreshCache()
    {
        CachedPackages.Clear();

        foreach (var package in Packages)
        {
            CachedPackages[package.Id] = package;
        }
    }
}
