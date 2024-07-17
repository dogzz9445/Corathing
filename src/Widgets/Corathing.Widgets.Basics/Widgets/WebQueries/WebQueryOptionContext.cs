using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;

using Corathing.Contracts.DataContexts;
using Corathing.Widgets.Basics.DataSources.ExecutableApps;
using Corathing.Widgets.Basics.Widgets.FileOpeners;

namespace Corathing.Widgets.Basics.Widgets.WebQueries;

public partial class WebQueryOptionContext : CustomSettingsContext
{

    protected override void OnCreate(object? defaultOption)
    {
        if (defaultOption is not WebQueryOption webQueryOption)
        {
            throw new ArgumentException($"Not a valid type for CustomSettings {nameof(FileOpenerOption)}");
        }
        Urls = new ObservableCollection<string>();
        CustomSettings = webQueryOption;
    }

    protected override void OnContextChanged()
    {
        if (CustomSettings is not WebQueryOption webQueryOption)
        {
            throw new ArgumentException($"Not a valid type for CustomSettings {nameof(FileOpenerOption)}");
        }
        webQueryOption.Urls = Urls?.Select(url => url).ToList();
    }

    protected override void OnSettingsChanged(object? option)
    {
        if (option is not WebQueryOption webQueryOption)
        {
            throw new ArgumentException($"Not a valid type for CustomSettings {nameof(FileOpenerOption)}");
        }
        Urls?.Clear();

        webQueryOption.Urls?.ForEach(url => Urls?.Add(url));
    }

    [ObservableProperty]
    private ObservableCollection<string>? _urls;
}
