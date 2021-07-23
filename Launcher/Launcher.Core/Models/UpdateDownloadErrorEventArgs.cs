using System;

namespace Launcher.Core.Models
{
    public class UpdateDownloadErrorEventArgs : EventArgs
    {
        public string Message { get; }
        public Exception Exception { get; }

        public UpdateDownloadErrorEventArgs(string message, Exception exception)
        {
            Message = message;
            Exception = exception;
        }
    }
}