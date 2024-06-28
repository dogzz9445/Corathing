using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Corathing.Contracts.Attributes;

/// <summary>
/// Resolve DataTemplate Source Path in Assembly
/// All defined DataTemplate Source Path will be loaded in Application.Current.Resources
/// Assembly 에 포함되는 DataTemplate 을 지정하는 Attribute
/// Organizer 에서 해당 DataTemplate 을 Application.Current.Resources 에 추가
/// isAbsolute 는 DataTemplateSource 가 상대경로인 경우 사용
///
/// Usage: <see cref="Uri"/>
/// Example:
///   AssemblyName       : Corathing.Widgets.Basics
///   Data Template Path :
///     Corathing.Widgets.Basics/
///     ├── Corathing.Widgets.Basics.proj
///     ├── DataTemplates.xaml
///     ├── Controls/
///     │   ├── ControlDataTemplates.xaml
/// [assembly: AssemblyCoraPackageDataTemplate("DataTemplates.xaml");
/// [assembly: AssemblyCoraPackageDataTemplate("ControlDataTemplates.xaml");
/// [assembly: AssemblyCoraPackageDataTemplate("pack://application:,,,/Corathing.Widgets;component/DataTemplates.xaml", true);
/// [assembly: AssemblyCoraPackageDataTemplate("pack://application:,,,/Corathing.Widgets;component/Controls/ControlDataTemplates.xaml", true);
/// </summary>
[AttributeUsage(AttributeTargets.Assembly)]
public class AssemblyCoraPackageDataTemplateAttribute : Attribute
{
    public bool IsAbsolute { get; set; }
    public string DataTemplateSource { get; set; }

    /// <summary>
    /// Resolve DataTemplate Source Path in Assembly
    /// All defined DataTemplate Source Path will be loaded in Application.Current.Resources
    /// Assembly 에 포함되는 DataTemplate 을 지정하는 Attribute
    /// Organizer 에서 해당 DataTemplate 을 Application.Current.Resources 에 추가
    /// isAbsolute 는 DataTemplateSource 가 상대경로인 경우 사용
    /// 
    /// Example:
    ///   AssemblyName       : Corathing.Widgets.Basics
    ///   Data Template Path :
    ///     Corathing.Widgets.Basics/
    ///     ├── Corathing.Widgets.Basics.proj
    ///     ├── DataTemplates.xaml
    ///     ├── Controls/
    ///     │   ├── ControlDataTemplates.xaml
    /// [assembly: AssemblyCoraPackageDataTemplate("DataTemplates.xaml");
    /// [assembly: AssemblyCoraPackageDataTemplate("ControlDataTemplates.xaml");
    /// [assembly: AssemblyCoraPackageDataTemplate("pack://application:,,,/Corathing.Widgets.Bascis;component/DataTemplates.xaml", true);
    /// [assembly: AssemblyCoraPackageDataTemplate("pack://application:,,,/Corathing.Widgets.Bascis;component/Controls/ControlDataTemplates.xaml", true);
    /// </summary>
    /// <param name="dataTemplateSource">DataTemplate Source To Load in App.Current.Resources</param>
    [MethodImpl(MethodImplOptions.NoInlining)]
    public AssemblyCoraPackageDataTemplateAttribute(string dataTemplateSource, bool isAbsolute = false)
    {
        if (string.IsNullOrEmpty(dataTemplateSource))
        {
            IsAbsolute = false;
            DataTemplateSource = "";
            return;
        }

        // FIXME:
        // relativeDataTemplateSource 가 정확히 어셈블리에 포함되는지 체크하는 로직 추가
        IsAbsolute = isAbsolute;
        DataTemplateSource = dataTemplateSource;
    }
}
