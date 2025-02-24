using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Corathing.Organizer.WPF.Converters;

public class PlaceholderDataTemplateConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == parameter)
        {
            return CollectionView.NewItemPlaceholder;
        }

        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == CollectionView.NewItemPlaceholder)
        {
            return parameter;
        }

        return value;
    }
}
