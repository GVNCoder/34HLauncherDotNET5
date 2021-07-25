// ReSharper disable InconsistentNaming

using System;
using System.Threading.Tasks;

using Launcher.Core.Models;

namespace Launcher.Core.Services
{
    public interface IApplicationUpdater
    {
        event EventHandler<CheckForUpdateEventArgs> OnCheckForUpdateCompleted;
        event EventHandler<ErrorOccuredEventArgs> OnError;
        event EventHandler<DownloadProgressEventArgs> OnUpdateDownloadProgress;
        event EventHandler<DownloadCompletedEventArgs> OnUpdateDownloadCompleted;

        Task CheckForUpdatesAsync(string checkLink, Version currentVersion);
        void Download(string downloadLink, string destinationPath);
        void Install();
        void Cleanup();
    }
}