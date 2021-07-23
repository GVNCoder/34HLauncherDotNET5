using System;

namespace Launcher.Core.Models
{
    public class UpdateCheckErrorEventArgs : EventArgs
    {
        public string Message { get; }
        public Exception Exception { get; }

        public UpdateCheckErrorEventArgs(string message, Exception exception)
        {
            Message = message;
            Exception = exception;
        }
    }
}