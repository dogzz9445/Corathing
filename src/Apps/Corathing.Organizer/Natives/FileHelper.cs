using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
using System.Drawing;
using Corathing.Organizer.Utils;

namespace Corathing.Organizer.Natives;

public static class FileHelper
{
    public static ImageSource GetIcon(string fileName)
    {
        Icon icon = Icon.ExtractAssociatedIcon(fileName);
        return System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                    icon.Handle,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
    }

    public static string GenerateUniqueFolder(Guid guid)
    {
        return ZBase32Encoder.ToString(guid.ToByteArray());
    }
}
