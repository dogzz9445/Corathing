using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Corathing.Widgets.Basics.Widgets.FileOpeners;

[ValueConversion(typeof(FileOpenType), typeof(int))]
public sealed class FileOpenTypeToIndexConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is FileOpenType.Files)
        {
            return 0;
        }

        if (value is FileOpenType.Folders)
        {
            return 1;
        }

        return 0;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is 0)
        {
            return FileOpenType.Files;
        }

        if (value is 1)
        {
            return FileOpenType.Folders;
        }

        return FileOpenType.Files;
    }
}
