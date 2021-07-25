using System;

namespace Launcher.Core.Models
{
    public class DownloadCompletedEventArgs : EventArgs
    {
        public bool Successful { get; }
        public string DownloadedFilePath { get; }

        public DownloadCompletedEventArgs(bool successful, string downloadedFilePath)
        {
            Successful = successful;
            DownloadedFilePath = downloadedFilePath;
        }
    }
}