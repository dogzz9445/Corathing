using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Corathing.Contracts.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class EntryCoraDataSourceAttribute : Attribute
{
    public Type DataSourceType { get; }
    public Type? OptionType { get; }
    public Type? SettingsContextType { get; }

    public string Name;
    public string Description;
    public string DefaultTitle;

    public EntryCoraDataSourceAttribute(
        Type dataSourceType,
        Type? optionType = null,
        Type? settingsContextType = null,
        string? name = null,
        string? description = null,
        string? defaultTitle = null)
    {
        DataSourceType = dataSourceType;
        OptionType = optionType;
        SettingsContextType = settingsContextType;

        Name = name;
        Description = description;
        DefaultTitle = defaultTitle;
    }
}
