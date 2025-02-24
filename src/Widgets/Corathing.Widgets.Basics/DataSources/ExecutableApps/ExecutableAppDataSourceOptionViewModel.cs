using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Corathing.Contracts.DataContexts;

using Microsoft.Win32;

using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace Corathing.Widgets.Basics.DataSources.ExecutableApps;

public partial class ExecutableAppDataSourceOptionViewModel :
    CustomSettingsContext
{
    protected override void OnCreate(object? defaultOption)
    {
    }

    protected override void OnContextChanged()
    {
        if (CustomSettings is not ExecutableAppDataSourceOption customSettings)
        {
            throw new ArgumentException($"Not a valid type for CustomSettings {nameof(ExecutableAppDataSourceOption)}");
        }
        customSettings.ExecutableAppPath = ExecutableAppPath;
        customSettings.CommandLineArguments = CommandLineArguments;
        CustomSettings = customSettings;
    }

    protected override void OnSettingsChanged(object? option)
    {
        if (option is not ExecutableAppDataSourceOption customSettings)
        {
            throw new ArgumentException($"Not a valid type for CustomSettings {nameof(ExecutableAppDataSourceOption)}");
        }
        ExecutableAppPath = customSettings.ExecutableAppPath;
        CommandLineArguments = customSettings.CommandLineArguments;
    }

    [ObservableProperty]
    private string? _executableAppPath;

    [ObservableProperty]
    private string? _commandLineArguments;

    [RelayCommand]
    public void OpenFile()
    {
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

        ExecutableAppPath = openFileDialog.FileName;
    }
}
