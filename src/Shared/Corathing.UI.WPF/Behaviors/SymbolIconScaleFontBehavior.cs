using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

using Microsoft.Xaml.Behaviors;
using Wpf.Ui.Controls;

namespace Corathing.UI.WPF.Behaviors;

public class SymbolIconScaleFontBehavior : Behavior<SymbolIcon>
{
    // MaxFontSize
    public double MaxFontSize
    {
        get => (double)GetValue(MaxFontSizeProperty);
        set => SetValue(MaxFontSizeProperty, value);
    }
    public static readonly DependencyProperty MaxFontSizeProperty =
        DependencyProperty.Register(
            nameof(MaxFontSize),
            typeof(double),
            typeof(SymbolIconScaleFontBehavior),
            new PropertyMetadata(128d));

    private const string MagicChar = "O";

    protected override void OnAttached()
    {
        AssociatedObject.SizeChanged += (s, e) => { CalculateFontSize(); };
    }

    private void CalculateFontSize()
    {
        double fontSize = MaxFontSize;

        var symbolIcon = AssociatedObject;

        Size desiredSize = MeasureText(symbolIcon);
        double widthMargins = symbolIcon.Margin.Left + symbolIcon.Margin.Right;
        double heightMargins = symbolIcon.Margin.Top + symbolIcon.Margin.Bottom;

        double desiredHeight = desiredSize.Height + heightMargins;
        double desiredWidth = desiredSize.Width + widthMargins;

        // adjust fontsize if text would be clipped vertically
        if (AssociatedObject.ActualHeight < desiredHeight)
        {
            double factor = (desiredHeight - heightMargins) / (AssociatedObject.ActualHeight - heightMargins);
            fontSize = Math.Min(fontSize, MaxFontSize / factor);
        }

        // adjust fontsize if text would be clipped horizontally
        if (AssociatedObject.ActualWidth < desiredWidth)
        {
            double factor = (desiredWidth - widthMargins) / (AssociatedObject.ActualWidth - widthMargins);
            fontSize = Math.Min(fontSize, MaxFontSize / factor);
        }

        // apply fontsize (always equal fontsizes)
        AssociatedObject.FontSize = fontSize;
    }

    /// <summary>
    /// Measures size of <see cref="SymbolIcon"/> desired from <see cref="MaxFontSize"/>
    /// </summary>
    /// <param name="symbol"><see cref="SymbolIcon"/>symbol</param>
    /// <returns><see cref="Size"/> of <see cref="SymbolIcon"/></returns>
    private Size MeasureText(SymbolIcon symbol)
    {
        var formattedText = new FormattedText(
            MagicChar,
            CultureInfo.CurrentUICulture,
            FlowDirection.LeftToRight,
            new Typeface(symbol.FontFamily, symbol.FontStyle, symbol.FontWeight, new FontStretch()),
            MaxFontSize, Brushes.Black,
            VisualTreeHelper.GetDpi(AssociatedObject).PixelsPerDip); // always uses MaxFontSize for desiredSize

        return new Size(formattedText.Width, formattedText.Height);
    }
}
