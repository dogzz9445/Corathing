﻿using System;
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
using Corathing.Organizer.WPF.Services;

using Microsoft.Extensions.DependencyInjection;

namespace Corathing.Organizer.WPF.ViewModels;

public partial class OrganizerSettingsViewModel : ObservableRecipient
{
    #region Readonly Properties in Initializing
    private readonly IServiceProvider _services;
    private readonly IThemeService _themeService;
    private readonly ILocalizationService _localizationService;
    private readonly IAppStateService _appStateService;

    private bool _isInitialized = false;
    #endregion

    #region 11. 설정파일

    [ObservableProperty]
    private bool? _useGlobalConfiguration;

    [ObservableProperty]
    private bool? _useAppPathConfiguration;

    [ObservableProperty]
    private string _customPath;

    #endregion

    #region 12. 게스트 모드
    [ObservableProperty]
    private bool? _useGuestMode = false;

    [ObservableProperty]
    private bool? _showSettingsButtonOnDashboard = true;

    [ObservableProperty]
    private string _username;

    [ObservableProperty]
    private string _password;
    #endregion

    [ObservableProperty]
    private string _appVersion = string.Empty;

    [ObservableProperty]
    private ApplicationTheme _currentApplicationTheme = ApplicationTheme.Unknown;

    [ObservableProperty]
    private ApplicationLanguage _currentApplicationLanguage = ApplicationLanguage.Unknown;

    public OrganizerSettingsViewModel(
        IServiceProvider services,
        IThemeService themeService,
        ILocalizationService localizationService,
        IAppStateService appStateService)
    {
        _services = services;
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
    public void Goback()
    {
        INavigationDialogService navigationDialogService
            = _services.GetRequiredService<INavigationDialogService>();
        navigationDialogService.GoBack();
    }

    private static string GetAssemblyVersion()
    {
        return Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? string.Empty;
    }
}
