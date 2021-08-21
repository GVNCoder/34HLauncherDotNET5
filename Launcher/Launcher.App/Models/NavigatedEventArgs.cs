using System;

namespace Launcher.Models
{
    public class NavigatedEventArgs : EventArgs
    {
        public Uri Source { get; }
        public object Content { get; }

        public NavigatedEventArgs(Uri source, object content)
        {
            Source = source;
            Content = content;
        }
    }
}