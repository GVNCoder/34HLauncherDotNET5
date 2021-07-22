using System;
using Launcher.Core.Models;

namespace Launcher.Core.Services
{
    public interface IUpdateDownloader
    {
        void Download(string downloadLink, string destinationPath);

        event EventHandler<DownloadCompletedEventArgs> OnDownloadCompleted;
        event EventHandler<DownloadErrorEventArgs> OnDownloadError;
        event EventHandler<DownloadProgressEventArgs> OnDownloadProgress;
    }
}