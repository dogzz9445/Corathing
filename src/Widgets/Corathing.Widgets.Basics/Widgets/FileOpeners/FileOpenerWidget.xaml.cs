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

[WidgetContextEntry(
    name: "Create File Opener",
    description: "Provides a one by one square widget.",
    menuPath: "Default/File Opener",
    menuOrder: 0,
    targetType: typeof(FileOpenerWidgetViewModel)
    )]
public partial class FileOpenerWidgetViewModel : WidgetContext
{
    [ObservableProperty]
    private string? _filePath;

    [ObservableProperty]
    private string? _fileType;

    [ObservableProperty]
    private string? _fileDescription;

    [ObservableProperty]
    private string? _fileSize;

    [ObservableProperty]
    private string? _fileLastModified;

    [ObservableProperty]
    private string? _fileCreated;

    [ObservableProperty]
    private string? _fileAccessed;

    [ObservableProperty]
    private string? _fileAttributes;

    [ObservableProperty]
    private string? _fileOwner;

    [ObservableProperty]
    private string? _fileGroup;

    [ObservableProperty]
    private string? _filePermissions;

    [ObservableProperty]
    private string? _fileContent;

    [ObservableProperty]
    private string? _fileContentLength;

    [ObservableProperty]
    private string? _fileContentEncoding;

    [ObservableProperty]
    private string? _fileContentHash;

    [ObservableProperty]
    private string? _fileContentHashAlgorithm;

    [ObservableProperty]
    private string? _fileContentHashLength;

    [ObservableProperty]
    private string? _fileContentHashEncoding;


    /// <summary>
    /// Initializes a new instance of the <see cref="OneByOneViewModel"/> class.
    /// </summary>
    public FileOpenerWidgetViewModel(IServiceProvider services) : base(services)
    {
        WidgetTitle = $"FileOpener";
        VisibleTitle = false;
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
