using System;
using System.Threading.Tasks;

using Launcher.Core.Models;

namespace Launcher.Core.Services
{
    public interface IUpdateChecker
    {
        UpdateDescription UpdateDescription { get; }

        Task<bool> IsNewVersionAvailable(string checkLink, Version currentVersion);

        event EventHandler<UpdateCheckErrorEventArgs> OnUpdateCheckError;
    }
}