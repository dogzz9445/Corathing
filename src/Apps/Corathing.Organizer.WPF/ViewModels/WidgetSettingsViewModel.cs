using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Corathing.Contracts.Bases;
using Corathing.Contracts.DataContexts;
using Corathing.Contracts.Services;
using Corathing.Contracts.Utils.Exetensions;
using Corathing.Dashboards.WPF.Controls;

using Microsoft.Extensions.DependencyInjection;

namespace Corathing.Organizer.WPF.ViewModels;

public partial class WidgetSettingsViewModel : ObservableObject
{
    #region Constructor with IServiceProvider
    private readonly IServiceProvider _services;

    public WidgetSettingsViewModel(IServiceProvider services)
    {
        _services = services;
    }
    #endregion

    // Original properties
    private WidgetHost _originalWidget;
    private WidgetContext _originalContext;

    private WidgetHost _settingsWidgetHost;
    [ObservableProperty]
    private WidgetContext _tempWidgetContext;
    private Type _tempCustomSettingsStateType;
    [ObservableProperty]
    private IWidgetCustomSettingsContext? _tempCustomSettingsContext;

    public Type RegisterWidget(WidgetHost settingsWidgetHost, WidgetHost originalWidgetHost)
    {
        _originalWidget = originalWidgetHost;
        _settingsWidgetHost = settingsWidgetHost;

        _originalContext = _originalWidget.DataContext as WidgetContext;

        var packageService = _services.GetService<IPackageService>();

        TempWidgetContext = packageService.CreateWidgetContext(_originalContext.GetType().FullName);
        _tempCustomSettingsStateType = packageService.GetCustomSettingsType(_originalContext.GetType().FullName);

        // If Custom Settings exists
        // Custom Settings 가 존재할 경우
        if (_tempCustomSettingsStateType != null)
        {
            TempCustomSettingsContext = packageService.CreateWidgetSettingsContext(_originalContext.GetType().FullName);
            if (TempCustomSettingsContext is INotifyPropertyChanged notifier)
            {
                notifier.PropertyChanged += OnCustomSettingsChanged;
            }
            _originalContext.CopyTo(TempWidgetContext, _tempCustomSettingsStateType);
            WidgetStateExtension.CopyProperties(
                _originalContext.State.CustomSettings,
                TempCustomSettingsContext.CustomSettings,
                _tempCustomSettingsStateType
                );
            TempCustomSettingsContext.UpdateSettings();
        }

        _settingsWidgetHost.DataContext = TempWidgetContext;
        return _originalContext.GetType();
    }

    [RelayCommand]
    public void Apply()
    {
        if (_originalContext == null)
            return;

        TempWidgetContext.CopyToWithoutLayout(_originalContext, _tempCustomSettingsStateType);
        _originalContext.UpdateTo(_originalContext.State);

        var appStateService = _services.GetService<IAppStateService>();
        appStateService.UpdateWidget(_originalContext.State);
    }

    [RelayCommand]
    public void GoBack(Window window)
    {
        window.Close();
    }

    public void OnCustomSettingsChanged(object sender, PropertyChangedEventArgs e)
    {
        if (_originalContext == null)
            return;

        WidgetStateExtension.CopyProperties(
            TempCustomSettingsContext.CustomSettings,
            TempWidgetContext.State.CustomSettings,
            _tempCustomSettingsStateType);
    }
}
