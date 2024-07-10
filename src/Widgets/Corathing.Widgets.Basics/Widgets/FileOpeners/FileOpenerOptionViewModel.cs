using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Corathing.Contracts.DataContexts;
using Corathing.UI.WPF.Structures;
using Corathing.Widgets.Basics.DataSources.ExecutableApps;

using Microsoft.Win32;

using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace Corathing.Widgets.Basics.Widgets.FileOpeners;

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

public partial class FileOpenerOptionViewModel :
    CustomSettingsContext
{
    protected override void OnCreate(object? defaultOption)
    {
        if (defaultOption is not FileOpenerOption fileOpenerOption)
        {
            throw new ArgumentException($"Not a valid type for CustomSettings {nameof(FileOpenerOption)}");
        }
        Files = new ObservableCollection<FileInfo>();
        Folders = new ObservableCollection<FolderInfo>();
        CustomSettings = fileOpenerOption;
        ExecutableAppDataSourceSelector = new ExecutableAppDataSourceSelector(Services);
        //ExecutableAppDataSourceSelector.PropertyChanged += (sender, args) =>
        //{
        //    OnPropertyChanged(new PropertyChangedEventArgs(nameof(ExecutableAppDataSourceSelector)));
        //};
    }

    protected override void OnContextChanged()
    {
        if (CustomSettings is not FileOpenerOption fileOpenerOption)
        {
            throw new ArgumentException($"Not a valid type for CustomSettings {nameof(FileOpenerOption)}");
        }
        fileOpenerOption.OpenType = OpenType;
        fileOpenerOption.Files = Files.Select(file => file.FileName).ToList();
        fileOpenerOption.Folders = Folders.Select(folder => folder.FolderName).ToList();
        fileOpenerOption.ExecutableAppDataSourceId = ExecutableAppDataSourceSelector.SelectedDataSourceContext.DataSourceId;
        //ExecutableAppDataSourceSelector.Select(fileOpenerOption.ExecutableAppDataSourceId);
        CustomSettings = fileOpenerOption;
    }

    protected override void OnSettingsChanged(object? option)
    {
        if (option is not FileOpenerOption fileOpenerOption)
        {
            throw new ArgumentException($"Not a valid type for CustomSettings {nameof(FileOpenerOption)}");
        }
        Files.Clear();
        Folders.Clear();

        OpenType = fileOpenerOption.OpenType;
        fileOpenerOption.Files?.ForEach(file => Files.Add(new FileInfo()
        {
            FileName = file,
        }));
        fileOpenerOption.Folders?.ForEach(folder => Folders.Add(new FolderInfo()
        {
            FolderName = folder,
        }));
    }

    [ObservableProperty]
    private FileOpenType _openType;
    [ObservableProperty]
    private ObservableCollection<FileInfo> _files;
    [ObservableProperty]
    private ObservableCollection<FolderInfo> _folders;
    [ObservableProperty]
    private FileInfo _selectedFile;
    [ObservableProperty]
    private FolderInfo _selectedFolder;

    [ObservableProperty]
    private ExecutableAppDataSourceSelector _executableAppDataSourceSelector;

    [RelayCommand]
    public void AddFile()
    {
        Files.Add(new FileInfo());
    }

    [RelayCommand]
    public void AddFolder()
    {
        Folders.Add(new FolderInfo());
    }

    [RelayCommand]
    public void RemoveFile(object? file)
    {
        if (file is not FileInfo fileInfo)
        {
            // TODO:
            // Change Exception Type
            throw new Exception();
        }
        Files.Remove(fileInfo);
    }

    [RelayCommand]
    public void RemoveFolder(object? folder)
    {
        if (folder is not FolderInfo folderInfo)
        {
            // TODO:
            // Change Exception Type
            throw new Exception();
        }
        Folders.Remove(folderInfo);
    }

    [RelayCommand]
    public void OpenFile(object? file)
    {
        if (file is not FileInfo fileInfo)
        {
            // TODO:
            // Change Exception Type
            throw new Exception();
        }

        //OpenedFilePathVisibility = Visibility.Collapsed;

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
        //OpenedFilePathVisibility = Visibility.Visible;
    }

    [RelayCommand]
    public void OpenFolder(object? folder)
    {
        if (folder is not FolderInfo folderInfo)
        {
            // TODO:
            // Change Exception Type
            throw new Exception();
        }
        //OpenedFolderPathVisibility = Visibility.Collapsed;

        OpenFolderDialog openFolderDialog =
            new()
            {
                Multiselect = true,
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
        //OpenedFolderPathVisibility = Visibility.Visible;
    }

}
