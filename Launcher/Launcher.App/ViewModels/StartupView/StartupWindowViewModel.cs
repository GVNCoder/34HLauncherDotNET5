// ReSharper disable CheckNamespace
// ReSharper disable MemberCanBeMadeStatic.Global

using System.Reflection;
using System.Windows;
using System.Windows.Input;

using Launcher.Commands;
using Launcher.Core.Extensions;
using Launcher.Core.Models;
using Launcher.Core.Services;
using Launcher.Core.Utilities;
using Launcher.Views;

using Serilog;

namespace Launcher.App.ViewModels
{
    public class StartupWindowViewModel : DependencyObject
    {
        #region Constants

#if DEBUG
        private const string LauncherUpdateDescriptionLink = @"https://raw.githubusercontent.com/GVNCoder/34HLauncherDotNET5/master/Launcher/UpdateDescription.json";
#else
        private const string LauncherUpdateDescriptionLink = @"";
#endif

        #endregion

        private readonly ILogger _logger;
        private readonly IUpdateChecker _updateChecker;
        private readonly IUpdateDownloader _updateDownloader;
        private readonly IUpdateInstaller _updateInstaller;

        private UpdateDescription _updateDescription;
        private Window _currentView;

        #region Ctor

        public StartupWindowViewModel(
            ILogger logger
            , IUpdateChecker updateChecker
            , IUpdateDownloader updateDownloader
            , IUpdateInstaller updateInstaller)
        {
            _logger = logger;
            _updateChecker = updateChecker;
            _updateDownloader = updateDownloader;
            _updateInstaller = updateInstaller;

            _updateChecker.OnCheckForUpdateCompleted += _OnUpdaterCheckForUpdateCompleted;
            _updateChecker.OnError += _OnUpdaterError;

            _updateDownloader.OnUpdateDownloadCompleted += _OnUpdaterUpdateDownloadCompleted;
            _updateDownloader.OnUpdateDownloadProgress += _OnUpdaterUpdateDownloadProgress;
            _updateDownloader.OnError += _OnUpdaterError;

            // create commands
            ViewLoadedCommand = new RelayCommand<Window>(_ViewLoadedExecuteCommand);
            ViewUnloadedCommand = new RelayCommand<object>(_ViewUnloadedExecuteCommand);
        }

        #endregion

        #region Dependency properties

        public int UpdateDownloadPercentProgress
        {
            get => (int)GetValue(UpdateDownloadPercentProgressProperty);
            set => SetValue(UpdateDownloadPercentProgressProperty, value);
        }
        public static readonly DependencyProperty UpdateDownloadPercentProgressProperty =
            DependencyProperty.Register("UpdateDownloadPercentProgress", typeof(int), typeof(StartupWindowViewModel), new PropertyMetadata(0));

        #endregion

        #region Commands

        public ICommand ViewLoadedCommand { get; }

        private void _ViewLoadedExecuteCommand(Window view)
        {
            _currentView = view;

            // begin check for updates
            _updateChecker.CheckForUpdatesAsync(LauncherUpdateDescriptionLink)
                .Forget();
        }

        public ICommand ViewUnloadedCommand { get; }

        private void _ViewUnloadedExecuteCommand(object parameter)
        {
            _updateChecker.OnCheckForUpdateCompleted -= _OnUpdaterCheckForUpdateCompleted;
            _updateChecker.OnError -= _OnUpdaterError;

            _updateDownloader.OnUpdateDownloadCompleted -= _OnUpdaterUpdateDownloadCompleted;
            _updateDownloader.OnUpdateDownloadProgress -= _OnUpdaterUpdateDownloadProgress;
            _updateDownloader.OnError -= _OnUpdaterError;
        }

        #endregion

        #region Private helpers

        private void _OnUpdaterError(object sender, ErrorOccuredEventArgs e)
        {
            _logger.Warning(e.Exception, e.Message);
        }

        private void _OnUpdaterUpdateDownloadCompleted(object sender, DownloadCompletedEventArgs e)
        {
            if (e.Successful)
            {
                _applicationUpdater.Install(e.DownloadedFilePath, _updateDescription);
            }
        }

        private void _OnUpdaterUpdateDownloadProgress(object sender, DownloadProgressEventArgs e)
        {
            UpdateDownloadPercentProgress = e.PercentProgress;
        }

        private void _OnUpdaterCheckForUpdateCompleted(object sender, CheckForUpdateEventArgs e)
        {
            var currentAssemblyName = Assembly.GetExecutingAssembly()
                .GetName();
            var updateDescription = e.UpdateDescription;

            if (updateDescription != null && updateDescription.LatestVersion > currentAssemblyName.Version)
            {
                _updateDescription = updateDescription;
                var updateDestinationPath = FileSystemUtility.GetTemporaryDirectory();

                // begin update download
                _updateDescription = updateDescription;
                _updateDownloader.DownloadAsync(updateDescription.DownloadLink, updateDestinationPath);
            }
            else
            {
                _ShowMainWindow();
            }
        }

        private void _ShowMainWindow()
        {
            _currentView.Hide();

            var mainWindow = new MainWindowView();

            mainWindow.Show();
            mainWindow.ShowActivated = true;

            _currentView.Close();
        }

        #endregion
    }
}