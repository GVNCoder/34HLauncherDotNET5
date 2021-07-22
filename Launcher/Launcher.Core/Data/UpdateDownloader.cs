using Launcher.Core.Models;
using Launcher.Core.Services;

using System;

namespace Launcher.Core.Data
{
    public class UpdateDownloader : IUpdateDownloader
    {
        #region IUpdateDownloader

        public event EventHandler<DownloadCompletedEventArgs> OnDownloadCompleted;
        public event EventHandler<DownloadErrorEventArgs> OnDownloadError;
        public event EventHandler<DownloadProgressEventArgs> OnDownloadProgress;

        public void Download(string downloadLink, string destinationPath)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}