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

using Corathing.Contracts.Attributes;
using Corathing.Contracts.Bases;

namespace Corathing.Dashboards.WPF.Widgets
{
    public class EmptyWidgetOption
    {

    }

    public partial class EmptyWidgetState : WidgetState<EmptyWidgetOption>
    {

    }

    [WidgetContextEntry(
        name: "Create Empty",
        description: "Empty Widget.",
        menuPath: "Empty",
        menuOrder: 0,
        targetType: typeof(EmptyWidgetContext)
        )]
    public partial class EmptyWidgetContext : WidgetContext
    {
        [ObservableProperty]
        private EmptyWidgetState _emptyWidgetState;

        public EmptyWidgetContext(IServiceProvider services) : base(services)
        {
        }
    }

    /// <summary>
    /// Interaction logic for EmptyWidget.xaml
    /// </summary>
    public partial class EmptyWidget
    {
        public EmptyWidget()
        {
            InitializeComponent();
        }
    }
}
