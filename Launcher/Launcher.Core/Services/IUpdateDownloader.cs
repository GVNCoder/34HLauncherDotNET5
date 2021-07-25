using System;
using Launcher.Core.Models;

namespace Launcher.Core.Services
{
    public interface IUpdateDownloader
    {
        event EventHandler<DownloadProgressEventArgs> OnUpdateDownloadProgress;
        event EventHandler<DownloadCompletedEventArgs> OnUpdateDownloadCompleted;
        event EventHandler<ErrorOccuredEventArgs> OnError;

        void DownloadAsync(string downloadLink, string destinationPath);
    }
}