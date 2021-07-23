using System;

namespace Launcher.Core.Models
{
    public class UpdateDownloadProgressEventArgs : EventArgs
    {
        public int PercentProgress { get; }

        public UpdateDownloadProgressEventArgs(int percentProgress)
        {
            PercentProgress = percentProgress;
        }
    }
}