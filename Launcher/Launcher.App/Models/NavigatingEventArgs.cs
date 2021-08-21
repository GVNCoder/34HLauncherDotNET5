using System;

namespace Launcher.Models
{
    public class NavigatingEventArgs : EventArgs
    {
        public string TargetSource { get; }

        public NavigatingEventArgs(string targetSource)
        {
            TargetSource = targetSource;
        }
    }
}