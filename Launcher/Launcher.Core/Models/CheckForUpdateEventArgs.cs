using System;

namespace Launcher.Core.Models
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