using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Corathing.Contracts.Attributes;
using Corathing.Contracts.Bases;
using Corathing.Contracts.Services;
using Corathing.Widgets.Basics.DataSources.ExecutableApps;
using Microsoft.Extensions.DependencyInjection;
using DataFormats = System.Windows.DataFormats;

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
    private FileOpenType _openType = FileOpenType.Files;

    [ObservableProperty]
    private string? _icon;

    [ObservableProperty]
    private ImageSource? _imageIcon;

    [ObservableProperty]
    private ObservableCollection<string>? _filePaths;

    [ObservableProperty]
    private ObservableCollection<string>? _folderPaths;

    [ObservableProperty]
    private ExecutableAppDataSourceContext? _executableAppDataSourceContext;

    public override void OnCreate(IServiceProvider services, WidgetState state)
    {
        FilePaths = new ObservableCollection<string>();
        FolderPaths = new ObservableCollection<string>();
        Icon = "Document24";

        ILocalizationService localizationService = _services.GetService<ILocalizationService>();
        localizationService.Provide("Corathing.Widgets.Basics.FileOpenerName", value =>
        {
            DefaultTitle = value;
            if (string.IsNullOrEmpty(state.CoreSettings.Title))
            {
                WidgetTitle = DefaultTitle;
            }
        });
    }

    public void OnDrop(System.Windows.DragEventArgs e)
    {
        if (e.Data == null || State == null)
            return;

        if (!e.Data.GetDataPresent(DataFormats.FileDrop))
            return;

        if (e.Data.GetData(DataFormats.FileDrop) is not string[] files || files.Length == 0)
            return;

        var isDirectory = File.GetAttributes(files[0]).HasFlag(FileAttributes.Directory);

        if (State.CustomSettings is not FileOpenerOption option)
            return;

        if (isDirectory)
        {
            option.OpenType = FileOpenType.Folders;
            option.Folders = new List<string>();
            option.Folders.AddRange(files.Where(file =>
                File.GetAttributes(file).HasFlag(FileAttributes.Directory)));
        }
        else
        {
            option.OpenType = FileOpenType.Files;
            option.Files = new List<string>();
            option.Files.AddRange(files.Where(file =>
                !File.GetAttributes(file).HasFlag(FileAttributes.Directory)));
            ImageIcon = GetIcon(files[0]);
        }

        ApplyState(State);
    }

    public override void OnStateChanged(WidgetState state)
    {
        if (state.CustomSettings is not FileOpenerOption option)
        {
            return;
        }
        OpenType = option.OpenType;
        if (OpenType == FileOpenType.Files)
        {
            Icon = "Document24";
        }
        else if (OpenType == FileOpenType.Folders)
        {
            Icon = "Folder24";
        }

        FilePaths.Clear();
        if (option.Files != null)
        {
            foreach (var file in option.Files)
            {
                FilePaths.Add(file);
            }
        }

        if (option.ExecutableAppDataSourceId != null && option.ExecutableAppDataSourceId != Guid.Empty)
        {
            var dataSourceService = _services.GetRequiredService<IDataSourceService>();
            ExecutableAppDataSourceContext = dataSourceService.GetDataSourceContext<ExecutableAppDataSourceContext>(option.ExecutableAppDataSourceId);
        }
    }

    public override void OnDestroy()
    {
        // TODO:
        // Remove localization services
        //ILocalizationService localizationService = _services.GetService<ILocalizationService>();
        //localizationService.Provide("Corathing.Widgets.Basics.FileOpenerName", value =>
        //);
    }

    [RelayCommand]
    public void Execute()
    {
        if (ExecutableAppDataSourceContext != null)
        {
            ExecutableAppDataSourceContext.Execute(FilePaths.ToList());
        }
        else
        {
            if (OpenType == FileOpenType.Files)
            {
                foreach (var file in FilePaths)
                {
                    Process.Start(
                        new ProcessStartInfo()
                        {
                            FileName = file,
                            UseShellExecute = true,
                        }
                    );
                }
            }
            else if (OpenType == FileOpenType.Folders)
            {
                foreach (var folder in FolderPaths)
                {
                    Process.Start(
                        new ProcessStartInfo()
                        {
                            FileName = "explorer.exe",
                            Arguments = folder,
                            UseShellExecute = true,
                        }
                    );
                }
            }
        }
    }

    private static ImageSource? GetIcon(string fileName)
    {
        Icon? icon = System.Drawing.Icon.ExtractAssociatedIcon(fileName);
        if (icon == null)
            return null;
        return System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                    icon.Handle,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
    }
}
