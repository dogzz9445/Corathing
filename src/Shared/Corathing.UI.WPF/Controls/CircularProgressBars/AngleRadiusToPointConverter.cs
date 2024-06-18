using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Corathing.UI.WPF.Controls.CircularProgressBars;

public class AngleRadiusToPointConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length != 3)
            return new();

        if (values[0] is double angle &&
            values[1] is double radius &&
            values[2] is Thickness offset)
        {
            double piang = angle * Math.PI / 180;

            double px = Math.Sin(piang) * radius + radius;
            double py = -Math.Cos(piang) * radius + radius;

            return new Point(px + offset.Left, py + offset.Top);
        }

        return new();
    }

    public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
