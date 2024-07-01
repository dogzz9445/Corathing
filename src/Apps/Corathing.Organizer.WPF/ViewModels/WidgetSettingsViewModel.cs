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
    private IServiceProvider _services;

    private WidgetHost _originalWidget;
    private WidgetHost _settingsWidgetHost;
    private WidgetContext _originalContext;

    [ObservableProperty]
    private WidgetContext _tempWidgetContext;

    private Type _tempCustomSettingsStateType;

    [ObservableProperty]
    private IWidgetCustomSettingsContext? _tempCustomSettingsContext;

    public WidgetSettingsViewModel(IServiceProvider services)
    {
        _services = services;
    }

    public Type RegisterWidget(WidgetHost settingsWidgetHost, WidgetHost originalWidgetHost)
    {
        _originalWidget = originalWidgetHost;
        _settingsWidgetHost = settingsWidgetHost;

        _originalContext = _originalWidget.DataContext as WidgetContext;

        var packageService = _services.GetService<IPackageService>();

        _tempCustomSettingsStateType = packageService.GetCustomSettingsType(_originalContext.GetType().FullName);
        TempWidgetContext = packageService.CreateWidgetContext(_originalContext.GetType().FullName);
        TempCustomSettingsContext = packageService.CreateWidgetSettingsContext(_originalContext.GetType().FullName);
        if (TempCustomSettingsContext is INotifyPropertyChanged notifier)
        {
            notifier.PropertyChanged += OnCustomSettingsChanged;
        }
        _settingsWidgetHost.DataContext = TempWidgetContext;
        _originalContext.CopyTo(TempWidgetContext, _tempCustomSettingsStateType);
        WidgetStateExtension.CopyProperties(
            _originalContext.State.CustomSettings,
            TempCustomSettingsContext.CustomSettings,
            _tempCustomSettingsStateType
            );
        TempCustomSettingsContext.UpdateSettings();

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
