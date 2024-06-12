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

namespace Corathing.Dashboards.Sample.Widgets
{
    public partial class EmptyWidgetContext : WidgetContext
    {
        public EmptyWidgetContext()
        {
            WidgetTitle = "Empty Widget";
        }
    }

    /// <summary>
    /// Interaction logic for WidgetFromContext.xaml
    /// </summary>
    public partial class EmptyWidget
    {
        public EmptyWidget()
        {
            InitializeComponent();
        }
    }
}
