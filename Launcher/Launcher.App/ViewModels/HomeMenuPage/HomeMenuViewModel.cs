using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

using Launcher.Commands;
using Launcher.Services;

using Serilog;
using Zlo4NET.Api.Service;

namespace Launcher.ViewModels
{
    public class HomeMenuViewModel : DependencyObject
    {
        private readonly INavigationService _navigationService;
        private readonly IZInstalledGames _installedGamesService;
        private readonly ILogger _logger;

        #region Ctor

        public HomeMenuViewModel(
            INavigationService navigationService
            , IZInstalledGames installedGamesService
            , ILogger logger)
        {
            _navigationService = navigationService;
            _installedGamesService = installedGamesService;
            _logger = logger;

            // create commands
            ViewLoadedCommand = new AsyncRelayCommand<object>(_ViewLoadedExecuteCommand);
            ViewUnloadedCommand = new RelayCommand<object>(_ViewUnloadedExecuteCommand);
        }

        #endregion

        #region Dependency properties


        #endregion

        #region Commands

        public ICommand ViewLoadedCommand { get; }

        private async Task _ViewLoadedExecuteCommand(object view)
        {
        }

        public ICommand ViewUnloadedCommand { get; }

        private void _ViewUnloadedExecuteCommand(object parameter)
        {
        }

        #endregion
    }
}