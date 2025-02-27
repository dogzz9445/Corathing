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

using Corathing.WPF.Sample.ViewModels;

namespace Corathing.WPF.Sample;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly MainWindowViewModel _viewModel;
    //private readonly IPopupService _popupService;

    public MainWindow()
    {
        InitializeComponent();

        // ViewModel 생성 및 연결
        _viewModel = new MainWindowViewModel();
        this.DataContext = _viewModel;
    }
}
