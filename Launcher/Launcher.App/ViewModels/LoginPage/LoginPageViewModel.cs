using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

using Launcher.Commands;
using Launcher.Localization;
using Launcher.Services;
using Launcher.Utilities;

using Serilog;
using Zlo4NET.Api.Service;
using Zlo4NET.Api.Shared;

namespace Launcher.ViewModels
{
    public class LoginPageViewModel : DependencyObject
    {
        #region Constants

        private const string ClientProcessName = "ZClient";

        #endregion

        private readonly INavigationService _navigationService;
        private readonly IZConnection _connection;
        private readonly ILogger _logger;

        #region Ctor

        public LoginPageViewModel(
            INavigationService navigationService
            , IZConnection connection
            , ILogger logger)
        {
            _navigationService = navigationService;
            _connection = connection;
            _logger = logger;

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

        public bool IsConnectButtonAvailable
        {
            get => (bool)GetValue(IsConnectButtonAvailableProperty);
            set => SetValue(IsConnectButtonAvailableProperty, value);
        }
        public static readonly DependencyProperty IsConnectButtonAvailableProperty =
            DependencyProperty.Register("IsConnectButtonAvailable", typeof(bool), typeof(LoginPageViewModel), new PropertyMetadata(false));

        #endregion

        #region Commands

        public ICommand ViewLoadedCommand { get; }

        private void _ViewLoadedExecuteCommand(object view)
        {
            IsConnectButtonAvailable = true;

            // track connection changes
            _connection.ConnectionChanged += _OnConnectionChanged;
        }

        private void _OnConnectionChanged(object sender, ZConnectionChangedEventArgs e)
        {
            if (e.IsConnected)
            {
                Dispatcher.Invoke(() => _navigationService.Navigate("Views\\HomeMenuPage\\HomeMenuPageView.xaml"));
            }
            else
            {
                // show error tip and activate button back
                Dispatcher.Invoke(() =>
                {
                    IsConnectButtonAvailable = true;
                    IsConnectionFailed = true;
                });
            }
        }

        public ICommand ViewUnloadedCommand { get; }

        private void _ViewUnloadedExecuteCommand(object parameter)
        {
            // untrack connection changes
            _connection.ConnectionChanged -= _OnConnectionChanged;
        }

        public ICommand RunClientCommand { get; }

        private void _RunClientExecuteCommand(object parameter)
        {
            // try to determine is client already runned
            var matchedProcesses = Process.GetProcessesByName(ClientProcessName);
            if (matchedProcesses.Any())
            {
                return;
            }

            // TODO: Here should be initialization from settings
            var clientPath = "invalidPath";

            if (FileSystemUtility.FileExists(clientPath) == false)
            {
                var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                var dialogTitle = LocalizationManager.GetTranslationByKey("ChooseClientDialogTitle");
                var dialogWindow = StandardFileDialogUtility.BuildSingleselectOpenFileDialog(baseDirectory, dialogTitle,
                    "ZClient executable file (*.exe)|ZClient.exe");

                // manually choose client file
                var dialogResult = dialogWindow.ShowDialog();
                if (dialogResult.GetValueOrDefault(false) == false)
                {
                    return;
                }

                clientPath = dialogWindow.FileName;
                // TODO: Update settings path here
            }

            // try to run process
            var processWorkingDirectory = Path.GetDirectoryName(clientPath);
            var processStartInfo = new ProcessStartInfo
            {
                FileName = clientPath,
                WorkingDirectory = processWorkingDirectory,
                Verb = "runas"
            };

            try
            {
                Process.Start(processStartInfo);
            }
            catch (Exception exception)
            {
                _logger.Error(exception, $"{nameof(_RunClientExecuteCommand)} Start client process failed.");

                // TODO: Use internal dialog system here
                MessageBox.Show("Can't run, see application log!");
            }
        }

        public ICommand ConnectCommand { get; }

        private void _ConnectExecuteCommand(object parameter)
        {
            // deactivate button first
            IsConnectButtonAvailable = false;

            // try to connect and wait for response to activate button back
            _connection.Connect();
        }

        public ICommand OpenEmuStatusCommand { get; }

        private void _OpenEmuStatusExecuteCommand(object parameter)
        {
            UrlRunUtility.Open("https://zloemu.net/status");
        }

        public ICommand OpenContactDeveloperCommand { get; }

        private void _OpenContactDeveloperExecuteCommand(object parameter)
        {
            // TODO: Open dialog window with contact channels with me
        }

        #endregion
    }
}