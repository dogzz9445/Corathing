using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Corathing.Organizer.UnitTests.Factories;

public static class WindowFactory
{
    public static Window Create() =>
        new MainWindow();
}
