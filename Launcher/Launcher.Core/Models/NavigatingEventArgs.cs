using System;

namespace Launcher.Core.Models
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