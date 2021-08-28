
using System;
using System.Windows;
using System.Windows.Input;

using Launcher.Commands;
using Launcher.Helpers;
using Launcher.Services;
using Serilog;
using Zlo4NET.Api.Service;

namespace Launcher.ViewModels
{
    public class MainWindowViewModel : DependencyObject
    {
        private readonly ILogger _logger;
        private readonly INavigationService _navigationService;
        private readonly IZConnection _connection;

        private WindowAdjustWorker _windowAdjustWorker;
        private Window _currentView;

        #region Ctor

        public MainWindowViewModel(
            ILogger logger
            , INavigationService navigationService
            , IZConnection connection)
        {
            _logger = logger;
            _navigationService = navigationService;
            _connection = connection;

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

            // default navigation
            _navigationService.Navigate("Views\\LoginPage\\LoginPageView.xaml");

            // track connection changes
            _connection.ConnectionChanged += (sender, args) =>
            {
                if (args.IsConnected)
                {
                    // TODO: Navigate to Home page
                }
                else
                {
                    _navigationService.Navigate("Views\\LoginPage\\LoginPageView.xaml");
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