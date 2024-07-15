using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Corathing.Widgets.Basics.Widgets.FileOpeners;

/// <summary>
/// One way converter from <see cref="FileOpenType"/> to <see cref="Visibility"/>.
/// </summary>
[ValueConversion(typeof(FileOpenType), typeof(Visibility))]
public class FileOpenTypeFileToVisibleConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is FileOpenType fileOpenType)
        {
            return fileOpenType switch
            {
                FileOpenType.Files => Visibility.Visible,
                FileOpenType.Folders => Visibility.Collapsed,
                _ => Visibility.Collapsed,
            };
        }

        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}

[ValueConversion(typeof(FileOpenType), typeof(Visibility))]
public class FileOpenTypeFolderToVisibleConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is FileOpenType fileOpenType)
        {
            return fileOpenType switch
            {
                FileOpenType.Files => Visibility.Collapsed,
                FileOpenType.Folders => Visibility.Visible,
                _ => Visibility.Collapsed,
            };
        }

        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
