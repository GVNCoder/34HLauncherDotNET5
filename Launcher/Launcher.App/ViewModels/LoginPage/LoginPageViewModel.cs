using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

using Launcher.Commands;
using Launcher.Utilities;
using Zlo4NET.Api.Service;

namespace Launcher.ViewModels
{
    public class LoginPageViewModel : DependencyObject
    {
        private readonly IZConnection _connection;

        #region Ctor

        public LoginPageViewModel(
            IZConnection connection)
        {
            _connection = connection;

            // create commands
            ViewLoadedCommand = new RelayCommand<object>(_ViewLoadedExecuteCommand);
            ViewUnloadedCommand = new RelayCommand<object>(_ViewUnloadedExecuteCommand);
            RunClientCommand = new RelayCommand<object>(_RunClientExecuteCommand);
            ConnectCommand = new RelayCommand<object>(_ConnectExecuteCommand);
            OpenEmuStatusCommand = new RelayCommand<object>(_OpenEmuStatusExecuteCommand);
            OpenContactDeveloperCommand = new RelayCommand<object>(_OpenContactDeveloperExecuteCommand);
        }

        #endregion

        #region Dependency properties

        public bool IsConnectionFailed
        {
            get => (bool)GetValue(IsConnectionFailedProperty);
            set => SetValue(IsConnectionFailedProperty, value);
        }
        public static readonly DependencyProperty IsConnectionFailedProperty =
            DependencyProperty.Register("IsConnectionFailed", typeof(bool), typeof(LoginPageViewModel), new PropertyMetadata(false));

        #endregion

        #region Commands

        public ICommand ViewLoadedCommand { get; }

        private void _ViewLoadedExecuteCommand(object view)
        {
        }

        public ICommand ViewUnloadedCommand { get; }

        private void _ViewUnloadedExecuteCommand(object parameter)
        {
        }

        public ICommand RunClientCommand { get; }

        private void _RunClientExecuteCommand(object parameter)
        {
        }

        public ICommand ConnectCommand { get; }

        private void _ConnectExecuteCommand(object parameter)
        {
        }

        public ICommand OpenEmuStatusCommand { get; }

        private void _OpenEmuStatusExecuteCommand(object parameter)
        {
            UrlRunUtility.Open("https://zloemu.net/status");
        }

        public ICommand OpenContactDeveloperCommand { get; }

        private void _OpenContactDeveloperExecuteCommand(object parameter)
        {
        }

        #endregion
    }
}