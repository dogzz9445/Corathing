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

namespace Corathing.Organizer.ViewModels;

public partial class WidgetSettingsViewModel : ObservableObject
{
    private IServiceProvider _services;

    private WidgetHost _originalWidget;
    private WidgetHost _settingsWidgetHost;
    private WidgetContext _originalContext;

    [ObservableProperty]
    private WidgetContext _tempWidgetContext;

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
        packageService.TryGetWidgetGenerator(_originalContext.GetType().FullName, out var generator);

        TempWidgetContext = generator.CreateWidget();
        TempCustomSettingsContext = generator.CreateCustomSettingsContext();
        if (TempCustomSettingsContext is INotifyPropertyChanged notifier)
        {
            notifier.PropertyChanged += OnCustomSettingsChanged;
        }
        _settingsWidgetHost.DataContext = TempWidgetContext;
        _originalContext.CopyTo(TempWidgetContext, generator.CustomSettingsType);
        WidgetStateExtension.CopyProperties(
            _originalContext.State.CustomSettings,
            TempCustomSettingsContext.CustomSettings,
            generator.CustomSettingsType
            );
        TempCustomSettingsContext.UpdateSettings();

        return generator.ContextType;
    }

    [RelayCommand]
    public void Apply()
    {
        if (_originalContext == null)
            return;

        var packageService = _services.GetService<IPackageService>();
        packageService.TryGetWidgetGenerator(_originalContext.GetType().FullName, out var generator);

        TempWidgetContext.CopyToWithoutLayout(_originalContext, generator.CustomSettingsType);
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

        var packageService = _services.GetService<IPackageService>();
        packageService.TryGetWidgetGenerator(_originalContext.GetType().FullName, out var generator);

        WidgetStateExtension.CopyProperties(
            TempCustomSettingsContext.CustomSettings,
            TempWidgetContext.State.CustomSettings,
            generator.CustomSettingsType);
    }
}
