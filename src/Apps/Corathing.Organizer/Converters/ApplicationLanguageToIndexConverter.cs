using System;
using System.Globalization;
using System.Windows.Data;

using Corathing.Contracts.Services;

namespace Corathing.Organizer.Converters;

[ValueConversion(typeof(ApplicationLanguage), typeof(int))]
public class ApplicationLanguageToIndexConverter : IValueConverter
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
}
