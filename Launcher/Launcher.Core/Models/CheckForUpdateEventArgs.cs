using System;

namespace Launcher.Core.Models
{
    public class CheckForUpdateEventArgs : EventArgs
    {
        public bool IsUpdateAvailable { get; }
        public UpdateDescription UpdateDescription { get; }

        public CheckForUpdateEventArgs(bool isUpdateAvailable, UpdateDescription updateDescription)
        {
            IsUpdateAvailable = isUpdateAvailable;
            UpdateDescription = updateDescription;
        }
    }
}