using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Corathing.Organizer.Extensions;

public static class WindowExtensions
{
    public static void MaximizeToFirstMonitor(this Window window)
    {
        var primaryScreen = Screen.AllScreens.Where(s => s.Primary).FirstOrDefault();

        if (primaryScreen != null)
        {
            MaximizeWindow(window, primaryScreen);
        }
    }

    public static void MaximizeToSecondMonitor(this Window window)
    {
        var secondScreen = Screen.AllScreens.Where(s => !s.Primary).FirstOrDefault();

        if (secondScreen != null)
        {
            MaximizeWindow(window, secondScreen);
        }
    }

    public static void MaximizeToMonitor(this Window window, int monitor)
    {
        var screen = Screen.AllScreens.Where(s => s.DeviceName == string.Format(@"\\.\DISPLAY{0}", monitor)).FirstOrDefault();

        if (screen != null)
        {
            MaximizeWindow(window, screen);
        }
    }

    public static void MaximizeWindow(this Window window, Screen screen)
    {
        if (!window.IsLoaded)
            window.WindowStartupLocation = WindowStartupLocation.Manual;

        var workingArea = screen.WorkingArea;
        window.Left = workingArea.Left;
        window.Top = workingArea.Top;
        window.Width = workingArea.Width;
        window.Height = workingArea.Height;
        window.WindowStyle = WindowStyle.None;
        window.ResizeMode = ResizeMode.NoResize;

        if (window.IsLoaded)
            window.WindowState = WindowState.Maximized;
    }

    public static void CenterToFirstMonitor(this Window window)
    {
        var primaryScreen = Screen.AllScreens.Where(s => s.Primary).FirstOrDefault();

        if (primaryScreen != null)
        {
            CenterWindow(window, primaryScreen);
        }
    }

    public static void CenterToSecondMonitor(this Window window)
    {
        var secondScreen = Screen.AllScreens.Where(s => !s.Primary).FirstOrDefault();

        if (secondScreen != null)
        {
            CenterWindow(window, secondScreen);
        }
    }

    public static void CenterToMonitor(this Window window, int monitor)
    {
        var screen = Screen.AllScreens.Where(s => s.DeviceName == string.Format(@"\\.\DISPLAY{0}", monitor)).FirstOrDefault();

        if (screen != null)
        {
            CenterWindow(window, screen);
        }
    }

    public static void CenterWindow(this Window window, Screen screen)
    {
        if (!window.IsLoaded)
            window.WindowStartupLocation = WindowStartupLocation.Manual;

        var workingArea = screen.WorkingArea;
        window.Left = workingArea.Left + (workingArea.Width - window.Width) / 2;
        window.Top = workingArea.Top + (workingArea.Height - window.Height) / 2;

        if (window.IsLoaded)
            window.WindowState = WindowState.Normal;
    }

    public static void MinimizeWindow(this Window window)
    {
        window.WindowState = WindowState.Minimized;
    }

    public static void CenterWindowToParent(this Window window)
    {
        if (window.Owner == null)
            return;

        if (!window.IsLoaded)
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;

        if (window.IsLoaded)
            window.WindowState = WindowState.Normal;
    }
}
