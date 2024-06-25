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

using Corathing.Organizer.ViewModels;

using Microsoft.Extensions.DependencyInjection;

using Wpf.Ui;

using UserControl = System.Windows.Forms.UserControl;

namespace Corathing.Organizer.Views;

/// <summary>
/// Interaction logic for NavigationDialogView.xaml
/// </summary>
public partial class NavigationDialogView
{
    public NavigationDialogViewModel ViewModel { get; set; }

    public NavigationDialogView(
        INavigationService navigationService,
        NavigationDialogViewModel viewModel
        )
    {
        InitializeComponent();

        ViewModel = viewModel;
        navigationService.SetNavigationControl(NavigationView);

        NavigationView.Navigated += NavigationView_Navigated;
        NavigationView.Navigating += NavigationView_Navigating;
    }

    private void NavigationView_Navigating(Wpf.Ui.Controls.NavigationView sender, Wpf.Ui.Controls.NavigatingCancelEventArgs args)
    {
    }

    private void NavigationView_Navigated(Wpf.Ui.Controls.NavigationView sender, Wpf.Ui.Controls.NavigatedEventArgs args)
    {
    }

    private void OnNavigationSelectionChanged(object sender, RoutedEventArgs e)
    {
        if (sender is not Wpf.Ui.Controls.NavigationView navigationView)
        {
            return;
        }

        //NavigationView.SetCurrentValue(
        //    NavigationView.HeaderVisibilityProperty,
        //    navigationView.SelectedItem?.TargetPageType != typeof(DashboardPage)
        //        ? Visibility.Visible
        //        : Visibility.Collapsed
        //);
    }

    private void OnNavigating(Wpf.Ui.Controls.NavigationView sender, Wpf.Ui.Controls.NavigatingCancelEventArgs args)
    {
    }
}
