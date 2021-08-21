using System;

namespace Launcher.Models
{
    public class DownloadProgressEventArgs : EventArgs
    {
        public int PercentProgress { get; }

        public DownloadProgressEventArgs(int percentProgress)
        {
            PercentProgress = percentProgress;
        }
    }
}