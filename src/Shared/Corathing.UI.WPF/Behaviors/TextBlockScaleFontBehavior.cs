using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using Corathing.UI.WPF.Helpers;

using Microsoft.Xaml.Behaviors;

namespace Corathing.UI.WPF.Behaviors;

public class TextBlockScaleFontBehavior : Behavior<Grid>
{
    // MaxFontSize
    public double MaxFontSize { get { return (double)GetValue(MaxFontSizeProperty); } set { SetValue(MaxFontSizeProperty, value); } }
    public static readonly DependencyProperty MaxFontSizeProperty = DependencyProperty.Register("MaxFontSize", typeof(double), typeof(ScaleFontBehavior), new PropertyMetadata(48d));

    protected override void OnAttached()
    {
        AssociatedObject.SizeChanged += (s, e) => { CalculateFontSize(); };
    }

    private void CalculateFontSize()
    {
        double fontSize = MaxFontSize;

        // get grid height (if limited)
        // get column width (if limited)
        Grid parentGrid = AssociatedObject;

        List<TextBlock> tbs = VisualHelper.FindVisualChildren<TextBlock>(this.AssociatedObject);
        foreach (var tb in tbs)
        {
            double gridHeight = double.MaxValue;
            double gridWidth = double.MaxValue;
            if (parentGrid.RowDefinitions.Count > 0)
            {
                RowDefinition row = parentGrid.RowDefinitions[Grid.GetRow(tb)];
                gridHeight = row.Height == GridLength.Auto ? double.MaxValue : AssociatedObject.ActualHeight;
            }
            else
            {
                gridHeight = AssociatedObject.ActualHeight;
            }
            if (parentGrid.ColumnDefinitions.Count > 0)
            {
                ColumnDefinition col = parentGrid.ColumnDefinitions[Grid.GetColumn(tb)];
                gridWidth = col.Width == GridLength.Auto ? double.MaxValue : AssociatedObject.ActualWidth;
            }
            else
            {
                gridWidth = AssociatedObject.ActualWidth;
            }

            // get desired size with fontsize = MaxFontSize
            Size desiredSize = MeasureText(tb);
            double widthMargins = tb.Margin.Left + tb.Margin.Right;
            double heightMargins = tb.Margin.Top + tb.Margin.Bottom;

            double desiredHeight = desiredSize.Height + heightMargins;
            double desiredWidth = desiredSize.Width + widthMargins;

            // adjust fontsize if text would be clipped vertically
            if (gridHeight < desiredHeight)
            {
                double factor = (desiredHeight - heightMargins) / (gridHeight - heightMargins);
                fontSize = Math.Min(fontSize, MaxFontSize / factor);
            }

            // adjust fontsize if text would be clipped horizontally
            if (gridWidth < desiredWidth)
            {
                double factor = (desiredWidth - widthMargins) / (gridWidth - widthMargins);
                fontSize = Math.Min(fontSize, MaxFontSize / factor);
            }

            if (fontSize <= 0)
                fontSize = 1;
            tb.FontSize = fontSize;
        }
    }

    /// <summary>
    /// Measures size of <see cref="SymbolIcon"/> desired from <see cref="MaxFontSize"/>
    /// </summary>
    /// <param name="symbol"><see cref="SymbolIcon"/>symbol</param>
    /// <returns><see cref="Size"/> of <see cref="SymbolIcon"/></returns>
    private Size MeasureText(TextBlock tb)
    {
        var formattedText = new FormattedText(
            tb.Text,
            CultureInfo.CurrentUICulture,
            FlowDirection.LeftToRight,
            new Typeface(tb.FontFamily, tb.FontStyle, tb.FontWeight, tb.FontStretch),
            MaxFontSize,
            Brushes.Black,
            VisualTreeHelper.GetDpi(AssociatedObject).PixelsPerDip); // always uses MaxFontSize for desiredSize

        return new Size(formattedText.Width, formattedText.Height);
    }
}
