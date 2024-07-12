using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

using Corathing.Widgets.Basics.Widgets.FileOpeners;

namespace Corathing.Widgets.Basics.Widgets.WebPages;

[ValueConversion(typeof(int), typeof(int))]
public class AutoReloadValueToIndexConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not int interval)
        {
            return 0;
        }

        return interval switch
        {
            0 => 0,
            30 => 1,
            60 => 2,
            120 => 3,
            300 => 4,
            600 => 5,
            900 => 6,
            1800 => 7,
            3600 => 8,
            86400 => 9,
            _ => 0
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not int index)
        {
            return 0;
        }

        return index switch
        {
            0 => 0,
            1 => 30,
            2 => 60,
            3 => 120,
            4 => 300,
            5 => 600,
            6 => 900,
            7 => 1800,
            8 => 3600,
            9 => 86400,
            _ => 0
        };
    }
}
