using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

using Corathing.Contracts.Services;

namespace Corathing.Organizer.Converters;

public class UILanguageToIndexConverter : IValueConverter
{
    public object? Convert(object value, Type targetType,
        object parameter, CultureInfo culture)
        => value is string languageName ? languageName switch
        {
            "English" => 0,
            "한국어" => 1,
            _ => 0,
        } : 0;

    public object? ConvertBack(object value, Type targetType,
        object parameter, CultureInfo culture)
        => value is int index ? index switch
        {
            0 => "English",
            1 => "한국어",
            _ => "English",
        } : "English";
}
