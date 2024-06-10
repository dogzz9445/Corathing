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
using Corathing.Widgets.Basics.Widgets.Commanders;

namespace Corathing.Widgets.Basics.Widgets.FileOpeners;

public class FileOpenerOption
{
    public string FilePath { get; set; }
}


[WidgetContextEntry(
    name: "Create File Opener",
    description: "Provides a one by one square widget.",
    menuPath: "Default/File Opener",
    menuOrder: 0,
    targetType: typeof(FileOpenerWidgetViewModel),
    optionType: typeof(FileOpenerOption)
    )]
public partial class FileOpenerWidgetViewModel : WidgetContext
{
    [ObservableProperty]
    private string? _filePath;

    [ObservableProperty]
    private string? _fileType;

    /// <summary>
    /// Initializes a new instance of the <see cref="OneByOneViewModel"/> class.
    /// </summary>
    public FileOpenerWidgetViewModel(
        IServiceProvider services)
        : base(services)
    {
        WidgetTitle = $"FileOpener";
        VisibleTitle = true;
    }
}

/// <summary>
/// FileOpenerWidget.xaml에 대한 상호 작용 논리
/// </summary>
public partial class FileOpenerWidget
{
    public FileOpenerWidget()
    {
        InitializeComponent();
    }
}
