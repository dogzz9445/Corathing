using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Corathing.UI.WPF.Converters;

/// <inheritdoc />
/// <summary>
/// Converter for if bool value provided is true then set Visibility.Collapsed else Visibility.Visible
/// </summary>
/// <seealso cref="System.Windows.Data.IValueConverter" />
[ValueConversion(typeof(bool), typeof(Visibility))]
public class BoolNotNullToVisibilityConverter : IValueConverter
{
    /// <inheritdoc />
    /// <summary>
    /// Converts a value.
    /// </summary>
    /// <param name="value">The value produced by the binding source.</param>
    /// <param name="targetType">The type of the binding target property.</param>
    /// <param name="parameter">The converter parameter to use.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>
    /// A converted value. If the method returns <see langword="Visibility.Collapsed" />, the valid null value is used.
    /// </returns>
    /// <exception cref="System.Exception">InvertBoolToVisibilityConverter</exception>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null)
            return Visibility.Collapsed;

        if (!(value is bool boolValue))
            return Visibility.Collapsed;

        return boolValue ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
