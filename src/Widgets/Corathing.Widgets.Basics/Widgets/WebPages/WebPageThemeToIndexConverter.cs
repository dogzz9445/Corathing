using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;


namespace Corathing.Widgets.Basics.Widgets.WebPages;

[ValueConversion(typeof(WebPageTheme), typeof(int))]
public class WebPageThemeToIndexConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is WebPageTheme.App)
        {
            return 0;
        }

        if (value is WebPageTheme.Light)
        {
            return 1;
        }

        if (value is WebPageTheme.Dark)
        {
            return 2;
        }

        return 0;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is 0)
        {
            return WebPageTheme.App;
        }

        if (value is 1)
        {
            return WebPageTheme.Light;
        }

        if (value is 2)
        {
            return WebPageTheme.Dark;
        }

        return WebPageTheme.App;
    }
}
