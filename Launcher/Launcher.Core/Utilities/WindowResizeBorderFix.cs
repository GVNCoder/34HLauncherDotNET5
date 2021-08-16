// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

using System;
using System.Runtime.InteropServices;

namespace Launcher.Core.Utilities
{
    public static class WindowResizeBorderFix
    {
        #region Constants

        private const float DefaultWPFDPI = 96f;

        #endregion

        public static (float left, float top, float right, float bottom) GetWindowResizeBorderThickness()
        {
            var dpiX = _GetDpi(GetDeviceCapsIndex.LOGPIXELSX);
            var dpiY = _GetDpi(GetDeviceCapsIndex.LOGPIXELSY);

            var dx = GetSystemMetrics(GetSystemMetricsIndex.CXFRAME);
            var dy = GetSystemMetrics(GetSystemMetricsIndex.CYFRAME);

            // this adjustment is needed only since .NET 4.5 
            var d = GetSystemMetrics(GetSystemMetricsIndex.SM_CXPADDEDBORDER);

            dx += d;
            dy += d;

            var leftBorder = dx / dpiX;
            var topBorder = dy / dpiY;

            return (leftBorder, topBorder, leftBorder, topBorder);
        }

        #region External imports

        [DllImport("user32.dll")]
        private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        [DllImport("user32.dll")]
        private static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("gdi32.dll")]
        private static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

        [DllImport("user32.dll")]
        private static extern int GetSystemMetrics(GetSystemMetricsIndex nIndex);

        #endregion

        #region Private helpers

        private static float _GetDpi(GetDeviceCapsIndex index)
        {
            var desktopWnd = IntPtr.Zero;
            var dc = GetDC(desktopWnd);

            float dpi;
            try
            {
                dpi = GetDeviceCaps(dc, (int)index);
            }
            finally
            {
                ReleaseDC(desktopWnd, dc);
            }
            return dpi / DefaultWPFDPI;
        }

        #endregion

        #region Internal types

        private enum GetDeviceCapsIndex
        {
            LOGPIXELSX = 88,
            LOGPIXELSY = 90
        }

        private enum GetSystemMetricsIndex
        {
            CXFRAME = 32,
            CYFRAME = 33,
            SM_CXPADDEDBORDER = 92
        }

        #endregion
    }
}