using System;
using System.Drawing;
using System.Management;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
// ReSharper disable InconsistentNaming

namespace WpfUtilities
{
    // https://docs.microsoft.com/en-us/windows/desktop/hidpi/wm-dpichanged
    // https://docs.microsoft.com/en-us/dotnet/api/microsoft.win32.systemevents?redirectedfrom=MSDN&view=netframework-4.7.2

    public static class DpiHelper
    {
        public static Dpi GetDpiByManagement()
        {
            var dpiX = 96.0;
            var dpiY = 96.0;

            using (ManagementClass mc = new ManagementClass("Win32_DesktopMonitor"))
            {
                using (ManagementObjectCollection moc = mc.GetInstances())
                {
                    foreach (ManagementBaseObject each in moc)
                    {
                        dpiX = int.Parse(each.Properties["PixelsPerXLogicalInch"].Value.ToString());
                        dpiY = int.Parse(each.Properties["PixelsPerYLogicalInch"].Value.ToString());
                    }
                }
            }

            return new Dpi(dpiX, dpiY);
        }

        public static Dpi GetDpiByGraphics()
        {
            double dpiX;
            double dpiY;

            using (var graphics = Graphics.FromHwnd(IntPtr.Zero))
            {
                dpiX = graphics.DpiX;
                dpiY = graphics.DpiY;
            }

            return new Dpi(dpiX, dpiY);
        }

        public static Dpi GetDpiFromVisual(Visual visual)
        {
            PresentationSource source = PresentationSource.FromVisual(visual);
            double dpiX = 96.0, dpiY = 96.0;

            if (source != null && source.CompositionTarget != null)
            {
                dpiX = 96.0 * source.CompositionTarget.TransformToDevice.M11;
                dpiY = 96.0 * source.CompositionTarget.TransformToDevice.M22;
            }

            return new Dpi(dpiX, dpiY);
        }

        private const int LOGPIXELSX = 88;
        private const int LOGPIXELSY = 90;

        [DllImport("gdi32.dll")]
        static extern int GetDeviceCaps(IntPtr hdc, int index);

        [DllImport("user32.dll")]
        static extern IntPtr GetDC(IntPtr Hwnd);

        public static void GetDpiSetting(out double dpiX, out double dpiY)
        {
            // get desktop dc
            IntPtr h = GetDC(IntPtr.Zero);

            // get dpi from dc
            dpiX = GetDeviceCaps(h, LOGPIXELSX);
            dpiY = GetDeviceCaps(h, LOGPIXELSY);
        }
    }

    public struct Dpi
    {
        public double X { get; set; }

        public double Y { get; set; }

        public Dpi(double x, double y)
        {
            X = x;
            Y = y;
        }
    }
}