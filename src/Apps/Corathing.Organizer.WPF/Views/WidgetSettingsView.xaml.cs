using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Corathing.Contracts.Bases;
using Corathing.Contracts.Services;
using Corathing.Dashboards.Controls;
using Corathing.Dashboards.WPF.Controls;
using Corathing.Organizer.WPF.Extensions;
using Corathing.Organizer.WPF.ViewModels;

using Microsoft.Extensions.DependencyInjection;

namespace Corathing.Organizer.WPF.Views;

/// <summary>
/// WidgetSettingsView.xaml에 대한 상호 작용 논리
/// </summary>
public partial class WidgetSettingsView : Page, INavigationView
{
    public WidgetSettingsViewModel ViewModel { get; set; }

    // Dependency Object 를 사용하게 변경핡서
    // DataContext에 대해서

    public WidgetSettingsView()
    {
        InitializeComponent();

        DataContext = ViewModel = App.Current.Services.GetRequiredService<WidgetSettingsViewModel>();

    }
    public void OnPreviewGoback(object? parameter = null)
    {
    }

    public void OnBack(object? parameter = null)
    {
    }

    public void OnForward(object? parameter = null)
    {
        if (parameter is not WidgetHost widgetHost)
        {
            App.Current.Services.GetService<IDialogService>()
                .ShowAlertDanger("Failed to load widget settings.");
            return;
        }
        // FIXME:
        // This is a temporary solution to get the context type of the widget.
        // This will be replaced with a more elegant solution in the future.
        WidgetHost tempWidgetHost = new WidgetHost();

        if (widgetHost.DataContext is WidgetContext widgetContext)
        {
            Type contextType = widgetContext.GetType();
            if (contextType != null)
            {
                ViewModel?.Initialize(widgetContext, tempWidgetHost);
                var dataTemplateKey = new DataTemplateKey(contextType);
                var dataTemplate = FindResource(dataTemplateKey) as DataTemplate;
                if (dataTemplate != null)
                {
                    tempWidgetHost.ContentTemplate = dataTemplate;
                }
            }
            else
            {
                App.Current.Services.GetService<IDialogService>()
                    .ShowAlertDanger("Failed to load widget settings.");
            }
        }
        else
        {
            App.Current.Services.GetService<IDialogService>()
                .ShowAlertDanger("Failed to load widget settings.");
        }

        WidgetHostContentPresenter.Content = tempWidgetHost;
    }
}
