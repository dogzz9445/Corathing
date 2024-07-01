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

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Wpf.Ui.Controls;

namespace Corathing.Organizer.WPF.Controls;

public partial class MultiLevelNavigationViewModel(Wpf.Ui.INavigationService navigationService)
{
    [RelayCommand]
    private void NavigateForward(Type type)
    {
        _ = navigationService.NavigateWithHierarchy(type);
    }

    [RelayCommand]
    private void NavigateBack()
    {
        _ = navigationService.GoBack();
    }
}

/// <summary>
/// Interaction logic for MultiLevelNavigationPage.xaml
/// </summary>
public partial class MultiLevelNavigationPage : INavigableView<MultiLevelNavigationViewModel>
{
    public MultiLevelNavigationViewModel ViewModel { get; }

    public MultiLevelNavigationPage(MultiLevelNavigationViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = ViewModel;

        InitializeComponent();
    }
}
