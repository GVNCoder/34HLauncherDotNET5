using System;

namespace Launcher.Models
{
    public class CheckForUpdateEventArgs : EventArgs
    {
        public UpdateDescription UpdateDescription { get; }

        public CheckForUpdateEventArgs(UpdateDescription updateDescription)
        {
            UpdateDescription = updateDescription;
        }
    }
}