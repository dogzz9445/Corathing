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

using Corathing.Contracts.DataContexts;
using Corathing.Dashboards.WPF.Controls;
using Corathing.Organizer.WPF.Extensions;
using Corathing.Organizer.WPF.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Corathing.Organizer.WPF.Views;

/// <summary>
/// Interaction logic for DataSourceContextSettingsView.xaml
/// </summary>
public partial class DataSourceSettingsView : Page
{
    public DataSourceSettingsViewModel ViewModel;

    public DataSourceSettingsView(DataSourceContext originalContext)
    {
        DataContext = ViewModel = App.Current.Services.GetRequiredService<DataSourceSettingsViewModel>();

        InitializeComponent();

        Type contextType = originalContext.GetType();
        ViewModel.Initialize(contextType);

        Loaded += (s, e) =>
        {
            var window = Window.GetWindow(this);
            window.Width = 800;
            window.Height = 800;
            window.CenterWindowToParent();
        };
    }
}
