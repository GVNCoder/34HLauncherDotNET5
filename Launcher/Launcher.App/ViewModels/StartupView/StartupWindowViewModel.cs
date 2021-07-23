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
        private const string LauncherUpdateDescriptionLink = @"";
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
        {

        }

        private void _OnDownloadError(object sender, UpdateDownloadErrorEventArgs e)
        {

        }

        private void _OnDownloadCompleted(object sender, UpdateDownloadCompletedEventArgs e)
        {

        }

        private void _OnDownloadProcess(object sender, UpdateDownloadProgressEventArgs e)
        {

        }

        #endregion
    }
}