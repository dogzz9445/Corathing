using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Corathing.Contracts.Attributes;
using Corathing.Contracts.Bases;
using Corathing.Contracts.Services;
using Corathing.Widgets.Basics.DataSources.ExecutableApps;
using Microsoft.Extensions.DependencyInjection;

namespace Corathing.Widgets.Basics.Widgets.FileOpeners;

[EntryCoraWidget(
    contextType: typeof(FileOpenerWidgetContext),
    customSettingsType: typeof(FileOpenerOption),
    customSettingsContextType: typeof(FileOpenerOptionViewModel),
    name: "Create File Opener",
    description: "Provides a one by one square widget.",
    menuPath: "Default/File Opener",
    menuOrder: 0
    )]
[EntryCoraWidgetName(ApplicationLanguage.en_US, "File Opener")]
[EntryCoraWidgetName(ApplicationLanguage.ko_KR, "파일 열기")]
[EntryCoraWidgetDescription(ApplicationLanguage.en_US, "Provide file and folder opening")]
[EntryCoraWidgetDescription(ApplicationLanguage.ko_KR, "파일 및 폴더 열기 기능 제공")]
[EntryCoraWidgetDefaultTitle(ApplicationLanguage.en_US, "File Opener")]
[EntryCoraWidgetDefaultTitle(ApplicationLanguage.ko_KR, "파일 열기")]
public partial class FileOpenerWidgetContext : WidgetContext
{
    [ObservableProperty]
    private ObservableCollection<string>? _filePaths;

    [ObservableProperty]
    private ObservableCollection<string>? _fileType;

    [ObservableProperty]
    private ExecutableAppDataSourceContext _executableAppDataSourceContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="OneByOneViewModel"/> class.
    /// </summary>
    public FileOpenerWidgetContext(
        IServiceProvider services, WidgetState state)
        : base(services, state)
    {
        ILocalizationService localizationService = services.GetService<ILocalizationService>();
        localizationService.Provide("Corathing.Widgets.Basics.FileOpenerName", value =>
        {
            DefaultTitle = value;
            if (string.IsNullOrEmpty(state.CoreSettings.Title))
            {
                WidgetTitle = DefaultTitle;
            }
        });
    }

    public override void OnStateChanged(WidgetState state)
    {
        if (state.CustomSettings is not FileOpenerOption customSettings)
        {
            return;
        }
        //FilePath = customSettings.Files;
        //FileType = customSettings.FileType;
        if (customSettings.ExecutableAppDataSourceId != Guid.Empty)
        {
            var dataSourceService = _services.GetRequiredService<IDataSourceService>();
            ExecutableAppDataSourceContext = dataSourceService.GetDataSourceContext<ExecutableAppDataSourceContext>(customSettings.ExecutableAppDataSourceId);
        }
    }

    [RelayCommand]
    public void Execute()
    {

    }

    [RelayCommand]
    public void ExecuteDefaultApp()
    {

    }
}
