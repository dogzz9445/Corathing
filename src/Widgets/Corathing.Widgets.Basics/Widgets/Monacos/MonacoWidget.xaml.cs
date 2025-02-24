
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
using System.Windows.Threading;
using Microsoft.Web.WebView2.Wpf;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Corathing.Contracts.Bases;
using Corathing.Contracts.Services;
using Microsoft.Extensions.DependencyInjection;
using Corathing.Contracts.Entries;
using Corathing.Contracts.Attributes;

namespace Corathing.Widgets.Basics.Widgets.Monacos;

/// <summary>
/// Interaction logic for MonacoWidget.xaml
/// </summary>
public partial class MonacoWidget
{
    public MonacoWidget()
    {
        InitializeComponent();
    }
}
