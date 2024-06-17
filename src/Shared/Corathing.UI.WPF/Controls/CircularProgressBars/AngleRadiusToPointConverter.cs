using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Corathing.UI.WPF.Controls.CircularProgressBars;

[ValueConversion(typeof(double[]), typeof(Point))]
public class AngleRadiusToPointConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if (values.Length != 2)
            return new();

        if (values[0] is double angle &&
            values[1] is double radius)
        {
            double piang = angle * Math.PI / 180;

            double px = Math.Sin(piang) * radius + radius;
            double py = -Math.Cos(piang) * radius + radius;

            return new Point(px, py);
        }

        return new();
    }

    public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
