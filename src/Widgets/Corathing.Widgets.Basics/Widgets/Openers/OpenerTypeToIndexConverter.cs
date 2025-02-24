using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Corathing.Widgets.Basics.Widgets.Openers;

[ValueConversion(typeof(OpenerType), typeof(int))]
public sealed class OpenerTypeToIndexConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is OpenerType openerType)
        {
            return openerType switch
            {
                OpenerType.Files => 0,
                OpenerType.Folders => 1,
                OpenerType.Links => 2,
                _ => 0,
            };
        }

        return 0;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int openerIndex)
        {
            return openerIndex switch
            {
                0 => OpenerType.Files,
                1 => OpenerType.Folders,
                2 => OpenerType.Links,
                _ => OpenerType.Files,
            };
        }

        return OpenerType.Files;
    }
}
