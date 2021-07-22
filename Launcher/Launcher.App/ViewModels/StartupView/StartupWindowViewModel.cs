// ReSharper disable CheckNamespace

using System.Windows;

using Launcher.Core.Services;

namespace Launcher.App.ViewModels
{
    public class StartupWindowViewModel : DependencyObject
    {
        #region Constants

        private const string LauncherCheckUpdateLink = @"";
        private const string UpdaterCheckUpdateLink = @"";

        #endregion

        #region Window settings

        public static string Title { get; } = "34H Launcher";

        #endregion

        private readonly IUpdateChecker _updateChecker;
        private readonly IUpdateDownloader _updateDownloader;

        #region Ctor

        public StartupWindowViewModel(
            IUpdateChecker updateChecker
            , IUpdateDownloader updateDownloader)
        {
            _updateChecker = updateChecker;
            _updateDownloader = updateDownloader;
        }

        #endregion
    }
}