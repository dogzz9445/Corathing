using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Corathing.Contracts.Bases;
using Corathing.Contracts.DataContexts;
using Corathing.Contracts.Entries;
using Corathing.Contracts.Services;

using Microsoft.Extensions.DependencyInjection;

namespace Corathing.Contracts.Utils.Generators;

/// <summary>
/// DataSource 생성기
/// Assembly 에 정의된 정보를 기반으로 DataSource 생성
/// - DataSourceContext 생성
/// - DataSourceState 생성
/// - DataSourceCoreState 생성
/// - DataSource CustomSettings 생성
/// AssemblyName, DataSourceFullName, DisplayName, DisplayDescription, CustomSettings 생성
/// </summary>
/// <param name="services"></param>
public class CoraDataSourceGenerator(IServiceProvider services)
{
    public ICoraDataSourceInfo? Info { get; set; }

    public DataSourceContext CreateDataSource(DataSourceState? dataSourceState = null)
    {
        ArgumentNullException.ThrowIfNull(Info);
        ArgumentNullException.ThrowIfNull(Info.DataSourceType);

        var context = Activator.CreateInstance(Info.DataSourceType) as DataSourceContext;

        ArgumentNullException.ThrowIfNull(context);

        context.Initialize(services, dataSourceState ?? CreateState());

        return context;
    }

    private DataSourceState CreateState()
    {
        ArgumentNullException.ThrowIfNull(Info);
        ArgumentNullException.ThrowIfNull(Info.DataSourceType);

        DataSourceState state = new DataSourceState();
        state.Id = Guid.NewGuid();

        state.PackageReference = services
            .GetRequiredService<IPackageService>()
            .GetPackageReferenceState(GetAssemblyName());

        state.CoreSettings = new DataSourceCoreState()
        {
            TypeName = GetDataSourceFullName(),
            Title = GenerateDefaultTitle(),
            IsDependentOnWidget = false,
            DependencyWidget = null,
        };

        state.CustomSettigns = CreateCustomOption();

        return state;
    }

    public object? CreateCustomOption()
    {
        ArgumentNullException.ThrowIfNull(Info);

        if (Info.OptionType == null)
        {
            return null;
        }
        return Activator.CreateInstance(Info.OptionType);
    }

    public CustomSettingsContext? CreateSettingsContext()
    {
        ArgumentNullException.ThrowIfNull(Info);

        if (Info.OptionType == null || Info.SettingsContextType == null)
        {
            return null;
        }
        var option = CreateCustomOption();
        var settingsContext = Activator.CreateInstance(Info.SettingsContextType) as CustomSettingsContext;
        settingsContext?.Initialize(services, option);
        return settingsContext;
    }

    /// <summary>
    /// 네임스페이스를 포함한 데이터소스 컨텍스트의 전체 이름을 가져옵니다.
    /// Get the full name of the DataSource context including the namespace.
    /// example:
    ///   "Corathing.Widgets.Basics.DataSources.ExecutableApps.ExecutableAppDataSourceContext"
    /// </summary>
    /// <returns><see cref="string"/>DataSource Context Full Name</returns>
    public string? GetDataSourceFullName()
    {
        ArgumentNullException.ThrowIfNull(Info);
        ArgumentNullException.ThrowIfNull(Info.DataSourceType);

        return Info.DataSourceType.FullName;
    }

    /// <summary>
    /// 데이터소스 컨텍스트의 어셈블리 이름을 가져옵니다.
    /// example:
    ///   "Corathing.Widgets.Basics"
    /// </summary>
    /// <returns><see cref="string"/>DataSource context assembly name</returns>
    public string? GetAssemblyName()
    {
        ArgumentNullException.ThrowIfNull(Info);
        ArgumentNullException.ThrowIfNull(Info.DataSourceType);

        return Info.DataSourceType.Assembly.GetName().Name;
    }

    /// <summary>
    /// 데이터소스의 기본 제목으로 지정될 이름을 생성합니다.
    /// 다국어 제목이 없을 경우 기본 제목을 생성합니다.
    /// Get the title to be set on the DataSource title.
    /// Get the default title if there is no localized title.
    /// example returns:
    ///   ko-KR: "실행가능한 앱"
    ///   en-US: "Executable App"
    /// </summary>
    /// <returns><see cref="string"/>Default Title "Executable App"</returns>
    private string GenerateDefaultTitle()
    {
        ArgumentNullException.ThrowIfNull(Info);

        string? defaultTitle = null;
        if (Info.LocalizedDefaultTitles != null)
        {
            var localizationService = services.GetRequiredService<ILocalizationService>();
            var language = localizationService.GetAppLanguage();
            if (Info.LocalizedDefaultTitles.TryGetValue(language, out var localizedTitle))
            {
                defaultTitle = localizedTitle;
            }
        }
        if (string.IsNullOrEmpty(defaultTitle) && !string.IsNullOrEmpty(Info.DefaultTitle))
        {
            defaultTitle = Info.DefaultTitle;
        }
        return defaultTitle ?? Info.DataSourceType.Name;
    }

    /// <summary>
    /// 화면에 표시될 이름을 가져옵니다.
    /// 다국어 이름이 없을 경우 기본 이름을 가져옵니다.
    /// Get the name to be displayed on the screen.
    /// Get the default name if there is no localized name.
    /// example returns:
    ///   ko-KR: "실행가능한 앱 데이터소스"
    ///   en-US: "Executable App DataSource"
    /// </summary>
    /// <returns><see cref="string"/>Name "Executable App DataSource"</returns>
    public string GetDisplayName()
    {
        ArgumentNullException.ThrowIfNull(Info);

        string? name = null;
        if (Info.LocalizedNames != null)
        {
            var localizationService = services.GetRequiredService<ILocalizationService>();
            var language = localizationService.GetAppLanguage();
            if (Info.LocalizedNames.TryGetValue(language, out var localizedName))
            {
                name = localizedName;
            }
        }
        if (string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(Info.Name))
        {
            name = Info.Name;
        }
        return name ?? Info.Name;
    }

    /// <summary>
    /// 화면에 표시될 설명을 가져옵니다.
    /// 다국어 설명이 없을 경우 기본 설명을 가져옵니다.
    /// Get the description to be displayed on the screen.
    /// Get the default description if there is no localized description.
    /// example returns:
    ///   ko-KR: "실행 가능한 앱을 지정하기 위한 데이터소스입니다."
    ///   en-US: "DataSource for specifying executable apps."
    /// </summary>
    /// <returns><see cref="string"/>Description "DataSource for specifying executable apps."</returns>
    public string GetDisplayDescription()
    {
        ArgumentNullException.ThrowIfNull(Info);

        string? description = null;
        if (Info.LocalizedDescriptions != null)
        {
            var localizationService = services.GetRequiredService<ILocalizationService>();
            var language = localizationService.GetAppLanguage();
            if (Info.LocalizedDescriptions.TryGetValue(language, out var localizedDescription))
            {
                description = localizedDescription;
            }
        }
        if (string.IsNullOrEmpty(description) && !string.IsNullOrEmpty(Info.Description))
        {
            description = Info.Description;
        }
        return description ?? Info.Description;
    }
}
