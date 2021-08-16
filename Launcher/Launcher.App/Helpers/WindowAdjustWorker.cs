// ReSharper disable SwitchStatementMissingSomeCases

using System;
using System.Windows;
using System.Windows.Shell;

using Launcher.Core.Utilities;

namespace Launcher.Helpers
{
    public class WindowAdjustWorker
    {
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
                    var (left, top, right, bottom) = WindowResizeBorderFix.GetWindowResizeBorderThickness();

                    _window.BorderThickness = new Thickness(left, top, right, bottom);
                    _windowChrome.CaptionHeight = _maximizedCaptionHeight;
                    break;
            }
        }

        #endregion
    }
}