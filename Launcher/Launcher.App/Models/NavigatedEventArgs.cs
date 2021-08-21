using System;

namespace Launcher.Models
{
    public class NavigatedEventArgs : EventArgs
    {
        public string Source { get; }
        public object Content { get; }

        public NavigatedEventArgs(string source, object content)
        {
            Source = source;
            Content = content;
        }
    }
}