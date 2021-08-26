using System;
using System.Windows.Controls;

namespace Launcher.Models
{
    public class NavigatedEventArgs : EventArgs
    {
        public Uri Source { get; }
        public Page Content { get; }

        public NavigatedEventArgs(Uri source, Page content)
        {
            Source = source;
            Content = content;
        }
    }
}