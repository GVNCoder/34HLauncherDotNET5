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

        #region Window settings

        public static string Title { get; } = "34H Launcher";

        #endregion

        private readonly ILogger _logger;
        private readonly IUpdateChecker _updateChecker;
        private readonly IUpdateDownloader _updateDownloader;

        private UpdateDescription _updateDescription;
        private Window _currentView;

        #region Ctor

        public StartupWindowViewModel(
            ILogger logger
            , IUpdateChecker updateChecker
            , IUpdateDownloader updateDownloader)
        {
            _logger = logger;
            _updateChecker = updateChecker;
            _updateDownloader = updateDownloader;

            _updateChecker.OnUpdateCheckError += _OnUpdateCheckError;
            _updateDownloader.OnDownloadError += _OnDownloadError;
            _updateDownloader.OnDownloadCompleted += _OnDownloadCompleted;
            _updateDownloader.OnDownloadProgress += _OnDownloadProcess;

            // create commands
            ViewLoadedCommand = new AsyncRelayCommand<Window>(_ViewLoadedExecuteCommand);
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

        private async Task _ViewLoadedExecuteCommand(Window view)
        {
            _currentView = view;

            var currentAssemblyName = Assembly.GetExecutingAssembly()
                .GetName();
            var isNewVersionAvailable =
                await _updateChecker.IsNewVersionAvailableAsync(LauncherUpdateDescriptionLink,
                    currentAssemblyName.Version);

            if (isNewVersionAvailable)
            {
                var updateDescription = _updateChecker.UpdateDescription;
                var updateDestinationPath = FileSystemUtility.GetTemporaryDirectory();

                _updateDescription = updateDescription;
                _updateDownloader.Download(updateDescription.DownloadLink, updateDestinationPath);
            }
            else
            {
                _ShowMainWindow();
            }
        }

        public ICommand ViewUnloadedCommand { get; }

        private void _ViewUnloadedExecuteCommand(object parameter)
        {
            _updateChecker.OnUpdateCheckError -= _OnUpdateCheckError;
            _updateDownloader.OnDownloadError -= _OnDownloadError;
            _updateDownloader.OnDownloadCompleted -= _OnDownloadCompleted;
            _updateDownloader.OnDownloadProgress -= _OnDownloadProcess;
        }

        #endregion

        #region Private helpers

        private void _OnUpdateCheckError(object sender, UpdateCheckErrorEventArgs e)
            => _logger.Warning(e.Exception, e.Message);

        private void _OnDownloadError(object sender, UpdateDownloadErrorEventArgs e)
            => _logger.Warning(e.Exception, e.Message);

        private void _OnDownloadCompleted(object sender, UpdateDownloadCompletedEventArgs e)
        {
            // TODO: Extract to IUpdater impl

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

        private void _OnDownloadProcess(object sender, UpdateDownloadProgressEventArgs e)
        {
            UpdateDownloadPercentProgress = e.PercentProgress;
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