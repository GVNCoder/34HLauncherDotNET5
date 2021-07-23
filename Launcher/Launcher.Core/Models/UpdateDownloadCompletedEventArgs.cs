using System;

namespace Launcher.Core.Models
{
    public class UpdateDownloadCompletedEventArgs : EventArgs
    {
        public string DownloadedFilePath { get; }

        public UpdateDownloadCompletedEventArgs(string downloadedFilePath)
        {
            DownloadedFilePath = downloadedFilePath;
        }
    }
}