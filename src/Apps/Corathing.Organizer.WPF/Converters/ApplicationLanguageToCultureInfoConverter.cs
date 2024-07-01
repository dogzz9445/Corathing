using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

using Corathing.Contracts.Services;
using Corathing.Dashboards.WPF.Bindings;

namespace Corathing.Organizer.WPF.Converters;

[ValueConversion(typeof(ApplicationLanguage), typeof(CultureInfo))]
public class ApplicationLanguageToCultureInfoConverter : IValueConverter
{
    public object? Convert(object value, Type targetType,
        object parameter, CultureInfo culture)
        => value is ApplicationLanguage language ? language switch
        {
            ApplicationLanguage.en_US => 0,
            ApplicationLanguage.ko_KR => 1,
            _ => 0,
        } : 0;

    public object? ConvertBack(object value, Type targetType,
        object parameter, CultureInfo culture)
        => value is int index ? index switch
        {
            0 => ApplicationLanguage.en_US,
            1 => ApplicationLanguage.ko_KR,
            _ => ApplicationLanguage.en_US,
        } : ApplicationLanguage.en_US;

    //public object? Convert(object value, Type targetType,
    //    object parameter, CultureInfo culture)
    //{
    //    if (value == null)
    //        return null;
    //    if (value is not AvailableCulture)
    //        return null;
    //    return Convert((AvailableCulture)value);
    //}

    //public object? ConvertBack(object value, Type targetType,
    //    object parameter, CultureInfo culture)
    //{
    //    if (value == null)
    //        return null;
    //    if (value is not CultureInfo)
    //        return null;
    //    return ConvertBack((CultureInfo)value);
    //}

    //public static CultureInfo Convert(AvailableCulture culture)
    //{
    //    return CultureInfo.GetCultureInfo(
    //        StringToAvailableCultureConverter.
    //        ConvertBack(culture));
    //}

    //public static AvailableCulture ConvertBack(CultureInfo culture)
    //{
    //    return StringToAvailableCultureConverter.Convert(culture.Name);
    //}
}
