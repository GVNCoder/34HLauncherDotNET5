// ReSharper disable SwitchStatementMissingSomeCases
// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo

using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Shell;

using Launcher.Core.Utilities;

namespace Launcher.Helpers
{
    public class WindowAdjustWorker
    {
        #region Constants

        private const float DefaultWPFDPI = 96f;

        #endregion

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

        private readonly Window _window;
        private readonly WindowChrome _windowChrome;

        private readonly Thickness _defaultBorderThickness;
        private readonly double _maximizedCaptionHeight;
        private readonly double _normalCaptionHeight;

        #region Ctor

        public WindowAdjustWorker(Window window)
        {
            _window = window;
            _windowChrome = WindowChrome.GetWindowChrome(window);
            _defaultBorderThickness = window.BorderThickness;

            // calculate caption height
            var defaultCaptionHeight = _windowChrome.CaptionHeight;
            var resizeBorderHeight = _windowChrome.ResizeBorderThickness.Top;

            _maximizedCaptionHeight = defaultCaptionHeight + resizeBorderHeight;
            _normalCaptionHeight = defaultCaptionHeight - resizeBorderHeight;

            // fix caption height
            _windowChrome.CaptionHeight = _normalCaptionHeight;

            // track window state changes
            _window.StateChanged += _OnWindowStateChanged;
        }

        #endregion

        #region Private helpers

        private void _OnWindowStateChanged(object sender, EventArgs e)
        {
            switch (_window.WindowState)
            {
                case WindowState.Normal:
                case WindowState.Minimized:
                    _windowChrome.CaptionHeight = _normalCaptionHeight;
                    _window.BorderThickness = _defaultBorderThickness;
                    break;
                case WindowState.Maximized:
                    var (left, top, right, bottom) = _GetWindowResizeBorderThickness();

                    _window.BorderThickness = new Thickness(left, top, right, bottom);
                    _windowChrome.CaptionHeight = _maximizedCaptionHeight;
                    break;
            }
        }

        private static (float left, float top, float right, float bottom) _GetWindowResizeBorderThickness()
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
    }
}