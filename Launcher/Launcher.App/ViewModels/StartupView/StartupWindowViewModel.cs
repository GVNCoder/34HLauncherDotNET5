// ReSharper disable CheckNamespace
// ReSharper disable MemberCanBeMadeStatic.Global

using System.Reflection;
using System.Windows;
using System.Windows.Input;

using Launcher.Commands;
using Launcher.Core.Models;
using Launcher.Core.Services;
using Launcher.Core.Utilities;

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

        public ICommand ViewLoadedCommand => new AsyncRelayCommand<object>(async parameter =>
        {
            var currentAssemblyName = Assembly.GetExecutingAssembly()
                .GetName();
            var isNewVersionAvailable =
                await _updateChecker.IsNewVersionAvailableAsync(LauncherUpdateDescriptionLink,
                    currentAssemblyName.Version);

            if (isNewVersionAvailable)
            {
                var updateDescription = _updateChecker.UpdateDescription;
                var updateDestinationPath = FileSystemUtility.GetTemporaryDirectory();

                _updateDownloader.Download(updateDescription.DownloadLink, updateDestinationPath);
            }
        });

        public ICommand ViewUnloadedCommand => new RelayCommand<object>(parameter =>
        {
            _updateChecker.OnUpdateCheckError -= _OnUpdateCheckError;
            _updateDownloader.OnDownloadError -= _OnDownloadError;
            _updateDownloader.OnDownloadCompleted -= _OnDownloadCompleted;
            _updateDownloader.OnDownloadProgress -= _OnDownloadProcess;
        });

        #endregion

        #region Private helpers

        private void _OnUpdateCheckError(object sender, UpdateCheckErrorEventArgs e)
            => _logger.Warning(e.Exception, e.Message);

        private void _OnDownloadError(object sender, UpdateDownloadErrorEventArgs e)
            => _logger.Warning(e.Exception, e.Message);

        private void _OnDownloadCompleted(object sender, UpdateDownloadCompletedEventArgs e)
        {
            // TODO: Do some stuff with update file here...
        }

        private void _OnDownloadProcess(object sender, UpdateDownloadProgressEventArgs e)
        {
            UpdateDownloadPercentProgress = e.PercentProgress;
        }

        #endregion
    }
}