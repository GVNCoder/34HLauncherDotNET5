﻿// ReSharper disable CheckNamespace
// ReSharper disable MemberCanBeMadeStatic.Global
// ReSharper disable LoopCanBeConvertedToQuery
// ReSharper disable InvertIf

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;

using Launcher.Commands;
using Launcher.Extensions;
using Launcher.Models;
using Launcher.Services;
using Launcher.Utilities;
using Launcher.Views;

using Newtonsoft.Json;
using Serilog;

namespace Launcher.App.ViewModels
{
    public class StartupWindowViewModel : DependencyObject
    {
        #region Constants

        private const string DevChannelUpdateDescription =
            @"https://drive.google.com/uc?id=1eOdfe2215pMBbYxCS-NsndjnFpWwrdNy&export=download";
        private const string ProdChannelUpdateDescription = "";

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

            _updateInstaller.OnError += _OnUpdaterError;

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

        public bool IsProgressBarVisible
        {
            get => (bool)GetValue(IsProgressBarVisibleProperty);
            set => SetValue(IsProgressBarVisibleProperty, value);
        }
        public static readonly DependencyProperty IsProgressBarVisibleProperty =
            DependencyProperty.Register("IsProgressBarVisible", typeof(bool), typeof(StartupWindowViewModel), new PropertyMetadata(false));

        public bool IsCheckForUpdatesLabelVisible
        {
            get => (bool)GetValue(IsCheckForUpdatesLabelVisibleProperty);
            set => SetValue(IsCheckForUpdatesLabelVisibleProperty, value);
        }
        public static readonly DependencyProperty IsCheckForUpdatesLabelVisibleProperty =
            DependencyProperty.Register("IsCheckForUpdatesLabelVisible", typeof(bool), typeof(StartupWindowViewModel), new PropertyMetadata(true));

        #endregion

        #region Commands

        public ICommand ViewLoadedCommand { get; }

        private void _ViewLoadedExecuteCommand(Window view)
        {
            _currentView = view;

            // determine is we runs as post update
            if (LauncherApp.CommandLineArguments.KeyValueArguments.TryGetValue(CommandLineUtility.PostUpdateDescription, out var postUpdateDescriptionValue))
            {
                var postUpdateDescription = JsonConvert.DeserializeObject<PostUpdateDescription>(postUpdateDescriptionValue);

                _updateInstaller.CleanupFiles(postUpdateDescription.UpdaterFileName, postUpdateDescription.UpdateDirPath);

                // add to log updater error message
                if (string.IsNullOrEmpty(postUpdateDescription.ErrorMessage) == false)
                {
                    _logger.Error($"Updater return error {postUpdateDescription.ErrorMessage}");
                }
            }

            // determine updates channel
            var updateDescriptionLink = LauncherApp.IsDev
                ? DevChannelUpdateDescription
                : ProdChannelUpdateDescription;

            // begin check for updates
            _updateChecker.CheckForUpdatesAsync(updateDescriptionLink)
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

            _updateInstaller.OnError -= _OnUpdaterError;
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
                // prepare arguments
                var downloadedFilePath = e.DownloadedFilePath;
                var updateDirectory = Path.GetDirectoryName(downloadedFilePath);
                var processBackBaseArguments = LauncherApp.CommandLineArguments.ItselfValuedArguments
                    .Single(a => a == CommandLineUtility.DevChannel);

                // build update steps
                var updateSteps = new Func<bool>[]
                {
                    () => _updateInstaller.ValidateUpdateFileHash(downloadedFilePath, _updateDescription.ZipHash),
                    () => _updateInstaller.TryUnpackUpdateFiles(downloadedFilePath),
                    () => _updateInstaller.TryRunUpdater(updateDirectory, _updateDescription, processBackBaseArguments)
                };

                // try to update step by step
                var stepsResults = updateSteps.Any(us => us.Invoke() == false);
                if (stepsResults)
                {
                    // try to cleanup update files
                    _updateInstaller.CleanupFiles(_updateDescription.UpdaterFileName, updateDirectory);
                }
            }

            _ShowMainWindow();
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

            if (updateDescription != null
                && (LauncherApp.IsDev
                    ? updateDescription.LatestVersion > currentAssemblyName.Version
                    : updateDescription.LatestVersion > currentAssemblyName.Version
                      || currentAssemblyName.Version.Revision != 0))
            {
                // update UI
                Dispatcher.Invoke(() =>
                {
                    IsCheckForUpdatesLabelVisible = false;
                    IsProgressBarVisible = true;
                });

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

            var mainWindow = new MainWindowView { ShowActivated = true };

            // set as main window
            Application.Current.MainWindow = mainWindow;

            mainWindow.Show();

            _currentView.Close();
        }

        #endregion
    }
}