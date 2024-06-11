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

using Corathing.Contracts.Bases;
using Corathing.Contracts.Services;
using Corathing.Dashboards.WPF.Services;
using Corathing.Organizer.Services;

namespace Corathing.Organizer.ViewModels;

public partial class OrganizerSettingsViewModel : ObservableRecipient
{
    #region Readonly Properties in Initializing
    private readonly IThemeService _themeService;
    private readonly ILocalizationService _localizationService;
    private readonly IAppStateService _appStateService;

    private bool _isInitialized = false;
    #endregion

    [ObservableProperty]
    private string _appVersion = string.Empty;

    [ObservableProperty]
    private ApplicationTheme _currentApplicationTheme = ApplicationTheme.Unknown;

    [ObservableProperty]
    private ApplicationLanguage _currentApplicationLanguage = ApplicationLanguage.Unknown;

    [ObservableProperty]
    private bool? _useGlobalConfiguration;

    [ObservableProperty]
    private bool? _useAppPathConfiguration;

    [ObservableProperty]
    private string _customPath;

    public OrganizerSettingsViewModel(
        IThemeService themeService,
        ILocalizationService localizationService,
        IAppStateService appStateService)
    {
        _themeService = themeService;
        _localizationService = localizationService;
        _appStateService = appStateService;

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

    partial void OnCurrentApplicationLanguageChanged(ApplicationLanguage oldValue, ApplicationLanguage newValue)
    {
        _localizationService.Apply(newValue);
    }

    async partial void OnUseGlobalConfigurationChanged(bool? oldValue, bool? newValue)
    {
        var appSettings = _appStateService.GetAppSettings();
        if (appSettings.UseGlobalConfiguration == newValue)
            return;
        appSettings.UseGlobalConfiguration = newValue;
        _appStateService.UpdateAppSettings(appSettings);
    }

    async partial void OnUseAppPathConfigurationChanged(bool? oldValue, bool? newValue)
    {
        var appSettings = _appStateService.GetAppSettings();
        if (appSettings.UseAppPathConfiguration == newValue)
            return;
        appSettings.UseAppPathConfiguration = newValue;
        _appStateService.UpdateAppSettings(appSettings);
    }

    async partial void OnCustomPathChanged(string? oldValue, string newValue)
    {
        var appSettings = _appStateService.GetAppSettings();
        // FIXME:
        // / \ 차이와 Lower Trim 등을 적용하여야함
        if (string.Compare(appSettings.CustomConfigurationFilename, newValue) == 0)
            return;
        appSettings.CustomConfigurationFilename = newValue;
        _appStateService.UpdateAppSettings(appSettings);
    }


    private void InitializeViewModel()
    {
        CurrentApplicationTheme = _themeService.GetAppTheme();
        CurrentApplicationLanguage = _localizationService.GetAppLanguage();
        AppVersion = $"{GetAssemblyVersion()}";

        var appSettings = _appStateService.GetAppSettings();
        UseGlobalConfiguration = appSettings.UseGlobalConfiguration;
        UseAppPathConfiguration = appSettings.UseAppPathConfiguration;
        CustomPath = appSettings.CustomConfigurationFilename;

        _isInitialized = true;
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
