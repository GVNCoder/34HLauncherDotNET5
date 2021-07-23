using System;
using Launcher.Core.Models;

namespace Launcher.Core.Services
{
    public interface IUpdateDownloader
    {
        void Download(string downloadLink, string destinationPath);

        event EventHandler<UpdateDownloadCompletedEventArgs> OnDownloadCompleted;
        event EventHandler<UpdateDownloadErrorEventArgs> OnDownloadError;
        event EventHandler<UpdateDownloadProgressEventArgs> OnDownloadProgress;
    }
}