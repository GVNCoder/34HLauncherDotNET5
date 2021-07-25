using System;

namespace Launcher.Core.Models.Exceptions
{
    public class FileDamagedException : Exception
    {
        public FileDamagedException() : base()
        {
        }

        public FileDamagedException(string message) : base(message)
        {
        }
    }
}