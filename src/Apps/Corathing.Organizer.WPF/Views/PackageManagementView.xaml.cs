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

namespace Corathing.Organizer.WPF.Views;

/// <summary>
/// Interaction logic for PackageManagementView.xaml
/// </summary>
public partial class PackageManagementView : Page, INavigationView
{
    public PackageManagementView()
    {
        InitializeComponent();
    }

    public void OnPreviewGoback(object? parameter = null)
    {
    }

    public void OnBack(object? parameter = null)
    {
    }

    public void OnForward(object? parameter = null)
    {
    }
}
