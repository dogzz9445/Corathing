using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

using Corathing.Contracts.Bases;
using Corathing.Contracts.DataContexts;
using Corathing.Contracts.Entries;
using Corathing.Contracts.Services;

using Microsoft.Extensions.DependencyInjection;

namespace Corathing.Contracts.Utils.Generators;

public class CoraWidgetGenerator(IServiceProvider services)
{
    public ICoraWidgetInfo Info { get; set; }

    /// <summary>
    /// Creates the widget.
    /// </summary>
    /// <returns>WidgetBase</returns>
    public WidgetContext CreateWidget(WidgetState? state = null)
    {
        ArgumentNullException.ThrowIfNull(Info.WidgetContextType);

        if (state == null)
        {
            state = CreateState();
        }
        var context = (WidgetContext)Activator.CreateInstance(Info.WidgetContextType, services, state);
        context.Layout = new WidgetLayout()
        {
            Id = Guid.NewGuid(),
            WidgetStateId = context.WidgetId,
            Rect = new WidgetLayoutRect()
            {
                X = 0,
                Y = 0,
                W = state.CoreSettings.ColumnSpan,
                H = state.CoreSettings.RowSpan,
            }
        };
        return context;
    }

    /// <summary>
    /// Creates the state.
    /// </summary>
    /// <returns>WidgetState</returns>
    public WidgetState CreateState()
    {
        ArgumentNullException.ThrowIfNull(Info.WidgetContextType);

        WidgetState state = new WidgetState();
        state.Id = Guid.NewGuid();
        
        state.PackageReference = services
            .GetRequiredService<IPackageService>()
            .GetPackageReferenceState(GetAssemblyName());

        state.CoreSettings = new WidgetCoreState()
        {
            TypeName = GetWidgetFullName(),

            RowIndex = 0,
            ColumnIndex = 0,
            RowSpan = Info.DefaultRowSpan,
            ColumnSpan = Info.DefaultColumnSpan,

            Title = GenerateDefaultTitle(),
            VisibleTitle = Info.DefaultVisibleTitle,

            UseDefaultBackgroundColor = true,
            BackgroundColor = "#FFFFFF",
        };

        state.CustomSettings = CreateCustomOption();

        return state;
    }

    public object? CreateCustomOption()
    {
        if (Info.WidgetCustomSettingsType == null)
            return null;
        return Activator.CreateInstance(Info.WidgetCustomSettingsType);
    }

    public IWidgetCustomSettingsContext? CreateSettingsContext()
    {
        if (Info.WidgetCustomSettingsType == null)
            return null;
        if (Info.WidgetCustomSettingsContextType == null)
            return null;
        var customSettings = CreateCustomOption();
        return Activator.CreateInstance(Info.WidgetCustomSettingsContextType, customSettings) as IWidgetCustomSettingsContext;
    }

    /// <summary>
    /// 네임스페이스를 포함한 위젯 컨텍스트의 전체 이름을 가져옵니다.
    /// Get the full name of the widget context including the namespace.
    /// example:
    ///   "Corathing.Widgets.Basics.Widgets.FileOpeners.FileOpenerWidgetContext"
    /// </summary>
    /// <returns><see cref="string"/>Widget Context Full Name</returns>
    public string? GetWidgetFullName()
    {
        ArgumentNullException.ThrowIfNull(Info.WidgetContextType);

        return Info.WidgetContextType.FullName;
    }

    /// <summary>
    /// 위젯 컨텍스트의 어셈블리 이름을 가져옵니다.
    /// example:
    ///   "Corathing.Widgets.Basics"
    /// </summary>
    /// <returns><see cref="string"/>Widget context assembly name</returns>
    public string? GetAssemblyName()
    {
        ArgumentNullException.ThrowIfNull(Info.WidgetContextType);

        return Info.WidgetContextType.Assembly.GetName().Name;
    }

    /// <summary>
    /// 위젯의 기본 제목으로 지정될 이름을 생성합니다.
    /// 다국어 제목이 없을 경우 기본 제목을 생성합니다.
    /// Get the title to be set on the widget title.
    /// Get the default title if there is no localized title.
    /// example returns:
    ///   ko-KR: "파일 열기"
    ///   en-US: "File Opener"
    /// </summary>
    /// <returns><see cref="string"/>Default Title "File Opener"</returns>
    private string GenerateDefaultTitle()
    {
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
        return defaultTitle ?? Info.WidgetContextType.Name;
    }

    /// <summary>
    /// 화면에 표시될 이름을 가져옵니다.
    /// 다국어 이름이 없을 경우 기본 이름을 가져옵니다.
    /// Get the name to be displayed on the screen.
    /// Get the default name if there is no localized name.
    /// example returns:
    ///   ko-KR: "파일 위젯"
    ///   en-US: "File Widget"
    /// </summary>
    /// <returns><see cref="string"/>Name "File Widget"</returns>
    public string GetDisplayName()
    {
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
    ///   ko-KR: "파일 및 폴더 열기가 가능한 위젯입니다."
    ///   en-US: "Widget that can open files and folders."
    /// </summary>
    /// <returns><see cref="string"/>Description "Widget that can open files and folders"</returns>
    public string GetDisplayDescription()
    {
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

    /// <summary>
    /// 메뉴에 표시될 경로를 가져옵니다.
    /// 다국어 경로가 없을 경우 기본 경로를 가져옵니다.
    /// Get the path to be displayed in the menu.
    /// Get the default path if there is no localized path.
    /// example returns:
    ///   ko-KR: "기본/파일 위젯"
    ///   en-US: "Default/File Widget"
    /// </summary>
    /// <returns><see cref="string"/>Meun Path "Default/File Widget"</returns>
    public string GetDisplayMenuPath()
    {
        string? meunPath = null;
        if (Info.LocalizedMenuPaths != null)
        {
            var localizationService = services.GetRequiredService<ILocalizationService>();
            var language = localizationService.GetAppLanguage();
            if (Info.LocalizedMenuPaths.TryGetValue(language, out var localizedMenuPath))
            {
                meunPath = localizedMenuPath;
            }
        }
        if (string.IsNullOrEmpty(meunPath) && !string.IsNullOrEmpty(Info.MenuPath))
        {
            meunPath = Info.MenuPath;
        }
        return meunPath ?? Info.MenuPath;
    }

    /// <summary>
    /// 메뉴에 표시될 툴팁을 가져옵니다.
    /// 로컬라이제이션 된 툴팁이 없을 경우 기본 툴팁을 가져옵니다.
    /// Get the tooltip to be displayed in the menu.
    /// Get the default tooltip if there is no localized tooltip.
    /// example returns:
    ///   ko-KR: "대시보드에 파일 위젯 생성"
    ///   en-US: "Generate File Widget on Dashboard"
    /// </summary>
    /// <returns><see cref="string"/>Menu Tooltip "Generate File Widget on Dashboard"</returns>
    public string GetDisplayMenuTooltip()
    {
        string? menuTooltip = null;
        if (Info.LocalizedMenuTooltips != null)
        {
            var localizationService = services.GetRequiredService<ILocalizationService>();
            var language = localizationService.GetAppLanguage();
            if (Info.LocalizedMenuTooltips.TryGetValue(language, out var localizedMenuTooltip))
            {
                menuTooltip = localizedMenuTooltip;
            }
        }
        if (string.IsNullOrEmpty(menuTooltip) && !string.IsNullOrEmpty(Info.MenuTooltip))
        {
            menuTooltip = Info.MenuTooltip;
        }
        return menuTooltip ?? Info.MenuTooltip;
    }
}
