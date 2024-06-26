using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Corathing.Dashboards.WPF.Converters;

[ValueConversion(typeof(string), typeof(SolidColorBrush))]
public class ColorNameStringToBrushConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not string colorName)
            return value;

        return new BrushConverter().ConvertFromString(colorName) as SolidColorBrush;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not Color color)
            return value;

        return color.ToString();
    }
}
