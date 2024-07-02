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

using Corathing.Widgets.Basics.Widgets.FileOpeners;

using Microsoft.Win32;

using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace Corathing.Widgets.Basics.DataSources.ExecutableApps;

public partial class ExecutableAppDataSourceOptionViewModel : ObservableObject
{
    [RelayCommand]
    public void OpenFile(object? file)
    {
        if (file is not Widgets.FileOpeners.FileInfo fileInfo)
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
}
