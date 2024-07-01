using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

using ControlzEx.Standard;

using MahApps.Metro.Controls;

using static Corathing.Organizer.WPF.Natives.WindowNativeMethods;

namespace Corathing.Organizer.WPF.Extensions;

public static class WindowExtensions
{
    private static List<MonitorInfo> GetMonitors()
    {
        var monitors = new List<MonitorInfo>();

        MonitorEnumProc callback = (IntPtr hMonitor, IntPtr hdcMonitor, ref Rect lprcMonitor, IntPtr dwData) =>
        {
            var monitorInfo = new MonitorInfo
            {
                Size = Marshal.SizeOf(typeof(MonitorInfo))
            };

            if (GetMonitorInfo(hMonitor, ref monitorInfo))
            {
                monitors.Add(monitorInfo);
            }
            return true; // Continue enumeration
        };

        EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, callback, IntPtr.Zero);
        return monitors;
    }

    private static Rect GetWindowBoundingRectangle(this MetroWindow window)
    {
        Rect result = new Rect(0.0, 0.0, 0.0, 0.0);
        if (window != null && PresentationSource.FromVisual(window) is HwndSource hwndSource && !hwndSource.IsDisposed && hwndSource.RootVisual != null && hwndSource.Handle != IntPtr.Zero)
        {
            RECT rECT = new RECT(0, 0, 0, 0);
            try
            {
                rECT = NativeMethods.GetWindowRect(hwndSource.Handle);
            }
            catch (Win32Exception)
            {
            }

            result = new Rect(rECT.Left, rECT.Top, rECT.Width, rECT.Height);
        }

        return result;
    }

    //public static void MaximizeToFirstMonitor(this MetroWindow window)
    //{
    //    var primaryScreen = Screen.AllScreens.Where(s => s.Primary).FirstOrDefault();

    //    if (primaryScreen != null)
    //    {
    //        MaximizeWindow(window, primaryScreen);
    //    }
    //}

    //public static void MaximizeToSecondMonitor(this MetroWindow window)
    //{
    //    var secondScreen = Screen.AllScreens.Where(s => !s.Primary).FirstOrDefault();

    //    if (secondScreen != null)
    //    {
    //        MaximizeWindow(window, secondScreen);
    //    }
    //}

    //public static void MaximizeToMonitor(this MetroWindow window, int monitor)
    //{
    //    window.Maxi
    //    var screen = Screen.AllScreens.Where(s => s.DeviceName == string.Format(@"\\.\DISPLAY{0}", monitor)).FirstOrDefault();

    //    if (screen != null)
    //    {
    //        MaximizeWindow(window, screen);
    //    }
    //}

    public static void MaximizeWindow(this MetroWindow window)
    {
        if (!window.IsLoaded)
            window.WindowStartupLocation = WindowStartupLocation.Manual;

        var workingArea = window.GetWindowBoundingRectangle();
        window.Left = workingArea.Left;
        window.Top = workingArea.Top;
        window.Width = workingArea.Width;
        window.Height = workingArea.Height;
        window.WindowStyle = WindowStyle.None;
        window.ResizeMode = ResizeMode.NoResize;

        if (window.IsLoaded)
            window.WindowState = WindowState.Maximized;
    }

    public static void CenterToFirstMonitor(this MetroWindow window)
    {
        //var primaryScreen = Screen.AllScreens.Where(s => s.Primary).FirstOrDefault();

        //if (primaryScreen != null)
        //{
        //    CenterWindow(window, primaryScreen);
        //}
    }

    public static void CenterToSecondMonitor(this MetroWindow window)
    {
        //var secondScreen = Screen.AllScreens.Where(s => !s.Primary).FirstOrDefault();

        //if (secondScreen != null)
        //{
        //    CenterWindow(window, secondScreen);
        //}
    }

    public static void CenterToMonitor(this MetroWindow window, int monitor)
    {
        //var screen = Screen.AllScreens.Where(s => s.DeviceName == string.Format(@"\\.\DISPLAY{0}", monitor)).FirstOrDefault();

        //if (screen != null)
        //{
        //    CenterWindow(window, screen);
        //}
    }

    public static void CenterWindow(this MetroWindow window)
    {
        if (!window.IsLoaded)
            window.WindowStartupLocation = WindowStartupLocation.Manual;

        var workingArea = window.GetWindowBoundingRectangle();
        window.Left = workingArea.Left + (workingArea.Width - window.Width) / 2;
        window.Top = workingArea.Top + (workingArea.Height - window.Height) / 2;

        if (window.IsLoaded)
            window.WindowState = WindowState.Normal;
    }

    public static void MinimizeWindow(this MetroWindow window)
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
        {
            window.WindowState = WindowState.Normal;

            if (window.Owner != null)
            {
                Window parent = window.Owner;
                window.Left = parent.Left + (parent.ActualWidth - window.ActualWidth) / 2;
                window.Top = parent.Top + (parent.ActualHeight - window.ActualHeight) / 2;
            }
        }
    }
}
