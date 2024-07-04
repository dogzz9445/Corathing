using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

using Corathing.Dashboards.WPF.Services;

namespace Corathing.Dashboards.WPF.Converters;

[ValueConversion(typeof(string), typeof(string))]
public class StringKeyToLocalizationStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        if (value is not string localizationKey)
        {
            return null;
        }

        return LocalizationService.Instance.GetString(localizationKey);
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
