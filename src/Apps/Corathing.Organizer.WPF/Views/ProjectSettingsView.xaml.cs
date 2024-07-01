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

using Corathing.Organizer.WPF.Extensions;
using Corathing.Organizer.WPF.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace Corathing.Organizer.WPF.Views
{
    /// <summary>
    /// ProjectSettingsView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ProjectSettingsView : Page
    {
        public ProjectSettingsViewModel? ViewModel;

        public ProjectSettingsView()
        {
            InitializeComponent();

            DataContext = ViewModel = App.Current.Services.GetService<ProjectSettingsViewModel>();

            Loaded += (s, e) =>
            {
                var window = Window.GetWindow(this);
                window.Width = 800;
                window.Height = 800;
                window.CenterWindowToParent();
            };
        }
    }
}
