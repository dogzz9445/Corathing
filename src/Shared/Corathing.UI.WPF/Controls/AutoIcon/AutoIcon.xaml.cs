using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace Corathing.UI.WPF.Controls.AutoIcon;

/// <summary>
/// Interaction logic for AutoIcon.xaml
/// </summary>
public partial class AutoIcon : UserControl
{
    #region Public Properties

    public static readonly DependencyProperty MaxSizeProperty = DependencyProperty.Register(
        nameof(MaxSize),
        typeof(double),
        typeof(AutoIcon),
        new PropertyMetadata(null));

    public double MaxSize
    {
        get => (double)GetValue(MaxSizeProperty);
        set => SetValue(MaxSizeProperty, value);
    }

    public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
        nameof(Icon),
        typeof(string),
        typeof(AutoIcon),
        new PropertyMetadata(null));

    public string Icon
    {
        get => (string)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }


    #endregion

    static AutoIcon()
    {

    }

    public AutoIcon()
    {
        InitializeComponent();
    }


    #region Private Methods
    /// <summary>
    /// Get DPI of the screen from Visual
    /// </summary>
    /// <returns></returns>
    private double GetDpi()
    {
        var dpi = VisualTreeHelper.GetDpi(this);
        return Math.Min(dpi.PixelsPerInchX, dpi.PixelsPerInchY);
    }

    public double ConvertFontSizeToPixels(double fontSize)
    {
        return fontSize * GetDpi() / 72;
    }

    public double ConvertPixelsToFontSize(double pixels)
    {
        return pixels * 72 / GetDpi();
    }

    #endregion
}
