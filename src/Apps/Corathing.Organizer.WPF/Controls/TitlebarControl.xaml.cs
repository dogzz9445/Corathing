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

using CommunityToolkit.Mvvm.Input;

using UserControl = System.Windows.Controls.UserControl;

namespace Corathing.Organizer.WPF.Controls;

/// <summary>
/// Interaction logic for Titlebar.xaml
/// </summary>
public partial class TitlebarControl : UserControl
{
    #region CloseWindowCommand
    public RelayCommand? CloseWindowCommand
    {
        get { return (RelayCommand)GetValue(CloseWindowCommandProperty); }
        set { SetValue(CloseWindowCommandProperty, value); }
    }
    public static readonly DependencyProperty CloseWindowCommandProperty =
        DependencyProperty.Register("CloseWindowCommand",
            typeof(RelayCommand),
            typeof(TitlebarControl),
            new FrameworkPropertyMetadata(null));
    #endregion

    #region MaximizeWindowCommand
    public RelayCommand? MaximizeWindowCommand
    {
        get { return (RelayCommand)GetValue(MaximizeWindowCommandProperty); }
        set { SetValue(MaximizeWindowCommandProperty, value); }
    }
    public static readonly DependencyProperty MaximizeWindowCommandProperty =
        DependencyProperty.Register(
            "MaximizeWindowCommand",
            typeof(RelayCommand),
            typeof(TitlebarControl),
            new FrameworkPropertyMetadata(null));

    public bool VisibleMaximizeButton
    {
        get { return (bool)GetValue(VisibleMaximizeButtonProperty); }
        set { SetValue(VisibleMaximizeButtonProperty, value); }
    }
    public static readonly DependencyProperty VisibleMaximizeButtonProperty =
        DependencyProperty.Register(
            "VisibleMaximizeButton",
            typeof(bool),
            typeof(TitlebarControl),
            new FrameworkPropertyMetadata(true));
    #endregion

    #region MinimizeWindowCommand
    public RelayCommand? MinimizeWindowCommand
    {
        get { return (RelayCommand)GetValue(MinimizeWindowCommandProperty); }
        set { SetValue(MinimizeWindowCommandProperty, value); }
    }
    public static readonly DependencyProperty MinimizeWindowCommandProperty =
        DependencyProperty.Register(
            "MinimizeWindowCommandCommand",
            typeof(RelayCommand),
            typeof(TitlebarControl),
            new FrameworkPropertyMetadata(null));

    public bool VisibleMinimizeButton
    {
        get { return (bool)GetValue(VisibleMinimizeButtonProperty); }
        set { SetValue(VisibleMinimizeButtonProperty, value); }
    }
    public static readonly DependencyProperty VisibleMinimizeButtonProperty =
        DependencyProperty.Register(
            "VisibleMinimizeButton",
            typeof(bool),
            typeof(TitlebarControl),
            new FrameworkPropertyMetadata(true));
    #endregion

    #region OpenSettingsCommand
    public RelayCommand<Window?>? OpenSettingsCommand
    {
        get { return (RelayCommand<Window?>?)GetValue(OpenSettingsCommandProperty); }
        set { SetValue(OpenSettingsCommandProperty, value); }
    }
    public static readonly DependencyProperty OpenSettingsCommandProperty =
        DependencyProperty.Register(
            "OpenSettingsCommand",
            typeof(RelayCommand<Window?>),
            typeof(TitlebarControl),
            new FrameworkPropertyMetadata(null));
    public bool VisibleSettingsButton
    {
        get { return (bool)GetValue(VisibleSettingsButtonProperty); }
        set { SetValue(VisibleSettingsButtonProperty, value); }
    }
    public static readonly DependencyProperty VisibleSettingsButtonProperty =
        DependencyProperty.Register(
            "VisibleSettingsButton",
            typeof(bool),
            typeof(TitlebarControl),
            new FrameworkPropertyMetadata(true));
    #endregion

    #region Title
    public string Title
    {
        get { return (string)GetValue(TitleProperty); }
        set { SetValue(TitleProperty, value); }
    }
    public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register("Title", typeof(string), typeof(TitlebarControl), new UIPropertyMetadata());
    #endregion

    #region AuthProfile
    public bool VisibleAuthProfile
    {
        get { return (bool)GetValue(VisibleAuthProfileProperty); }
        set { SetValue(VisibleAuthProfileProperty, value); }
    }
    public static readonly DependencyProperty VisibleAuthProfileProperty =
        DependencyProperty.Register(
            "VisibleAuthProfile",
            typeof(bool),
            typeof(TitlebarControl),
            new FrameworkPropertyMetadata(true));
    #endregion

    #region DateTime
    public bool VisibleDateTime
    {
        get { return (bool)GetValue(VisibleDateTimeProperty); }
        set { SetValue(VisibleDateTimeProperty, value); }
    }
    public static readonly DependencyProperty VisibleDateTimeProperty =
        DependencyProperty.Register(
            "VisibleDateTime",
            typeof(bool),
            typeof(TitlebarControl),
            new FrameworkPropertyMetadata(false));

    #endregion

    public TitlebarControl()
    {
        InitializeComponent();

        CloseWindowCommand ??= new RelayCommand(CloseWindow);
        MinimizeWindowCommand ??= new RelayCommand(MinimizeWindow);
        MaximizeWindowCommand ??= new RelayCommand(MaximizedWindow);
    }

    private void MinimizeWindow()
    {
        var window = Window.GetWindow(this);
        if (window == null)
        {
            return;
        }
        window.WindowState = WindowState.Minimized;
    }

    private void MaximizedWindow()
    {
        var window = Window.GetWindow(this);
        if (window == null)
        {
            return;
        }
        window.WindowState = window.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
    }

    /// <summary>
    /// 프로그램을 종료
    /// </summary>
    private void CloseWindow()
    {
        var window = Window.GetWindow(this);
        if (window == null)
        {
            return;
        }
        window.Close();
    }

    private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        var window = Window.GetWindow(this);
        if (window == null)
        {
            return;
        }
        window.DragMove();
    }
}
