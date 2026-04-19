using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

using Serilog;
using Serilog.Core;

namespace Corathing.Dashboards.WPF.Converters;

[ValueConversion(typeof(string), typeof(SolidColorBrush))]
public class ColorNameStringToBrushConverter : IValueConverter
{
    // TODO: define default Brush as DependencyProperty
    private readonly SolidColorBrush _colorBrush = Brushes.Black;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not string colorName)
            return value;

        SolidColorBrush brush = _colorBrush;
        try
        {
            if (new BrushConverter().ConvertFromString(colorName) is SolidColorBrush parsedBrush)
            {
                brush = parsedBrush;
            }
        }
        catch (NotSupportedException ex)
        {
            Log.Error(ex, "Error to parse {ColorName} as ColorBrush", colorName);
        }

        return _colorBrush;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not Color color)
            return value;

        return color.ToString();
    }
}
