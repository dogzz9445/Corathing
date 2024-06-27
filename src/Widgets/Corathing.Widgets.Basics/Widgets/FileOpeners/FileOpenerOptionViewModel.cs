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

using Corathing.Contracts.Entries;

using Microsoft.Win32;

using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace Corathing.Widgets.Basics.Widgets.FileOpeners;

public partial class FileOpenerOptionViewModel : ObservableObject, IWidgetCustomSettingsContext
{
    private FileOpenerOption _customSettings;
    public object? CustomSettings
    {
        get => GetCustomSettings();
        set
        {
            if (value is not FileOpenerOption customSettings)
            {
                // TODO:
                // Change Type
                throw new Exception();
            }
            _customSettings = customSettings;
            UpdateSettings();
        }
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

    public FileOpenerOptionViewModel(FileOpenerOption fileOpenerOption)
    {
        _customSettings = fileOpenerOption;

        Files = new ObservableCollection<FileInfo>();
        Folders = new ObservableCollection<FolderInfo>();

        UpdateSettings();
    }

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
    public void RemoveFile(object file)
    {
        Files.Remove(file is FileInfo fileInfo ? SelectedFile : null);
    }

    [RelayCommand]
    public void RemoveFolder(FolderInfo folder)
    {
        Folders.Remove(folder ?? SelectedFolder);
    }

    [RelayCommand]
    public void OpenFile(FileInfo file)
    {
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

        file.FileName = openFileDialog.FileName;
        //OpenedFilePathVisibility = Visibility.Visible;
    }

    [RelayCommand]
    public void OpenFolder(FolderInfo folder)
    {
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

        folder.FolderName = string.Join("\n", openFolderDialog.FolderNames);
        //OpenedFolderPathVisibility = Visibility.Visible;
    }

    public FileOpenerOption GetCustomSettings()
    {
        _customSettings.OpenType = OpenType;
        _customSettings.Folders = Folders.Select(item => item.FolderName).ToList();
        _customSettings.Files = Files.Select(item => item.FileName).ToList();

        return _customSettings;
    }

    public void UpdateSettings()
    {
        OpenType = _customSettings.OpenType;
        _customSettings.Files?.ForEach(file => Files.Add(new FileInfo()
        {
            FileName = file,
        }));
        _customSettings.Folders?.ForEach(folder => Folders.Add(new FolderInfo()
        {
            FolderName = folder,
        }));
    }
}
