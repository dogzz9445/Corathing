using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Corathing.Contracts.Services;

namespace Corathing.Contracts.Entries;

public interface ICoraDataSourceInfo
{
    // Packages
    ICoraPackageInfo CoraPackageInfo { get; }

    // Types
    Type DataSourceType { get; }
    Type? OptionType { get; }
    Type? SettingsContextType { get; }

    // Information
    string Name { get; }
    string Description { get; }
    string DefaultTitle { get; }
    Dictionary<ApplicationLanguage, string> LocalizedNames { get; }
    Dictionary<ApplicationLanguage, string> LocalizedDescriptions { get; }
    Dictionary<ApplicationLanguage, string> LocalizedDefaultTitles { get; }
    List<string> Tags { get; set; }
}

public class CoraDataSourceInfo : ICoraDataSourceInfo
{
    // Packages
    public ICoraPackageInfo CoraPackageInfo { get; set; }

    // Types
    public Type DataSourceType { get; set; }
    public Type? OptionType { get; set; }
    public Type? SettingsContextType { get; set; }

    // Information
    public string Name { get; set; }
    public string Description { get; set; }
    public string DefaultTitle { get; set; }
    public Dictionary<ApplicationLanguage, string> LocalizedNames { get; set; }
    public Dictionary<ApplicationLanguage, string> LocalizedDescriptions { get; set; }
    public Dictionary<ApplicationLanguage, string> LocalizedDefaultTitles { get; set; }
    public List<string> Tags { get; set; }
}
