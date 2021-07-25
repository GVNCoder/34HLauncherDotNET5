// ReSharper disable CheckNamespace
// ReSharper disable MemberCanBeMadeStatic.Global

using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading.Tasks;
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
        private readonly IApplicationUpdater _applicationUpdater;

        private UpdateDescription _updateDescription;
        private Window _currentView;

        #region Ctor

        public StartupWindowViewModel(
            ILogger logger
            , IApplicationUpdater applicationUpdater)
        {
            _logger = logger;
            _applicationUpdater = applicationUpdater;

            _applicationUpdater.OnCheckForUpdateCompleted += _OnUpdaterCheckForUpdateCompleted;
            _applicationUpdater.OnUpdateDownloadProgress += _OnUpdaterUpdateDownloadProgress;
            _applicationUpdater.OnUpdateDownloadCompleted += _OnUpdaterUpdateDownloadCompleted;
            _applicationUpdater.OnError += _OnUpdaterError;

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

            var currentAssemblyName = Assembly.GetExecutingAssembly()
                .GetName();

            // begin check for updates
            _applicationUpdater.CheckForUpdatesAsync(LauncherUpdateDescriptionLink, currentAssemblyName.Version)
                .Forget();
        }

        public ICommand ViewUnloadedCommand { get; }

        private void _ViewUnloadedExecuteCommand(object parameter)
        {
            _applicationUpdater.OnCheckForUpdateCompleted -= _OnUpdaterCheckForUpdateCompleted;
            _applicationUpdater.OnUpdateDownloadProgress -= _OnUpdaterUpdateDownloadProgress;
            _applicationUpdater.OnUpdateDownloadCompleted -= _OnUpdaterUpdateDownloadCompleted;
            _applicationUpdater.OnError -= _OnUpdaterError;
        }

        #endregion

        #region Private helpers

        private void _OnUpdaterError(object sender, ErrorOccuredEventArgs e)
        {
            _logger.Warning(e.Exception, e.Message);
        }

        private void _OnUpdaterUpdateDownloadCompleted(object sender, DownloadCompletedEventArgs e)
        {
            // compute update file hash
            using (var fileSteam = File.Open(e.DownloadedFilePath, FileMode.Open, FileAccess.Read))
            using (var md5 = MD5.Create())
            {
                var updateHashBytes = md5.ComputeHash(fileSteam);
                var encodedUpdateHash = BitConverter.ToString(updateHashBytes)
                    .Replace("-", string.Empty)
                    .ToLower();

                // validate update file
                if (encodedUpdateHash != _updateDescription.ZipHash)
                {
                    _logger.Warning("The update file is corrupted");

                    return;
                }
            }

            // unpack update
            var updateDirectory = Path.GetDirectoryName(e.DownloadedFilePath);

            ZipFile.ExtractToDirectory(e.DownloadedFilePath, updateDirectory);

            // run updater
            var updaterPath = Path.Combine(updateDirectory, _updateDescription.UpdaterFileName);
            var launcherBackPath = Process.GetCurrentProcess().MainModule?.FileName;
            var updateFilesPath = Path.Combine(updateDirectory, _updateDescription.UpdateFilesDirectoryName);

            var process = new Process();
            var processRunInfo = new ProcessStartInfo
            {
                FileName = updaterPath,
                CreateNoWindow = true,
                UseShellExecute = false,
                Arguments = string.Join('|', $"updateFilesPath={updateFilesPath}", $"processBackPath={launcherBackPath}"),
                Verb = "runas"
            };

            // apply settings
            process.StartInfo = processRunInfo;

            // try run updater
            try
            {
                var runResult = process.Start();
                if (runResult == false)
                {
                    _logger.Warning("Can't start a new process");
                }
                else
                {
                    _ShowMainWindow();
                }
            }
            catch (Exception exception)
            {
                _logger.Error(exception, "Can't start a new process");
            }
        }

        private void _OnUpdaterUpdateDownloadProgress(object sender, DownloadProgressEventArgs e)
        {
            UpdateDownloadPercentProgress = e.PercentProgress;
        }

        private void _OnUpdaterCheckForUpdateCompleted(object sender, CheckForUpdateEventArgs e)
        {
            if (e.IsUpdateAvailable)
            {
                var updateDescription = e.UpdateDescription;
                var updateDestinationPath = FileSystemUtility.GetTemporaryDirectory();

                // begin update download
                _updateDescription = updateDescription;
                _applicationUpdater.Download(updateDescription.DownloadLink, updateDestinationPath);
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