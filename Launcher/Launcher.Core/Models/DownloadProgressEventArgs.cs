using System;

namespace Launcher.Core.Models
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