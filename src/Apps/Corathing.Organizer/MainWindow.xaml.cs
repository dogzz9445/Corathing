using System.Diagnostics;
using System.Text;
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
using Corathing.Organizer.Views;

using MahApps.Metro.Controls;

using Microsoft.Extensions.DependencyInjection;

using Wpf.Ui;
using Wpf.Ui.Controls;

namespace Corathing.Organizer;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary> 
public partial class MainWindow : MetroWindow
{
    public MainViewModel ViewModel;

    public MainWindow(
        IServiceProvider services,
        IContentDialogService contentDialogService,
        MainViewModel viewModel
        )
    {
        InitializeComponent();

        contentDialogService.SetDialogHost(NavigationContentDialog);

        DataContext = ViewModel = viewModel;

        MouseDown += Window_MouseDown;
        MouseDoubleClick += Window_MouseDoubleClick;
    }


    private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (e.GetPosition(this).Y >= 64)
            return;

        if (e.ChangedButton == MouseButton.Left)
        {
            if (WindowState == WindowState.Normal)
                WindowState = WindowState.Maximized;
            else
                WindowState = WindowState.Normal;
        }
    }

    private void Window_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.GetPosition(this).Y >= 64)
            return;

        if (e.ChangedButton == MouseButton.Left)
            this.DragMove();
    }
}
