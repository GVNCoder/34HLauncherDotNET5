using System;

namespace Launcher.Models
{
    public class NavigatingEventArgs : EventArgs
    {
        public Uri TargetSource { get; }

        public NavigatingEventArgs(Uri targetSource)
        {
            TargetSource = targetSource;
        }
    }
}