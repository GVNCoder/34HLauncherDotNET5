﻿// ReSharper disable MemberCanBePrivate.Global

using System.Linq;
using System.Windows;
using System.Windows.Input;

using Launcher.Commands;
using Launcher.Views;

namespace Launcher.ViewModels
{
    public class WindowNonClientViewModel : DependencyObject
    {
        private readonly Window _windowRef;

        private bool _isWindowCanBeRestored;

        #region Ctor

        public WindowNonClientViewModel()
        {
            _windowRef = Application.Current.Windows
                .OfType<MainWindowView>()
                .First();

            // track window state
            _windowRef.StateChanged += (sender, args) => _isWindowCanBeRestored = _windowRef.WindowState == WindowState.Maximized;

            // create commands
            SystemWindowCloseCommand = new RelayCommand<object>(_SystemWindowCloseExecuteCommand);
            SystemWindowMinimizeCommand = new RelayCommand<object>(_SystemWindowMinimizeExecuteCommand);
            SystemWindowMaximizeCommand = new RelayCommand<object>(_SystemWindowMaximizeExecuteCommand);
        }

        #endregion

        #region Commands

        public ICommand SystemWindowCloseCommand { get; }

        private void _SystemWindowCloseExecuteCommand(object args)
        {
            SystemCommands.CloseWindow(_windowRef);
        }

        public ICommand SystemWindowMinimizeCommand { get; }

        private void _SystemWindowMinimizeExecuteCommand(object args)
        {
            SystemCommands.MinimizeWindow(_windowRef);
        }

        public ICommand SystemWindowMaximizeCommand { get; }

        private void _SystemWindowMaximizeExecuteCommand(object args)
        {
            if (_isWindowCanBeRestored)
            {
                SystemCommands.RestoreWindow(_windowRef);
                _isWindowCanBeRestored = false;
            }
            else
            {
                SystemCommands.MaximizeWindow(_windowRef);
                _isWindowCanBeRestored = true;
            }
        }

        #endregion
    }
}