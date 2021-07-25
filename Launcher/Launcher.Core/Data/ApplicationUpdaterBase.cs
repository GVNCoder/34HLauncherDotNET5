using System;

using Launcher.Core.Models;
using Launcher.Core.Services;

namespace Launcher.Core.Data
{
    public partial class ApplicationUpdater : IApplicationUpdater
    {
        public event EventHandler<ErrorOccuredEventArgs> OnError;

        #region Private helpers

        private void _RaiseOnError(string message, Exception exception = null)
            => OnError?.Invoke(this, new ErrorOccuredEventArgs(message, exception));

        #endregion
    }
}