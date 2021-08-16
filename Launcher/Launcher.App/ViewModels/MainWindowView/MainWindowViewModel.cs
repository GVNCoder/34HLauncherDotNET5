
using System;
using System.Windows;
using System.Windows.Input;

using Launcher.Commands;
using Launcher.Helpers;

using Serilog;

namespace Launcher.ViewModels
{
    public class MainWindowViewModel : DependencyObject
    {
        private readonly ILogger _logger;

        private WindowAdjustWorker _windowAdjustWorker;
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

            // window resize border fix and more
            // https://stackoverflow.com/questions/2967218/window-out-of-the-screen-when-maximized-using-wpf-shell-integration-library/2975574#2975574
            _windowAdjustWorker = new WindowAdjustWorker(_currentView);
        }

        public ICommand ViewUnloadedCommand { get; }

        private void _ViewUnloadedExecuteCommand(object parameter)
        {
        }

        #endregion
    }
}