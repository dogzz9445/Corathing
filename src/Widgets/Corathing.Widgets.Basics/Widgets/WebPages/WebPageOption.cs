using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;

namespace Corathing.Widgets.Basics.Widgets.WebPages;

public enum WebPageViewMode
{
    Mobile,
    Desktop,
}

public enum SessionScope
{
    Application,
    Project,
    Workflow,
    Widget,
}

public enum SessionPersistence
{
    Persistent,

}

public enum WebPageTheme
{
    App,
    Light,
    Dark,
}

public class WebPageOption
{
    public const string DefaultUrl = "https://github.com/dogzz9445/Corathing";

    /// <summary>
    /// Auto Reload Interval in seconds
    /// </summary>
    public int AutoReloadInterval { get; set; }

    /// <summary>
    /// Url of the web page
    /// </summary>
    public string? Url { get; set; } = DefaultUrl;

    /// <summary>
    /// 앱 테마와는 다른 테마 사용시 적용되는 테마 다크모드?
    /// </summary>
    public WebPageTheme IndependentTheme { get; set; } = WebPageTheme.App;

    public Guid? WebSessionDataSourceId { get; set; }
}
