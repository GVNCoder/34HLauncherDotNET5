using System;

namespace Launcher.Core.Models
{
    public class ErrorOccuredEventArgs : EventArgs
    {
        public string Message { get; }
        public Exception Exception { get; }

        public ErrorOccuredEventArgs(string message, Exception exception)
        {
            Message = message;
            Exception = exception;
        }
    }
}