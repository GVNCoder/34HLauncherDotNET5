// ReSharper disable SwitchStatementMissingSomeCases

using System;
using System.Windows;
using System.Windows.Input;

using Launcher.Commands;
using Launcher.Core.Utilities;

using Serilog;

namespace Launcher.ViewModels
{
    public class MainWindowViewModel : DependencyObject
    {
        private readonly ILogger _logger;

        private Thickness _defaultBorderThickness;
        private Window _currentView;

        #region Ctor

        public MainWindowViewModel(
            ILogger logger)
        {
            _logger = logger;

            // create commands
            ViewLoadedCommand = new RelayCommand<Window>(_ViewLoadedExecuteCommand);
            ViewUnloadedCommand = new RelayCommand<object>(_ViewUnloadedExecuteCommand);
        }

        #endregion

        #region Commands

        public ICommand ViewLoadedCommand { get; }

        private void _ViewLoadedExecuteCommand(Window view)
        {
            _currentView = view;

            // window resize border fix
            // https://stackoverflow.com/questions/2967218/window-out-of-the-screen-when-maximized-using-wpf-shell-integration-library/2975574#2975574
            _defaultBorderThickness = _currentView.BorderThickness;
            _currentView.StateChanged += (sender, args) =>
            {
                switch (_currentView.WindowState)
                {
                    case WindowState.Normal:
                    case WindowState.Minimized:
                        _currentView.BorderThickness = _defaultBorderThickness;
                        break;
                    case WindowState.Maximized:
                        var (left, top, right, bottom) = WindowResizeBorderFix.GetWindowResizeBorderThickness();
                        _currentView.BorderThickness = new Thickness(left, top, right, bottom);
                        break;
                }
            };
        }

        public ICommand ViewUnloadedCommand { get; }

        private void _ViewUnloadedExecuteCommand(object parameter)
        {
        }

        #endregion
    }
}