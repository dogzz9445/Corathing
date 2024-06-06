using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Corathing.Contracts.Services;

namespace Corathing.Organizer.ViewModels;

public partial class OrganizerSettingsViewModel : ObservableRecipient
{
    private readonly IThemeService _themeService;

    private bool _isInitialized = false;

    [ObservableProperty]
    private string _appVersion = string.Empty;

    [ObservableProperty]
    private ApplicationTheme _currentApplicationTheme = ApplicationTheme.Unknown;

    public OrganizerSettingsViewModel(IThemeService themeService)
    {
        _themeService = themeService;

        InitializeViewModel();
    }

    public void OnNavigatedTo()
    {
        if (!_isInitialized)
        {
            InitializeViewModel();
        }
    }

    public void OnNavigatedFrom() { }

    partial void OnCurrentApplicationThemeChanged(ApplicationTheme oldValue, ApplicationTheme newValue)
    {
        _themeService.Apply(newValue);
    }

    private void InitializeViewModel()
    {
        CurrentApplicationTheme = _themeService.GetAppTheme();
        AppVersion = $"{GetAssemblyVersion()}";

        _isInitialized = true;
    }

    private void OnThemeChanged(ApplicationTheme currentApplicationTheme, Color systemAccent)
    {
        // Update the theme if it has been changed elsewhere than in the settings.
        if (CurrentApplicationTheme != currentApplicationTheme)
        {
            CurrentApplicationTheme = currentApplicationTheme;
        }
    }

    [RelayCommand]
    public void Close(Window window)
    {
        window.Close();
    }

    private static string GetAssemblyVersion()
    {
        return Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? string.Empty;
    }
}
