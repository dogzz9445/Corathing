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

namespace Corathing.Widgets.Basics.Widgets.Commanders
{
    [WidgetContextEntry(
        name: "Create Commander",
        description: "Provides a one by one square widget.",
        menuPath: "Default/Commander",
        menuOrder: 0,
        targetType: typeof(CommanderWidgetViewModel)
        )]
    public partial class CommanderWidgetViewModel : WidgetContext
    {
        private string? _filePath;

        /// <summary>
        /// Initializes a new instance of the <see cref="OneByOneViewModel"/> class.
        /// </summary>
        public CommanderWidgetViewModel(IServiceProvider serivces) : base(serivces)
        {
            WidgetTitle = $"Commander";
        }
    }

    /// <summary>
    /// CommanderWidget.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CommanderWidget
    {
        public CommanderWidget()
        {
            InitializeComponent();
        }
    }
}
