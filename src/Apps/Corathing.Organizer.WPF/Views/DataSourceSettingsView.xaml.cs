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

using Corathing.Contracts.Services;
using Corathing.Contracts.DataContexts;
using Corathing.Dashboards.WPF.Controls;
using Corathing.Organizer.WPF.Extensions;
using Corathing.Organizer.WPF.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Corathing.Organizer.WPF.Views;

/// <summary>
/// Interaction logic for DataSourceContextSettingsView.xaml
/// </summary>
public partial class DataSourceSettingsView : Page, INavigationView
{
    public DataSourceSettingsViewModel ViewModel;

    public DataSourceSettingsView()
    {
        InitializeComponent();

        DataContext = ViewModel = App.Current.Services.GetRequiredService<DataSourceSettingsViewModel>();

    }
    public void OnPreviewGoback(object? parameter = null)
    {
    }

    public void OnBack(object? parameter = null)
    {
    }

    public void OnForward(object? parameter = null)
    {
        if (parameter is DataSourceContext originalContext)
        {
            Type contextType = originalContext.GetType();
            ViewModel.Initialize(contextType);
        }
    }
}
