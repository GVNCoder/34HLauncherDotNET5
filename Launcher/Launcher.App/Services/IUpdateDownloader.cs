using System;
using Launcher.Models;

namespace Launcher.Services
{
    public interface IUpdateDownloader
    {
        event EventHandler<DownloadProgressEventArgs> OnUpdateDownloadProgress;
        event EventHandler<DownloadCompletedEventArgs> OnUpdateDownloadCompleted;
        event EventHandler<ErrorOccuredEventArgs> OnError;

        void DownloadAsync(string downloadLink, string destinationPath);
    }
}