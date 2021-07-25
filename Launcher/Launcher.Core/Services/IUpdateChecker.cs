using System;
using System.Threading.Tasks;

using Launcher.Core.Models;

namespace Launcher.Core.Services
{
    public interface IUpdateChecker
    {
        event EventHandler<CheckForUpdateEventArgs> OnCheckForUpdateCompleted;
        event EventHandler<ErrorOccuredEventArgs> OnError;

        Task CheckForUpdatesAsync(string checkLink);
    }
}