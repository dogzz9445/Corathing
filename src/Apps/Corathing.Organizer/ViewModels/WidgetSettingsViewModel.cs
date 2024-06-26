using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Corathing.Contracts.Bases;
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

        // TODO:
        // 24-06-16:
        // 1. Create a new instance of the widget context
        // 2. Copy the properties of the original widget context to the new widget context
        // 3. Set the new widget context as the data context of the widget host
        // 4. Set the new widget context as the temporary widget context
        // 5. Set the original widget context as the original widget context


        TempWidgetContext = generator.CreateWidget();
        _settingsWidgetHost.DataContext = TempWidgetContext;

        _originalContext.CopyTo(TempWidgetContext);

        // TODO:
        //TempWidgetContext.State.CustomSettings =
        //    JsonHelper.DeepCopy(_originalContext.State.CustomSettings, generator.OptionType);
        //TempWidgetContext.State = JsonHelper.DeepCopy<WidgetState>(_tempWidgetContext.State);
        //TempWidgetContext.State = _originalContext.State.CustomSettings;

        return generator.ContextType;
    }

    [RelayCommand]
    public void Apply()
    {
        TempWidgetContext.CopyToWithoutLayout(_originalContext);
        _originalContext.UpdateTo(_originalContext.State);

        var appStateService = _services.GetService<IAppStateService>();
        appStateService.UpdateWidget(_originalContext.State);
    }

    [RelayCommand]
    public void GoBack(Window window)
    {
        window.Close();
    }
}
