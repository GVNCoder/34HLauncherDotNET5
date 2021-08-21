using System;
using System.Threading.Tasks;

using Launcher.Models;

namespace Launcher.Services
{
    public interface IUpdateChecker
    {
        event EventHandler<CheckForUpdateEventArgs> OnCheckForUpdateCompleted;
        event EventHandler<ErrorOccuredEventArgs> OnError;

        Task CheckForUpdatesAsync(string checkLink);
    }
}