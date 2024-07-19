using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Corathing.Contracts.DataContexts;
using Corathing.UI.ObjectModel;
using Corathing.Widgets.Basics.DataSources.ExecutableApps;

using Microsoft.Win32;

using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace Corathing.Widgets.Basics.Widgets.Openers;

public partial class FileInfo : ObservableObject
{
    [ObservableProperty]
    private string _fileName;
}

public partial class FolderInfo : ObservableObject
{
    [ObservableProperty]
    private string _folderName;
}

public partial class LinkInfo : ObservableObject
{
    [ObservableProperty]
    private string _linkName;
}

public partial class OpenerOptionContext :
    CustomSettingsContext
{
    protected override void OnCreate(object? defaultOption)
    {
        if (defaultOption is not OpenerOption openerOption)
        {
            throw new ArgumentException($"Not a valid type for CustomSettings {nameof(OpenerOption)}");
        }
        Files = new ObservableItemCollection<FileInfo>();
        Folders = new ObservableItemCollection<FolderInfo>();
        Links = new ObservableItemCollection<LinkInfo>();
        CustomSettings = openerOption;
        ExecutableAppDataSourceSelector = new ExecutableAppDataSourceSelector(Services);

        Files.ItemPropertyChanged += (s, e) => OnPropertyChanged(nameof(Files));
        Folders.ItemPropertyChanged += (s, e) => OnPropertyChanged(nameof(Folders));
        Links.ItemPropertyChanged += (s, e) => OnPropertyChanged(nameof(Links));
        ExecutableAppDataSourceSelector.PropertyChanged += (sender, args) =>
        {
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(ExecutableAppDataSourceSelector)));
        };
    }

    protected override void OnContextChanged()
    {
        if (CustomSettings is not OpenerOption openerOption)
        {
            throw new ArgumentException($"Not a valid type for CustomSettings {nameof(OpenerOption)}");
        }
        openerOption.OpenerType = OpenerType;
        openerOption.Files = Files?.Select(file => file.FileName).ToList();
        openerOption.Folders = Folders?.Select(folder => folder.FolderName).ToList();
        openerOption.Links = Links?.Select(folder => folder.LinkName).ToList();
        if (ExecutableAppDataSourceSelector != null &&
            ExecutableAppDataSourceSelector.SelectedDataSourceContext != null)
        {
            openerOption.ExecutableAppDataSourceId =
                ExecutableAppDataSourceSelector.SelectedDataSourceContext.DataSourceId;
        }
        CustomSettings = openerOption;
    }

    protected override void OnSettingsChanged(object? option)
    {
        if (option is not OpenerOption openerOption)
        {
            throw new ArgumentException($"Not a valid type for CustomSettings {nameof(OpenerOption)}");
        }
        Files?.Clear();
        Folders?.Clear();
        Links?.Clear();

        OpenerType = openerOption.OpenerType;
        openerOption.Files?.ForEach(file => Files?.Add(new FileInfo()
        {
            FileName = file,
        }));
        openerOption.Folders?.ForEach(folder => Folders?.Add(new FolderInfo()
        {
            FolderName = folder,
        }));
        openerOption.Links?.ForEach(link => Links?.Add(new LinkInfo()
        {
            LinkName = link,
        }));
        ExecutableAppDataSourceSelector?.Select(openerOption.ExecutableAppDataSourceId);
    }

    [ObservableProperty]
    private OpenerType _openerType;
    [ObservableProperty]
    private ObservableItemCollection<FileInfo>? _files;
    [ObservableProperty]
    private ObservableItemCollection<FolderInfo>? _folders;
    [ObservableProperty]
    private ObservableItemCollection<LinkInfo>? _links;

    [ObservableProperty]
    private ExecutableAppDataSourceSelector? _executableAppDataSourceSelector;

    [RelayCommand]
    public void AddFile()
    {
        Files?.Add(new FileInfo());
    }

    [RelayCommand]
    public void AddFolder()
    {
        Folders?.Add(new FolderInfo());
    }

    [RelayCommand]
    public void AddLink()
    {
        Links?.Add(new LinkInfo());
    }

    [RelayCommand]
    public void RemoveFile(object? file)
    {
        if (file is not FileInfo fileInfo)
        {
            throw new ArgumentException($"Not a valid type for CustomSettings {nameof(FileInfo)}");
        }
        Files?.Remove(fileInfo);
    }

    [RelayCommand]
    public void RemoveFolder(object? folder)
    {
        if (folder is not FolderInfo folderInfo)
        {
            throw new ArgumentException($"Not a valid type for CustomSettings {nameof(FolderInfo)}");
        }
        Folders?.Remove(folderInfo);
    }

    [RelayCommand]
    public void RemoveLink(object? link)
    {
        if (link is not LinkInfo linkInfo)
        {
            throw new ArgumentException($"Not a valid type for CustomSettings {nameof(LinkInfo)}");
        }
        Links?.Remove(linkInfo);
    }

    [RelayCommand]
    public void OpenFile(object? file)
    {
        if (file is not FileInfo fileInfo)
        {
            throw new ArgumentException($"Not a valid type for CustomSettings {nameof(FileInfo)}");
        }

        OpenFileDialog openFileDialog =
            new()
            {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                Filter = "All files (*.*)|*.*"
            };

        if (openFileDialog.ShowDialog() != true)
        {
            return;
        }

        if (!File.Exists(openFileDialog.FileName))
        {
            return;
        }

        fileInfo.FileName = openFileDialog.FileName;
        OnPropertyChanged(new PropertyChangedEventArgs(nameof(fileInfo.FileName)));
    }

    [RelayCommand]
    public void OpenFolder(object? folder)
    {
        if (folder is not FolderInfo folderInfo)
        {
            throw new ArgumentException($"Not a valid type for CustomSettings {nameof(FolderInfo)}");
        }

        OpenFolderDialog openFolderDialog = new()
        {
            Multiselect = false,
            InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
        };

        if (openFolderDialog.ShowDialog() != true)
        {
            return;
        }

        if (openFolderDialog.FolderNames.Length == 0)
        {
            return;
        }

        folderInfo.FolderName = string.Join("\n", openFolderDialog.FolderNames);
        OnPropertyChanged(new PropertyChangedEventArgs(nameof(folderInfo.FolderName)));
    }
}
