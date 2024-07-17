using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Corathing.Contracts.Bases;
using Corathing.Contracts.Services;

using Corathing.Widgets.Basics.DataSources.ExecutableApps;

using Corathing.Contracts.Attributes;
using Microsoft.Extensions.DependencyInjection;

using DataFormats = System.Windows.DataFormats;
using System.IO;

namespace Corathing.Widgets.Basics.Widgets.Openers;

[EntryCoraWidget(
    contextType: typeof(OpenerWidgetContext),
    customSettingsType: typeof(OpenerOption),
    customSettingsContextType: typeof(OpenerOptionContext),
    name: "Create Opener",
    description: "Open files or folders or links.",
    menuPath: "Default/Opener",
    menuOrder: 0
    )]
[EntryCoraWidgetName(ApplicationLanguage.en_US, "Opener")]
[EntryCoraWidgetName(ApplicationLanguage.ko_KR, "열기")]
[EntryCoraWidgetDescription(ApplicationLanguage.en_US, "Provide opening files or folders or links")]
[EntryCoraWidgetDescription(ApplicationLanguage.ko_KR, "파일, 폴더, 링크 열기 기능 제공")]
[EntryCoraWidgetDefaultTitle(ApplicationLanguage.en_US, "Opener")]
[EntryCoraWidgetDefaultTitle(ApplicationLanguage.ko_KR, "열기")]
public partial class OpenerWidgetContext : WidgetContext
{
    [ObservableProperty]
    private OpenerType _openerType = OpenerType.Files;

    [ObservableProperty]
    private string? _icon;

    [ObservableProperty]
    private ImageSource? _imageIcon;

    [ObservableProperty]
    private ObservableCollection<string>? _filePaths;

    [ObservableProperty]
    private ObservableCollection<string>? _folderPaths;

    [ObservableProperty]
    private ObservableCollection<string>? _links;

    [ObservableProperty]
    private ExecutableAppDataSourceContext? _executableAppDataSourceContext;

    public override void OnCreate(IServiceProvider services, WidgetState state)
    {
        FilePaths = new ObservableCollection<string>();
        FolderPaths = new ObservableCollection<string>();
        Links = new ObservableCollection<string>();
        Icon = "Document24";
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

        if (State.CustomSettings is not OpenerOption option)
            return;

        if (isDirectory)
        {
            option.OpenerType = OpenerType.Folders;
            option.Folders = new List<string>();
            option.Folders.AddRange(files.Where(file =>
                File.GetAttributes(file).HasFlag(FileAttributes.Directory)));
        }
        else
        {
            option.OpenerType = OpenerType.Files;
            option.Files = new List<string>();
            option.Files.AddRange(files.Where(file =>
                !File.GetAttributes(file).HasFlag(FileAttributes.Directory)));
            ImageIcon = GetIcon(files[0]);
        }

        ApplyState(State);
    }

    public override void OnStateChanged(WidgetState state)
    {
        if (state.CustomSettings is not OpenerOption option)
        {
            return;
        }
        OpenerType = option.OpenerType;
        if (OpenerType == OpenerType.Files)
        {
            Icon = "Document24";
        }
        else if (OpenerType == OpenerType.Folders)
        {
            Icon = "Folder24";
        }
        else if (OpenerType == OpenerType.Links)
        {
            Icon = "Link24";
        }

        if (option.Files != null)
        {
            FilePaths?.Clear();
            foreach (var file in option.Files)
            {
                FilePaths?.Add(file);
            }
        }
        if (option.Folders != null)
        {
            FolderPaths?.Clear();
            foreach (var folder in option.Folders)
            {
                FolderPaths?.Add(folder);
            }
        }
        if (option.Links != null)
        {
            Links?.Clear();
            foreach (var link in option.Links)
            {
                Links?.Add(link);
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
    }

    [RelayCommand]
    public void Execute()
    {
        switch (OpenerType)
        {
            case OpenerType.Files:
                foreach (var file in FilePaths)
                {
                    if (ExecutableAppDataSourceContext != null)
                    {
                        ExecutableAppDataSourceContext.Execute(FilePaths.ToList());
                    }
                    else
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
                break;
            case OpenerType.Folders:
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
                break;
            case OpenerType.Links:
                foreach (var link in Links)
                {
                    Process.Start(
                        new ProcessStartInfo()
                        {
                            FileName = "explorer.exe",
                            Arguments = link,
                            UseShellExecute = true,
                        }
                    );
                }
                break;
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
